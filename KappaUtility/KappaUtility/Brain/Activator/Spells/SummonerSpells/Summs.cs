using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells
{
    internal class Summs
    {
        internal static Menu menu;

        public static void Init()
        {
            try
            {
                menu = Load.MenuIni.AddSubMenu("SummonerSpells", "SummonerSpells");

                if (Mark.IsVaild() || Player.Spells.Any(s => s.Name.Equals("SummonerPoroThrow")))
                {
                    balls.Init();
                }
                if (Barrier.IsVaild())
                {
                    Borrier.Init();
                }
                if (Clarity.IsVaild())
                {
                    Clarty.Init();
                }
                if (Cleanse.IsVaild())
                {
                    Clanse.Init();
                }
                if (Exhaust.IsVaild())
                {
                    Exhust.Init();
                }
                if (Flash.IsVaild())
                {
                    Flish.Init();
                }
                if (Heal.IsVaild())
                {
                    Heel.Init();
                }
                if (Ignite.IsVaild())
                {
                    Ignoite.Init();
                }
                if (Smite.IsVaild())
                {
                    Smote.Init();
                }

                if (menu.LinkedValues.Count == 0)
                {
                    menu.AddGroupLabel("Your SummonerSpells are not Supported");
                }
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
