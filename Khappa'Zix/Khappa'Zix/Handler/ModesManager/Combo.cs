using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Khappa_Zix.Common;
using static Khappa_Zix.Handler.SpellHandler;
using static Khappa_Zix.Settings.Config.Combo;

namespace Khappa_Zix.Handler.ModesManager
{
    internal class Combo
    {
        private static bool Execute { get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo); } }
        public Combo()
        {
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (IsRStealthed && NoAAStealth)
            {
                args.Process = false;
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if(!Execute)
                return;

            var target = W.GetTarget();
            var isolatedtarget = EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => t.IsKillable(W.Range) && t.IsIsolated());

            var wready = UseW && W.IsReady();

            if (!wready)
            {
                target = Q.GetTarget();
            }

            if (IsolatedCombo && isolatedtarget != null)
            {
                target = isolatedtarget;
            }

            if (target != null)
            {
                if (IsolatedComboOnly && !target.IsIsolated())
                {
                    return;
                }

                if (wready)
                {
                    W.Cast(target, 45);
                }

                var qready = UseQ && Q.IsReady() && target.IsKillable(Q.Range);
                if (qready)
                {
                    Q.Cast(target);
                }

                var rready = UseR && R.IsReady();
                if (rready)
                {
                    if (Player.Instance.CountEnemyHeroesInRangeWithPrediction((int)E.Range) > Player.Instance.CountEnemyAlliesInRangeWithPrediction((int)E.Range))
                    {
                        R.Cast();
                    }
                }

                var eready = UseE && E.IsReady() && target.IsKillable(E.Range);
                if (eready)
                {
                    if (ExecuteTargetE && ComboDamage(target) >= target.Health)
                    {
                        E.Cast(target, 30);
                    }

                    var pred = E.GetPrediction(target);
                    if (NoETurrets && pred.CastPosition.UnderEnemyTurret())
                    {
                        return;
                    }

                    if (MinEDistance > target.Distance(Player.Instance))
                    {
                        return;
                    }

                    if (pred.CastPosition.CountEnemyHeroesInRangeWithPrediction(1000) >= NoEIntoCount)
                    {
                        return;
                    }

                    E.Cast(target, 30);
                }
            }
        }
    }
}
