using System;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Exhust
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Exhaust Settings");
                Summs.menu.CreateCheckBox("Exhaust", "Use Exhaust");
                Summs.menu.AddGroupLabel("Allies to use Exhaust For");
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    Summs.menu.CreateCheckBox("Exhaust" + ally.Name(), "Use For " + ally.Name());
                    Summs.menu.CreateSlider("Exhausthp" + ally.Name(), "Use For " + ally.Name() + " On {0}% HP", TargetSelector.GetPriority(ally) * 10);
                }
                Summs.menu.AddSeparator(0);
                Summs.menu.AddGroupLabel("Enemies to use Exhaust On");
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    Summs.menu.CreateCheckBox("Exhaust" + enemy.Name(), "Use On " + enemy.Name());
                    Summs.menu.CreateSlider("Exhausthp" + enemy.Name(), "Use On " + enemy.Name() + " On {0}% HP", TargetSelector.GetPriority(enemy) * 10);
                }
                Common.Events.OnInComingDamage.OnIncomingDamage += OnInComingDamage_OnIncomingDamage;
                Summs.menu.AddSeparator(5);
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Exhust.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void OnInComingDamage_OnIncomingDamage(Common.Events.OnInComingDamage.InComingDamageEventArgs args)
        {
            if (!Summs.menu.CheckBoxValue("Exhaust") || !(args.Sender is AIHeroClient) || !Exhaust.IsReady() || args.Sender.Distance(Player.Instance) > 750
                || !Summs.menu.CheckBoxValue("Exhaust" + args.Target.Name()) || !args.Target.IsKillable())
                return;

            if (Summs.menu.CheckBoxValue("Exhaust" + args.Sender.Name()) && Summs.menu.SliderValue("Exhausthp" + args.Sender.Name()) >= args.Sender.HealthPercent)
            {
                Exhaust.Cast(args.Sender);
            }
        }
    }
}
