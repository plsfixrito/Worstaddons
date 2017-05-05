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

namespace DarkStar_Mode
{
    class Program
    {
        private static Spell.Skillshot Q, W, E;
        private static Vector3 center = new Vector3(3798, 3738, 25);
        private static Geometry.Polygon CenterPoly = new Geometry.Polygon.Circle(center, 325);
        private static KeyBind acitve;
        private static CheckBox QonlyIntoCenter, QSelected, WSaveAlly, UseE, dontAA, drawCenter, drawE;
        private static List<AIHeroClient> enemyIntersections = new List<AIHeroClient>();
        private static AIHeroClient SelectedTarget;
        private static float lastQ;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if(Game.Type != GameType.DarkStar)
                return;

            Q = new Spell.Skillshot(SpellSlot.Q, 500000, SkillShotType.Linear, 500, 1900, 65, DamageType.Magical) {AllowedCollisionCount = 0};
            W = new Spell.Skillshot(SpellSlot.W, 50000, SkillShotType.Circular, 200, int.MaxValue, 300, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 125, 2000, 110, DamageType.Magical);

            var menu = MainMenu.AddMenu("DarkStar-Mode", "DarkStarmode");
            acitve = menu.Add("active", new KeyBind("Acitvate", false, KeyBind.BindTypes.HoldActive, 32));
            QonlyIntoCenter = menu.Add("qcenter", new CheckBox("Q To Center Only"));
            QSelected = menu.Add("QSelected", new CheckBox("Q Selected Target"));
            WSaveAlly = menu.Add("WSaveAlly", new CheckBox("W Save Allies"));
            UseE = menu.Add("UseE", new CheckBox("E Push Enemy To Center"));
            dontAA = menu.Add("dontAA", new CheckBox("Dont AA target if target health less than 1%"));
            menu.AddGroupLabel("Drawings");
            drawCenter = menu.Add("drawCenter", new CheckBox("Draw Center"));
            drawE = menu.Add("drawE", new CheckBox("Draw E"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (dontAA.CurrentValue && target.HealthPercent <= 2)
                args.Process = false;
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if(!WSaveAlly.CurrentValue || !W.IsReady() || !sender.IsAlly || sender.IsMe || args.Buff.Caster == null)
                return;

            switch (args.Buff.Type)
            {
                case BuffType.Stun:
                    if (CheckInterection(sender.ServerPosition, args.Buff.Caster.Position))
                        CastW(sender);
                    break;
                case BuffType.Slow:
                    if (sender.HealthPercent < 70 || sender.IsInRange(center, 1000))
                        CastW(sender);
                    break;
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.Q)
                lastQ = Core.GameTickCount;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(drawCenter.CurrentValue)
                CenterPoly.Draw(Color.AliceBlue, 2);
            if(drawE.CurrentValue)
                EPoly.Draw(Color.AliceBlue, 2);

            return;
            if (SelectedTarget != null)
            {
                var qpred = Q.GetPrediction(SelectedTarget);
                new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, qpred.CastPosition, Q.Width).Draw(Color.AliceBlue);
                Drawing.DrawText(SelectedTarget.ServerPosition.WorldToScreen(), Color.AliceBlue, qpred.HitChance.ToString(), 10);
            }

            if(acitve.CurrentValue && enemyIntersections != null && enemyIntersections.Any())
                foreach (var e in enemyIntersections)
                {
                    var qpred = Q.GetPrediction(e);
                    new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, qpred.CastPosition, Q.Width).Draw(Color.AliceBlue);
                    Drawing.DrawText(e.ServerPosition.WorldToScreen(), Color.AliceBlue, qpred.HitChance.ToString(), 10);
                }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if(!acitve.CurrentValue)
                return;
            
            if (Q.IsReady() && Q.ToggleState == 0 && Core.GameTickCount - lastQ > 3500)
            {
                if (QSelected.CurrentValue)
                {
                    SelectedTarget = TargetSelector.SelectedTarget;
                    if (SelectedTarget != null)
                        Q.CastMinimumHitchance(SelectedTarget, HitChance.Medium);
                }
                if (QonlyIntoCenter.CurrentValue)
                {
                    enemyIntersections = EntityManager.Heroes.Enemies.FindAll(e => e.IsValidTarget() && CheckInterection(e.ServerPosition, Player.Instance.ServerPosition));
                    var qtargets = enemyIntersections.Any(e => e.Distance(center) < 800) ?
                        enemyIntersections.OrderBy(e => e.Distance(center))
                        : enemyIntersections.OrderBy(e => e.Health);
                    var qtarget = qtargets.FirstOrDefault(e => Q.GetPrediction(e).HitChancePercent > 30 && CheckInterection(Q.GetPrediction(e).CastPosition, Player.Instance.ServerPosition));
                    if (qtarget != null)
                        Q.Cast(qtarget);
                }
                else
                {
                    var qtarget = Q.GetTarget();
                    if (qtarget != null)
                        Q.CastMinimumHitchance(qtarget, HitChance.Medium);
                }
            }

            if (UseE.CurrentValue && E.IsReady())
            {
                var target = EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget() && EPoly.IsInside(E.GetPrediction(t).CastPosition) && (E.GetPrediction(t).CastPosition.IsInRange(center, 750) || Player.Instance.IsInRange(center, 700) || EPoly.IsInside(center) || t.HealthPercent < 60));
                if (target != null)
                    E.Cast(Player.Instance.ServerPosition.Extend(center, 300).To3D());
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            var dashInfo = target.GetDashInfo();
            if (dashInfo != null)
            {
                var intersection = GetIntersectionResult(target.ServerPosition, dashInfo.EndPos);
                if (intersection.WillHit)
                    W.Cast(intersection.WillHit ? intersection.Point.To3D() : dashInfo.EndPos);
            }
            W.Cast(target);
        }

        private static bool CheckInterection(Vector3 start, Vector3 end)
        {
            return GetIntersectionResult(start, end).WillHit;
        }

        private static IntersectionResult GetIntersectionResult(Vector3 start, Vector3 end)
        {
            return new IntersectionResult(start.To2D(), end.To2D());
        }

        private static Geometry.Polygon EPoly
            => new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition.Extend(center, E.Range / 2f), Player.Instance.ServerPosition.Extend(center, -(E.Range / 2f)), E.Width);

        public class IntersectionResult
        {
            public IntersectionResult(Vector2 start, Vector2 end)
            {
                var intersectionPoints = new List<Vector2>();
                var points = CenterPoly.Points;
                for (int j = 0; j < points.Count; j++)
                {
                    var intersection = points[j].Intersection(points[j == points.Count - 1 ? 0 : j + 1], start, end);
                    if (intersection.Intersects)
                    {
                        intersectionPoints.Add(intersection.Point);
                        break;
                    }
                }

                this.WillHit = intersectionPoints.Any();
                this.Points = intersectionPoints;
                this.Point = intersectionPoints.OrderBy(p => p.Distance(start)).FirstOrDefault();
            }

            public bool WillHit;
            public Vector2 Point;
            public List<Vector2> Points;
            public Vector2[] Path;
        }
    }
}
