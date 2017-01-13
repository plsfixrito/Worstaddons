using System;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain
{
    internal class Load
    {
        public static void Init()
        {
            try
            {
                Utility.Load.Init();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At Brain.Load.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
