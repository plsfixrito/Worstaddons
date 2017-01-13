using System;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Misc
{
    internal class Load
    {
        internal static void Init()
        {
            try
            {
                AutoLvlup.Autolvlup.Init();
                Lantern.PickLantern.Init();
                Pings.PingDetector.Init();
                Skin.SkinChanger.Init();
                Warding.WardsHelper.Init();
                Warding.AntiStealth.Init();
                DPSCalculator.DPS.Init();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Misc.Load.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
