using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Events;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Borrier
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Barrier Settings");
                Summs.menu.CreateCheckBox("Barrier", "Use Barrier");
                Summs.menu.CreateSlider("Barrierhp", "Use For " + Player.Instance.Name() + " Under {0}% HP", TargetSelector.GetPriority(Player.Instance) * 5);
                OnInComingDamage.OnIncomingDamage += OnInComingDamage_OnIncomingDamage;
                Summs.menu.AddSeparator(5);
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Borrier.Init", ex, Logger.LogLevel.Error);
            }
        }

        public static void OnInComingDamage_OnIncomingDamage(OnInComingDamage.InComingDamageEventArgs args)
        {
            if (!Barrier.IsReady() || !args.Target.IsMe || !args.Target.IsKillable() || !EntityManager.Heroes.Enemies.Any(e => e.IsValid && !e.IsDead && e.IsInRange(args.Target, 1250)) && args.Target.Health > args.InComingDamage)
                return;

            if (!Summs.menu.CheckBoxValue("Barrier"))
                return;

            if (Summs.menu.SliderValue("Barrierhp") >= args.Target.HealthPercent || args.InComingDamage >= args.Target.TotalShieldHealth())
            {
                Barrier.Cast();
            }
        }
    }
}
