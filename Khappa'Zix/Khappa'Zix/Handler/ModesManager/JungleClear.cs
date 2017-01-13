using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Khappa_Zix.Common;
using static Khappa_Zix.Handler.SpellHandler;
using static Khappa_Zix.Settings.Config.JungleClear;

namespace Khappa_Zix.Handler.ModesManager
{
    internal class JungleClear
    {
        private static bool Execute { get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear); } }
        public JungleClear()
        {
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!Execute)
                return;
            
            var wtarget = W.JungleMinions().FirstOrDefault();
            var wready = W.IsReady() && UseW && wtarget != null && Player.Instance.ManaPercent > wMana;
            if (wready)
            {
                W.Cast(wtarget, 30);
            }

            var qtarget = Q.JungleMinions().FirstOrDefault();
            if (QIsolatedOnly)
            {
                qtarget = Q.JungleMinions().FirstOrDefault(m => m.IsIsolated());
            }

            var qready = Q.IsReady() && UseQ && qtarget != null && Player.Instance.ManaPercent > QMana;
            if (qready)
            {
                Q.Cast(qtarget);
            }
        }
    }
}
