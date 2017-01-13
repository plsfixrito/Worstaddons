using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace GenesisUrgot
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "Urgot";

        public static void Main(string[] args)
        {
            // Wait till the loading screen has passed
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        public static int TearTick = 0;

        private static void OnLoadingComplete(EventArgs args)
        {
            // Verify the champion we made this addon for
            if (Player.Instance.ChampionName != ChampName)
            {
                // Champion is not the one we made this addon for,
                // therefore we return
                return;
            }

            // Initialize the classes that we need
            KappaEvade.KappaEvade.Init();
            Events.Init();
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            LevelManager.Initialize();

            // Listen to events we need
            Interrupter.OnInterruptableSpell += InterruptManager.OnInterruptable;
            Events.OnIncomingDamage += ShieldManager.Events_OnIncomingDamage;
            Orbwalker.OnAttack += Modes.LaneClear.OnAttack;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            Circle.Draw(Color.Red, SpellManager.Q1.Range, Player.Instance.Position);
            // TODO: Uncomment if you want those enabled aswell, but remember to enable them
            // TODO: in the SpellManager aswell, otherwise you will get a NullReferenceException
            //Circle.Draw(Color.Red, SpellManager.W.Range, Player.Instance.Position);
            //Circle.Draw(Color.Red, SpellManager.E.Range, Player.Instance.Position);
            Circle.Draw(Color.Red, SpellManager.R.Range, Player.Instance.Position);
        }
    }
}
