using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Khappa_Zix.Common;
using static Khappa_Zix.Handler.SpellHandler;
using static Khappa_Zix.Settings.Config.LaneClear;

namespace Khappa_Zix.Handler.ModesManager
{
    internal class LaneClear
    {
        private static bool Execute { get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear); } }
        public LaneClear()
        {
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!Execute)
                return;
            
            var wtarget = W.LaneMinions().FirstOrDefault();
            var wready = UseW && W.IsReady() && wtarget != null && Player.Instance.ManaPercent > wMana;
            if (wready)
            {
                W.Cast(wtarget, 30);
            }

            var qtarget = Q.LaneMinions().FirstOrDefault();
            if (UseLQ)
            {
                qtarget = Q.LaneMinions().FirstOrDefault(m => QDamage(m) > m.Health);
            }

            var qready = (UseQ || UseLQ) && Q.IsReady() && qtarget != null && Player.Instance.ManaPercent > QMana;
            if (qready)
            {
                Q.Cast(qtarget);
            }
        }
    }
}
