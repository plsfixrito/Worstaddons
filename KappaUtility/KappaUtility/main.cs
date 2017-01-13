using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using KappaUtility.Brain.Utility.Tracker.HUD;
using KappaUtility.Brain.Utility.Tracker.Units.Placements;
using KappaUtility.Common.Misc;
using KappaUtility.Common.Texture;

namespace KappaUtility
{
    internal static class main
    {
        public static string KappaUtilityFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\KappaUtility";
        public static float GameStartTime;
        private static void Main()
        {
            Loading.OnLoadingCompleteSpectatorMode += Loading_OnLoadingCompleteSpectatorMode;
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingCompleteSpectatorMode(EventArgs args)
        {
            try
            {
                var mainmenu = MainMenu.AddMenu("KappaUtility", "KappaUtility");
                GameStartTime = Game.Time;
                new Brain.Utility.Tracker.SpellTracker.SpellTracker().Load(true, mainmenu);
                new TextureManager();
                new HUDTracker(true, mainmenu);
                new TrapsTracker(true, mainmenu);
                Logger.Send("KappaUtility Loaded !");
            }
            catch (Exception ex)
            {
                Logger.Send("Error At main.Loading_OnLoadingCompleteSpectatorMode", ex, Logger.LogLevel.Error);
            }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            try
            {
                GameStartTime = Game.Time;
                Common.Load.Init();
                Brain.Load.Init();
                Logger.Send("KappaUtility Loaded !");
            }
            catch (Exception ex)
            {
                Logger.Send("Error At main.Loading_OnLoadingComplete", ex, Logger.LogLevel.Error);
            }
        }
    }
}
