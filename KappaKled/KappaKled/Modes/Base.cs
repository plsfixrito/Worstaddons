using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using static KappaKled.Program;

namespace KappaKled.Modes
{
    class Base
    {
        public static void Init()
        {
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            Auto.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) LaneClear.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) LaneClear.Execute(true);
            //Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(sender == null || !sender.IsEnemy || !Q.IsReady()) return;

            if(AutoMenu["GapQkled"].Cast<CheckBox>().CurrentValue && State.MyCurrentState(State.Current.Kled))
                Q.Cast(sender);

            if (AutoMenu["GapQskaarl"].Cast<CheckBox>().CurrentValue && State.MyCurrentState(State.Current.Skaarl))
                Q.Cast(sender, HitChance.Low);
        }
    }
}
