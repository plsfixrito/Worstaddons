using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Events;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Heel
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Heal Settings");
                Summs.menu.CreateCheckBox("Heal", "Use Heal On Self");
                Summs.menu.CreateCheckBox("execute", "Use Before Execute Only");
                Summs.menu.CreateCheckBox("Healally", "Use Heal On Allies");
                Summs.menu.AddGroupLabel("Allies to use Heal On: ");
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    Summs.menu.CreateCheckBox("heal" + ally.Name(), "Heal " + ally.Name(), TargetSelector.GetPriority(ally) > 1);
                    Summs.menu.CreateSlider("hp" + ally.Name(), "Heal " + ally.Name() + " Under {0}% HP", TargetSelector.GetPriority(ally) * 10);
                }

                Summs.menu.AddSeparator(5);
                OnInComingDamage.OnIncomingDamage += OnInComingDamage_OnIncomingDamage;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Heel.Init", ex, Logger.LogLevel.Error);
            }
        }

        internal static void OnInComingDamage_OnIncomingDamage(OnInComingDamage.InComingDamageEventArgs args)
        {
            if (!Heal.IsReady() || !args.Target.IsKillable(800) || !EntityManager.Heroes.Enemies.Any(e => e.IsValid && !e.IsDead && e.IsInRange(args.Target, 1250)) && args.Target.Health > args.InComingDamage)
                return;

            if (!Summs.menu.CheckBoxValue("heal" + args.Target.Name()))
                return;

            if (Summs.menu.CheckBoxValue("execute"))
            {
                if (args.Target.IsMe || !args.Target.IsMe && Summs.menu.CheckBoxValue("Healally"))
                {
                    if (args.InComingDamage >= args.Target.Health && args.Target.IsInRange(Player.Instance, 800))
                        Heal.Cast();
                }
                return;
            }
            
            if (((Summs.menu.CheckBoxValue("Healally") && !args.Target.IsMe && args.Target.IsAlly) || Summs.menu.CheckBoxValue("Heal") && args.Target.IsMe)
                && (Summs.menu.SliderValue("hp" + args.Target.Name()) >= args.Target.HealthPercent || args.InComingDamage >= args.Target.TotalShieldHealth()))
            {
                Heal.Cast();
            }
        }
    }
}
