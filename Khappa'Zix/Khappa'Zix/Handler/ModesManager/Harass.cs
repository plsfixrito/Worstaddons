using System;
using EloBuddy;
using EloBuddy.SDK;
using Khappa_Zix.Common;
using static Khappa_Zix.Handler.SpellHandler;
using static Khappa_Zix.Settings.Config.Harass;

namespace Khappa_Zix.Handler.ModesManager
{
    internal class Harass
    {
        private static bool Execute { get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass); } }
        public Harass()
        {
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if(!Execute)
                return;

            var target = W.GetTarget();
            var wready = UseW && W.IsReady();

            if (!wready)
            {
                target = Q.GetTarget();
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
            }
        }
    }
}
