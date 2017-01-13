using System;
using KappaUtility.Brain.Utility.Tracker.HUD;
using KappaUtility.Brain.Utility.Tracker.Units.Ganks;
using KappaUtility.Brain.Utility.Tracker.Units.Placements;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Tracker
{
    internal class Load
    {
        public static void Init()
        {
            try
            {
                GanksDetector.Init();
                WardsTracker.Init();
                new SpellTracker.SpellTracker().Load();
                TeleportTracker.TeleportTracker.Init();
                new HUDTracker();
                new TrapsTracker();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Tracker.Load.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
