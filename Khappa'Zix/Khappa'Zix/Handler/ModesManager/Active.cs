using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Khappa_Zix.Common;
using static Khappa_Zix.Handler.SpellHandler;
using static Khappa_Zix.Settings.Config.KillSteal;

namespace Khappa_Zix.Handler.ModesManager
{
    internal class Active
    {
        private static bool Execute { get { return !Player.Instance.IsDead; } }
        public Active()
        {
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!Execute)
                return;

            KillSteal();
        }

        private static void KillSteal()
        {
            var qks = EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => QDamage(t) >= t.Health && t.IsKillable(Q.Range));
            var wks = EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => WDamage(t) >= t.Health && t.IsKillable(W.Range));
            var eks = EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => EDamage(t) >= t.Health && t.IsKillable(E.Range));

            var qready = Q.IsReady() && qks != null && UseQ;
            if (qready)
            {
                Q.Cast(qks);
                return;
            }

            var wready = W.IsReady() && wks != null && UseW;
            if (wready)
            {
                W.Cast(wks, 30);
                return;
            }
            var eready = E.IsReady() && eks != null && UseE;
            if (eready)
            {
                E.Cast(eks, 60);
            }
        }
    }
}
