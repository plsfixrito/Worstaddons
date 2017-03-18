using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaBaseUlt
{
    public class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static Baseult BaseUltSpell;
        private static Menu MenuIni;

        public static bool Enabled => MenuIni.Get<KeyBind>("enable").CurrentValue;
        public static bool DisableKey => MenuIni.Get<KeyBind>("disable").CurrentValue;
        public static bool Draw => MenuIni.Get<CheckBox>("draw").CurrentValue;
        public static int FocusMode => MenuIni.Get<ComboBox>("focus").CurrentValue;
        public static int Tolerance => MenuIni.Get<Slider>("tolerance").CurrentValue;
        public static int FowTolerance => MenuIni.Get<Slider>("fow").CurrentValue;
        public static int X => MenuIni.Get<Slider>("x").CurrentValue;
        public static int Y => MenuIni.Get<Slider>("y").CurrentValue;

        public static Dictionary<int, Baseult> InGameUlts = new Dictionary<int, Baseult>();
        public static List<TrackedRecall> TrackedRecalls = new List<TrackedRecall>();
        public static List<NotVisiable> NotVisiableEnemies = new List<NotVisiable>();

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if(!loadUlts())
                return;

            MenuIni = MainMenu.AddMenu("KappaBaseUlt", "kappabaseult");
            MenuIni.AddGroupLabel("KappaBaseUlt");
            MenuIni.Add("focus", new ComboBox("Priority mode", 0, "Target Selector Priority", "Least Health", "First Recall", "Last Recall"));
            MenuIni.Add("enable", new KeyBind("Enable", true, KeyBind.BindTypes.PressToggle));
            MenuIni.Add("disable", new KeyBind("Force Disable", false, KeyBind.BindTypes.HoldActive));
            MenuIni.Add("fow", new Slider("FoW Tolerance 0 = Always (In Seconds)", 7, 0, 60));
            MenuIni.Add("tolerance", new Slider("Shoot Tolerance (In MilliSeconds)", 0, -150, 300));
            MenuIni.AddLabel("More value = delay | Less value = Early");
            MenuIni.AddSeparator(0);

            MenuIni.AddGroupLabel("Enemies To BaseUlt");
            foreach (var enemy in EntityManager.Heroes.Enemies)
                MenuIni.Add(enemy.BaseSkinName, new CheckBox($"BaseUlt {enemy.BaseSkinName}"));
            MenuIni.AddSeparator(0);

            MenuIni.AddGroupLabel("Drawings");
            MenuIni.Add("draw", new CheckBox("Draw Recall Tracker"));
            MenuIni.Add("x", new Slider("Drawing X", 85));
            MenuIni.Add("y", new Slider("Drawing Y", 40));

            Game.OnUpdate += Game_OnUpdate;
            Teleport.OnTeleport += Teleport_OnTeleport;
            Drawing.OnEndScene += Drawing_OnEndScene;
            //Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if(caster == null || !caster.IsAlly || caster.IsMe)
                return;

            Baseult baseUlt;
            try { baseUlt = InGameUlts[caster.NetworkId]; }
            catch (Exception) { return; }

            if(baseUlt.Slot != args.Slot)
                return;
            
            if(!TrackedRecalls.Any(r => r.CastPosition(baseUlt, caster).IsInRange(args.End, 275) && r.TicksLeft + Game.Ping >= baseUlt.TravelTime(r.CastPosition(baseUlt, caster)) - 50))
                return;
            
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var spelldata = getSpell(sender.Owner, args.Slot).SData;
            Console.WriteLine($"{spelldata.Name} - {sender.Owner.Spellbook.GetSpell(args.Slot).SData.Name} - {spelldata.MissileSpeed} - {spelldata.CastTime}");
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if(!Draw)
                return;

            var drawText = "Tracked Recalls:\n";
            var end = new Vector3();
            foreach (var recall in TrackedRecalls)
            {
                var castPos = recall.CastPosition(BaseUltSpell, Player.Instance);
                var canBaseUlt = CanBaseUlt(recall, BaseUltSpell, Player.Instance);
                drawText += $"- {recall.Caster.BaseSkinName}: {recall.TicksLeft/*} | {BaseUltSpell.TravelTime(castPos)} | {healthAfterTime(recall.Caster, BaseUltSpell.TravelTime(castPos))*/} {(canBaseUlt ? "| CanBaseUlt" : recall.Ulted ? "| Ulted" : "")}\n";
                end = castPos;
            }

            //end.DrawCircle(100, SharpDX.Color.AliceBlue);
            Drawing.DrawText((int)(Drawing.Width * (X * 0.01f)), (int)(Drawing.Height * (Y * 0.01f)), Color.AliceBlue, drawText);
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if(caster == null || !caster.IsEnemy)
                return;

            if (args.Type == TeleportType.Recall && args.Status == TeleportStatus.Start)
            {
                var spawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(o => o.Team == caster.Team);
                if (spawn != null)
                {
                    TrackedRecalls.Add(new TrackedRecall
                    {
                        Caster = caster,
                        RecallDuration = args.Duration,
                        StartTick = Core.GameTickCount,
                        EndPosition = spawn.Position
                    });
                }
            }

            if (args.Status == TeleportStatus.Finish || args.Status == TeleportStatus.Abort || args.Status == TeleportStatus.Unknown)
            {
                TrackedRecalls.RemoveAll(t => t.Caster.IdEquals(caster));
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if(!enemy.IsHPBarRendered && !NotVisiableEnemies.Any(e => e.Target.IdEquals(enemy)))
                    NotVisiableEnemies.Add(new NotVisiable(enemy));

                if (enemy.IsHPBarRendered && NotVisiableEnemies.Any(e => e.Target.IdEquals(enemy)))
                    NotVisiableEnemies.RemoveAll(t => t.Target.IdEquals(enemy));
            }

            if(BaseUltSpell == null || !TrackedRecalls.Any())
                return;

            TrackedRecalls.RemoveAll(t => t.Ended);

            if(!Enabled || DisableKey)
                return;

            TrackedRecall recallTarget = null;

            switch (FocusMode)
            {
                case 0:
                    recallTarget = TrackedRecalls.OrderByDescending(t => TargetSelector.GetPriority(t.Caster)).FirstOrDefault(t => CanBaseUlt(t, BaseUltSpell, Player.Instance));
                    break;
                case 1:
                    recallTarget = TrackedRecalls.OrderBy(t => healthAfterTime(t.Caster, BaseUltSpell.TravelTime(t.CastPosition(BaseUltSpell, Player.Instance)))).FirstOrDefault(t => CanBaseUlt(t, BaseUltSpell, Player.Instance));
                    break;
                case 2:
                    recallTarget = TrackedRecalls.OrderBy(t => t.StartTick).FirstOrDefault(t => CanBaseUlt(t, BaseUltSpell, Player.Instance));
                    break;
                case 3:
                    recallTarget = TrackedRecalls.OrderByDescending(t => t.StartTick).FirstOrDefault(t => CanBaseUlt(t, BaseUltSpell, Player.Instance));
                    break;
                default:
                    recallTarget = TrackedRecalls.OrderByDescending(t => TargetSelector.GetPriority(t.Caster)).FirstOrDefault(t => CanBaseUlt(t, BaseUltSpell, Player.Instance));
                    break;
            }

            if(recallTarget == null)
                return;
            
            var travelTime = BaseUltSpell.TravelTime(recallTarget.CastPosition(BaseUltSpell, Player.Instance));
            var offset = 50 + Game.Ping + Tolerance;
            var mod = recallTarget.TicksLeft - travelTime;
            if (offset >= mod && BaseUltSpell.Cast(recallTarget.CastPosition(BaseUltSpell, Player.Instance)))
            {
                recallTarget.Ulted = true;
            }
        }

        private static SpellDataInst getSpell(Obj_AI_Base target, SpellSlot slot)
        {
            return target.Spellbook.GetSpell(slot);
        }

        private static float calculateDamage(AIHeroClient target, AIHeroClient source, Baseult spell)
        {
            return spell.CalculateDamage(source, target);
        }

        private static float healthAfterTime(AIHeroClient unit, float time)
        {
            var staticHPRegen = unit.CharData.BaseStaticHPRegen;
            var notVisiable = NotVisiableEnemies.FirstOrDefault(t => t.Target.IdEquals(unit));
            return Math.Min(unit.TotalShieldMaxHealth(), unit.TotalShieldHealth() + (staticHPRegen * ((notVisiable?.TicksPassed/1000f ?? 0f) + (time/1000f))));
        }

        public static bool CanBaseUlt(TrackedRecall recall, Baseult spell, AIHeroClient source)
        {
            var visiable = NotVisiableEnemies.FirstOrDefault(e => e.Target.IdEquals(recall.Caster));
            return !recall.Ulted
                && spell.IsReady()
                && (FowTolerance == 0 || visiable == null || FowTolerance * 1000 > visiable.TicksPassed - recall.TicksPassed)
                && MenuIni.Get<CheckBox>(recall.Caster.BaseSkinName).CurrentValue
                && spell.IsInRange(recall.CastPosition(spell, source))
                && recall.TicksLeft > spell.TravelTime(recall.CastPosition(spell, source))
                && Collision.Check(source, recall.CastPosition(spell, source), spell, recall.Caster)
                && calculateDamage(recall.Caster, source, spell) >= healthAfterTime(recall.Caster, spell.TravelTime(recall.CastPosition(spell, source)));
        }

        private static bool loadUlts()
        {
            foreach (var a in EntityManager.Heroes.Allies)
            {
                if (a.ChampionName.Equals("Ashe"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, int.MaxValue, 140, 250, 1600)
                    {
                        RawDamage = () => (200f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage,
                        AllowedCollisionCount = 1
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Draven"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Physical, int.MaxValue, 160, 300, 2000)
                    {
                        RawDamage = () => 75f + (100f * getSpell(a, SpellSlot.R).Level) + a.FlatPhysicalDamageMod * 1.1f,
                        AllowedCollisionCount = -1,
                        NameCheck = "DravenRCast"
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Ezreal"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, int.MaxValue, 160, 1000, 2000)
                    {
                        RawDamage = () => 200f + (150f * getSpell(a, SpellSlot.R).Level) + a.FlatPhysicalDamageMod + (a.TotalMagicalDamage * 0.9f),
                        AllowedCollisionCount = -1,
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Jinx"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Physical, int.MaxValue, 140, 550, 2100)
                    {
                        RawDamage = () => 150f + (100f * getSpell(a, SpellSlot.R).Level) + a.FlatPhysicalDamageMod * 1.5f,
                        AllowedCollisionCount = 1,
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Karthus"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, int.MaxValue, -1, 3000, int.MaxValue)
                    {
                        RawDamage = () => 100f + (150f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage * 0.6f,
                        AllowedCollisionCount = int.MaxValue,
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Lux"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, 3500, 150, 950, int.MaxValue)
                    {
                        RawDamage = () => 200f + (100f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage * 0.75f,
                        AllowedCollisionCount = int.MaxValue
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Xerath"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, 3520, 200, 600, int.MaxValue)
                    {
                        RawDamage = () => 170f + (30f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage * 0.43f,
                        AllowedCollisionCount = int.MaxValue,
                        NameCheck = "XerathRMissileWrapper",
                        RangeGrow = 1320,
                        SkillType = SkillShotType.Circular
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Pantheon"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, 5500, 700, 4600, int.MaxValue)
                    {
                        RawDamage = () => 100f + (300f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage,
                        AllowedCollisionCount = int.MaxValue,
                        NameCheck = "PantheonRJump"
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Ziggs"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, 5250, 275, 250, 1750)
                    {
                        RawDamage = () => 150f + (150f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage * 1.1f,
                        AllowedCollisionCount = int.MaxValue,
                        SkillType = SkillShotType.Circular
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Gragas"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Magical, 1100, 400, 250, 1600)
                    {
                        RawDamage = () => 100f + (100f * getSpell(a, SpellSlot.R).Level) + a.TotalMagicalDamage * 0.7f,
                        AllowedCollisionCount = -1,
                        SkillType = SkillShotType.Circular
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }

                if (a.ChampionName.Equals("Graves"))
                {
                    BaseUltSpell = new Baseult(SpellSlot.R, DamageType.Physical, 1250, 100, 250, 2100)
                    {
                        RawDamage = () => 100f + (150f * getSpell(a, SpellSlot.R).Level) + a.FlatPhysicalDamageMod * 1.5f,
                        AllowedCollisionCount = 1,
                    };

                    InGameUlts.Add(a.NetworkId, BaseUltSpell);
                }
            }

            BaseUltSpell = null;

            try { BaseUltSpell = InGameUlts[Player.Instance.NetworkId]; }
            catch (Exception) { return false; }

            return BaseUltSpell != null;
        }

        public class NotVisiable
        {
            public NotVisiable(AIHeroClient target)
            {
                this.Target = target;
            }
            public AIHeroClient Target;
            private float _startTick = Core.GameTickCount;
            public float TicksPassed => Core.GameTickCount - this._startTick;
        }
    }
}
