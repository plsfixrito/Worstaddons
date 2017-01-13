using System;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Activator
{
    internal class Load
    {
        internal static Menu MenuIni;

        public static void Init()
        {
            try
            {
                MenuIni = MainMenu.AddMenu("KappActivator", "KappActivator");
                MenuIni.AddGroupLabel("[PRESS F5 FOR THESE CHANGES TO TAKE AFFECT]");
                var autoshield = MenuIni.CreateCheckBox("Autoshield", "Enable Autoshield").CurrentValue;
                var summonerspells = MenuIni.CreateCheckBox("SummonerSpells", "Enable SummonerSpells").CurrentValue;
                var items = MenuIni.CreateCheckBox("Items", "Enable Items").CurrentValue;

                if (autoshield)
                    Spells.AutoShield.AutoShield.Init();
                if (summonerspells)
                    Spells.SummonerSpells.Summs.Init();
                if (items)
                    Items.Load.Init();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At Brain.Load.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
