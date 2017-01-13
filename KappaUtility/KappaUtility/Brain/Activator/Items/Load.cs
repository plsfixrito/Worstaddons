using System;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Activator.Items
{
    internal class Load
    {
        internal static void Init()
        {
            try
            {
                Defence.Qss.Init();
                Defence.Potions.Init();
                Defence.Def.Init();
                Offence.items.Init();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Items.Load.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
