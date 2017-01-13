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

namespace KappaBaseUlt
{
    internal static class Program
    {
        private static float Changed;
        private static int Counter;
        private static Menu baseMenu;
        private static Spell.Skillshot R { get; set; }

        private static readonly List<EnemyInfo> baseultlist = new List<EnemyInfo>();
        private static readonly List<EnemyInfo> RecallsList = new List<EnemyInfo>();
        
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            var me = Database.Champions.FirstOrDefault(hero => hero.Champion == Player.Instance.Hero);
            if (me?.Champion != Player.Instance.Hero)
            {
                return;
            }

            R = new Spell.Skillshot(me.Slot, me.Range, me.Type, me.CastDelay, me.Speed, me.Width);

            baseMenu = MainMenu.AddMenu("KappaBaseUlt", "KappaBaseUlt");
            baseMenu.AddGroupLabel(Player.Instance.Hero + " BaseUlt");
            baseMenu.AddSeparator(0);
            baseMenu.AddGroupLabel("Key Settings:");
            baseMenu.Add("enable", new KeyBind("Enable BaseUlt", true, KeyBind.BindTypes.PressToggle, 'K'));
            baseMenu.Add("disable1", new KeyBind("Disable Key", false, KeyBind.BindTypes.HoldActive));
            baseMenu.AddSeparator(0);
            baseMenu.AddGroupLabel("Settings:");
            baseMenu.Add("ping", new CheckBox("Calculate Ping"));
            baseMenu.Add("col", new CheckBox("Check Collison"));
            baseMenu.Add("limit", new Slider("FoW Time Limit [{0}]", 120, 0, 180));
            baseMenu.AddLabel("0 = Always");
            baseMenu.AddSeparator(0);
            baseMenu.AddGroupLabel("Drawings:");
            baseMenu.Add("count", new CheckBox("Draw Count"));
            baseMenu.Add("draw", new CheckBox("Draw Recall Bar"));
            var x = baseMenu.Add("rbx", new Slider("RecallBar X", 0, -200, 200));
            x.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    if (changeArgs.NewValue != changeArgs.OldValue)
                    {
                        Recallbar.X2 = changeArgs.NewValue * 10;
                        Changed = Core.GameTickCount;
                    }
                };
            Recallbar.X2 = x.CurrentValue * 10;
            var y = baseMenu.Add("rby", new Slider("RecallBar Y", 0, -200, 200));
            y.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    if (changeArgs.NewValue != changeArgs.OldValue)
                    {
                        Recallbar.Y2 = changeArgs.NewValue * 10;
                        Changed = Core.GameTickCount;
                    }
                };
            Recallbar.Y2 = y.CurrentValue * 10;
            baseMenu.AddGroupLabel("BaseUlt Enemies:");
            baseultlist.Clear();
            RecallsList.Clear();
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                baseMenu.Add(enemy.NetworkId.ToString(), new CheckBox("Use On " + enemy.BaseSkinName + " - (" + enemy.Name + ")"));
                baseultlist.Add(new EnemyInfo(enemy));
            }

            Game.OnUpdate += Game_OnTick;
            Teleport.OnTeleport += Teleport_OnTeleport;
            Drawing.OnEndScene += Drawing_OnDraw;
        }

        private static void Game_OnTick(EventArgs args)
        {
            foreach (var enemy in baseultlist.Where(e => e.Enemy.IsHPBarRendered && !e.Enemy.IsDead))
            {
                enemy.lastseen = Game.Time;
            }

            foreach (var enemy in RecallsList.Where(d => baseMenu[d.Enemy.NetworkId.ToString()].Cast<CheckBox>().CurrentValue && d.Duration > 0))
            {
                DoBaseUlt(enemy);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (baseMenu["count"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawText(Drawing.Height * 0.25f, Drawing.Width * 0.1f, System.Drawing.Color.GreenYellow, $"PossibleBaseUlts: {Counter}");
            }
            if (!baseMenu["draw"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            /*
            var X = Player.Instance.ServerPosition.WorldToScreen().X;
            var Y = Player.Instance.ServerPosition.WorldToScreen().Y;
            float i = 0;
            */
            foreach (var player in RecallsList.Where(e => baseMenu[e.Enemy.NetworkId.ToString()].Cast<CheckBox>().CurrentValue && e.Duration > 0))
            {
                /*
                var lastseen = baseultlist.FirstOrDefault(e => e.Enemy.NetworkId == player.Enemy.NetworkId);
                Drawing.DrawText(
                    X,
                    Y + i,
                    System.Drawing.Color.White,
                    player.Enemy.BaseSkinName + " | CountDown: " + (int)(player.CountDown()) + " | TravelTime: " + (int)player.Enemy.traveltime()
                    + " | LastSeen: " + (int)(Game.Time - lastseen?.lastseen) + " | Damage: " + (int)(player.Enemy.GetDamage()) + " | Health: "
                    + (int)player.Enemy.HP(),
                    5);
                i += 20f;
                */
                player.RecallBarDraw();
            }
            if (Core.GameTickCount - Changed < 3000)
            {
                new EnemyInfo(Player.Instance).RecallBarDraw();
            }
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (!sender.IsEnemy || args.Type != TeleportType.Recall)
            {
                return;
            }
            var hero = sender as AIHeroClient;
            if(hero == null)
                return;
            if (args.Status == TeleportStatus.Start)
            {
                if (RecallsList.Exists(s => s.Enemy.NetworkId.Equals(hero.NetworkId)))
                {
                    RecallsList.Add(new EnemyInfo(hero) { Duration = args.Duration, Started = args.Start, RecallDuration = args.Duration + Core.GameTickCount });
                }
                else
                {
                    RecallsList.Add(new EnemyInfo(hero) { Duration = args.Duration, Started = args.Start, RecallDuration = args.Duration + Core.GameTickCount });
                }

                if (args.Duration >= hero.traveltime() && hero.Killable())
                {
                    Counter ++;
                }
            }
            else
            {
                var remove = RecallsList.FirstOrDefault(r => r.Enemy.NetworkId.Equals(hero.NetworkId));
                if (remove == null)
                {
                    return;
                }
                RecallsList.Remove(remove);
                removeFromList(remove.Enemy);
            }
        }

        public static bool Killable(this Obj_AI_Base target)
        {
            var enemy = baseultlist.FirstOrDefault(e => e.Enemy.NetworkId.Equals(target.NetworkId));
            return enemy?.Enemy.GetDamage() >= target.Health;
        }

        /*
        public static float HP(this Obj_AI_Base target)
        {
            var enemy = baseultlist.FirstOrDefault(e => e.Enemy.NetworkId.Equals(target.NetworkId));
            var f = (enemy?.Enemy.Health + (enemy?.Enemy.HPRegenRate * (Game.Time - enemy?.lastseen))) * 0.9f;
            return f ?? 0;
        }*/

        public static float GetDamage(this AIHeroClient target)
        {
            var dmg = Player.Instance.GetSpellDamage(target, SpellSlot.R);
            var extradmg = 0f;
            if (Player.Instance.HasBuff("GangplankRUpgrade2"))
            {
                extradmg = dmg * 3F;
            }
            return !R.IsLearned ? 0 : dmg + extradmg;
        }

        private static void removeFromList(Obj_AI_Base sender)
        {
            var recall = RecallsList.FirstOrDefault(x => x.Enemy.NetworkId.Equals(sender.NetworkId));

            if (recall == null)
            {
                return;
            }

            RecallsList.Remove(recall);
        }

        public static float traveltime(this Obj_AI_Base target)
        {
            var hero = Player.Instance.Hero;
            var pos = target.Fountain();
            var distance = Player.Instance.Distance(pos);
            var speed = R.Speed;
            var delay = CastDelay(baseMenu["ping"].Cast<CheckBox>().CurrentValue);

            switch (hero)
            {
                case Champion.Lux:
                case Champion.Karthus:
                case Champion.Pantheon:
                case Champion.Gangplank:
                    return delay;
            }

            return distance / speed * 1000f;
        }

        public static float CastDelay(bool c)
        {
            return c ? R.CastDelay - (Game.Ping / 2f) : R.CastDelay;
        }

        private static void DoBaseUlt(EnemyInfo target)
        {
            var disable = baseMenu["disable1"].Cast<KeyBind>().CurrentValue;
            var enable = baseMenu["enable"].Cast<KeyBind>().CurrentValue;
            var CountDown = target.CountDown();
            var Traveltime = traveltime(target.Enemy);

            if (enable && !disable)
            {
                if (R.IsReady() && lastseen(target) && CountDown >= Traveltime && target.Enemy.Killable())
                {
                    if (CountDown - Traveltime <= CastDelay(baseMenu["ping"].Cast<CheckBox>().CurrentValue) && !target.Enemy.Collison())
                    {
                        Console.WriteLine("BASEULT");
                        Player.Instance.Spellbook.CastSpell(R.Slot, target.Enemy.Fountain());
                    }
                }
            }
        }

        private static bool lastseen(EnemyInfo target)
        {
            var enemy = baseultlist.FirstOrDefault(e => e.Enemy.NetworkId == target.Enemy.NetworkId);
            float timelimit = baseMenu["limit"].Cast<Slider>().CurrentValue;
            if (enemy != null)
            {
                if (timelimit.Equals(0))
                {
                    return true;
                }
                return Game.Time - enemy.lastseen <= timelimit;
            }
            return Game.Time - target?.lastseen <= timelimit;
        }

        public static float CountDown(this EnemyInfo target)
        {
            return target.Started + target.Duration - Core.GameTickCount;
        }

        private static bool Collison(this Obj_AI_Base target)
        {
            var col = baseMenu["col"].Cast<CheckBox>().CurrentValue;
            var Rectangle = new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, target.Fountain(), R.Width);

            return col && EntityManager.Heroes.Enemies.Count(e => Rectangle.IsInside(e) && e.IsValidTarget()) > R.AllowedCollisionCount;
        }

        private static Vector3 Fountain(this Obj_AI_Base target)
        {
            var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.Team == target.Team);
            return objSpawnPoint?.Position ?? Vector3.Zero;
        }

        public class EnemyInfo
        {
            public AIHeroClient Enemy;
            public float lastseen;
            public float RecallDuration;
            public float Duration;
            public float Started;
            public EnemyInfo(AIHeroClient enemy)
            {
                this.Enemy = enemy;
                this.lastseen = 0f;
                this.Duration = 0f;
            }
        }
    }
}
