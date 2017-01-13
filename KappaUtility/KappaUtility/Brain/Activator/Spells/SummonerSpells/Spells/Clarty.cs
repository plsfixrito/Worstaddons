using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Clarty
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Clarity Settings");
                Summs.menu.CreateCheckBox("Clarity", "Use Clarity");
                Summs.menu.AddGroupLabel("Use Clarity For");
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    Summs.menu.CreateCheckBox("clarity" + ally.Name(), "Clarity " + ally.Name(), TargetSelector.GetPriority(ally) > 1);
                    Summs.menu.CreateSlider("mp" + ally.Name(), "Clarity " + ally.Name() + " Under {0}% MP", TargetSelector.GetPriority(ally) * 10);
                }
                Game.OnTick += Game_OnTick;
                Summs.menu.AddSeparator(5);
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Clarty.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!Summs.menu.CheckBoxValue("Clarity") || !Clarity.IsReady())
                return;

            if (EntityManager.Heroes.Allies.Any(a => a.Distance(Player.Instance) <= 750 && Summs.menu.CheckBoxValue("clarity" + a.Name()) && a.ManaPercent <= Summs.menu.SliderValue("mp" + a.Name())))
            {
                Clarity.Cast();
            }
        }
    }
}
