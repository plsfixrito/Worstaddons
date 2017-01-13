using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace LuxUltBlocker
{
    class Program
    {
        public static Menu Menuini;

        public struct Supported
        {
            public Champion champ;
            public SpellSlot slot;
            public int Range;
        }

        public struct DetectedLuxUlt
        {
            public AIHeroClient Caster;
            public Vector3 Start;
            public Vector3 End;
            public int EndTime;
            public Geometry.Polygon.Rectangle SpellPoly;
        }

        public static List<DetectedLuxUlt> DetectedUlts = new List<DetectedLuxUlt>();

        public static List<Supported> SupChampions = new List<Supported>
        {
            new Supported { champ = Champion.Nocturne, slot = SpellSlot.R },
            new Supported { champ = Champion.Graves, slot = SpellSlot.W },
            new Supported { champ = Champion.MonkeyKing, slot = SpellSlot.W },
            new Supported { champ = Champion.Shaco, slot = SpellSlot.Q },
            new Supported { champ = Champion.Khazix, slot = SpellSlot.R },
        };

        public static SpellSlot Stealth;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (EntityManager.Heroes.Enemies.All(e => e.Hero != Champion.Lux)) return;
            if (SupChampions.All(s => s.champ != Player.Instance.Hero)) return;
                
            var champ = SupChampions.FirstOrDefault(s => s.champ == Player.Instance.Hero);
            Menuini = MainMenu.AddMenu("Anti-LuxUlt", "Anti-LuxUlt");
            Stealth = champ.slot;
            
            Menuini.Add(Stealth.ToString(), new CheckBox("Use " + champ.champ + " - " + champ.slot));
            if (champ.champ == Champion.Graves || champ.champ == Champion.Nocturne)
            {
                Menuini.Add("ally", new CheckBox("Use for Allies"));
            }

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var LuxUlt in DetectedUlts.Where(e => e.EndTime - Game.Time >= 0))
            {
                LuxUlt.SpellPoly.Draw(System.Drawing.Color.AliceBlue);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsEnemy || caster.Hero != Champion.Lux || args.Slot != SpellSlot.R) return;

            var endpos = args.Start.Extend(args.End, 3500).To3D();
            DetectedUlts.Add(new DetectedLuxUlt { Caster = caster, Start = args.Start, End = endpos, EndTime = (int)(Game.Time + 1.5), SpellPoly = new Geometry.Polygon.Rectangle(args.Start, endpos, 190) });
        }

        private static void Game_OnTick(EventArgs args)
        {
            var spell = Player.GetSpell(Stealth);
            foreach (var LuxUlt in DetectedUlts.Where(e => e.EndTime - Game.Time >= 0))
            {
                if (Menuini[Stealth.ToString()].Cast<CheckBox>().CurrentValue && spell.IsReady && spell.IsLearned)
                {
                    if (Player.Instance.Hero == Champion.Graves || Player.Instance.Hero == Champion.Nocturne)
                    {
                        var allydanger = EntityManager.Heroes.Allies.FirstOrDefault(a => LuxUlt.SpellPoly.IsInside(a) && !a.IsMe && a.IsValidTarget() && !a.IsDead);
                        if (Menuini["ally"].Cast<CheckBox>().CurrentValue && allydanger != null)
                        {
                            if (Player.Instance.Hero == Champion.Graves)
                            {
                                if (LuxUlt.Caster != null && LuxUlt.Caster.IsValidTarget(spell.SData.CastRange))
                                {
                                    Player.CastSpell(Stealth, LuxUlt.Caster.ServerPosition);
                                }
                            }
                            if (Player.Instance.Hero == Champion.Nocturne)
                            {
                                Player.CastSpell(Stealth);
                            }
                        }
                    }
                    if (LuxUlt.SpellPoly.IsInside(Player.Instance))
                    {
                        if (Player.Instance.Hero == Champion.Graves)
                        {
                            if (LuxUlt.Caster != null && LuxUlt.Caster.IsValidTarget(spell.SData.CastRange))
                            {
                                Player.CastSpell(Stealth, LuxUlt.Caster.ServerPosition);
                            }
                        }
                        if (Player.Instance.Hero == Champion.Nocturne || Player.Instance.Hero == Champion.Khazix || Player.Instance.Hero == Champion.MonkeyKing)
                        {
                            Player.CastSpell(Stealth);
                        }
                        if (Player.Instance.Hero == Champion.Shaco)
                        {
                            Player.CastSpell(Stealth, Prediction.Position.PredictUnitPosition(Player.Instance, 500).To3D());
                        }
                    }
                }
            }
            DetectedUlts.RemoveAll(ult => ult.EndTime - Game.Time < 0);
        }
    }
}
