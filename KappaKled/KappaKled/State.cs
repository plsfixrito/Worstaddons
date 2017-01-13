using System;
using EloBuddy;
using EloBuddy.SDK;
using static KappaKled.Program;

namespace KappaKled
{
    class State
    {
        public enum Current
        {
            Skaarl, Kled
        }

        public static Current MyState;

        public static void Init()
        {
            Game.OnTick += Game_OnTick; 
        }

        private static void Game_OnTick(EventArgs args)
        {
            // Kled AA Range > Skaarl
            MyState = user.GetAutoAttackRange() > 225 ? Current.Kled : Current.Skaarl;
        }

        public static bool MyCurrentState(Current state)
        {
            return MyState == state;
        }
    }
}
