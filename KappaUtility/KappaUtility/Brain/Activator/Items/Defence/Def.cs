using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Activator.Items.Defence
{
    internal class Def
    {
        private static Menu menu;

        internal static void Init()
        {
            try
            {
                menu = Activator.Load.MenuIni.AddSubMenu("Defence");

                menu.AddGroupLabel("Self Items Settings");
                foreach (var item in Common.Databases.ItemsDatabase.SelfSaverItems)
                {
                    menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
                    menu.CreateSlider(item.ItemInfo.Name + "hp", "Use " + item.ItemInfo.Name + " On {0}% HP", TargetSelector.GetPriority(Player.Instance) * 10);
                }

                menu.AddSeparator(0);
                menu.AddGroupLabel("Ally Items Settings");
                foreach (var item in Common.Databases.ItemsDatabase.AllySaverItems)
                {
                    menu.AddGroupLabel(item.ItemInfo.Name);
                    foreach (var ally in EntityManager.Heroes.Allies)
                    {
                        menu.CreateCheckBox(ally.Name() + item.ItemInfo.Name, "Use " + item.ItemInfo.Name + " For " + ally.Name());
                    }
                    menu.CreateSlider(item.ItemInfo.Name + "hp", "Use " + item.ItemInfo.Name + " On {0}% HP", 45);
                }
                Common.Events.OnInComingDamage.OnIncomingDamage += OnInComingDamage_OnIncomingDamage;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Items.Defence.Def.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void OnInComingDamage_OnIncomingDamage(Common.Events.OnInComingDamage.InComingDamageEventArgs args)
        {
            if (args.Target == null || !args.Target.IsKillable() || !args.Target.IsAlly || !EntityManager.Heroes.Enemies.Any(e => e.IsValid && !e.IsDead && e.IsInRange(args.Target, 2000)) && args.Target.Health > args.InComingDamage)
                return;
            
            if (args.Target.IsMe)
            {
                foreach (var item in Common.Databases.ItemsDatabase.SelfSaverItems.Where(i => i.ItemReady(menu)))
                {
                    var isinrage = item.Range.Equals(0f) || item.IsInRange(args.Target);
                    if (menu.SliderValue(item.ItemInfo.Name + "hp") >= args.Target.HealthPercent && isinrage)
                    {
                        item.Cast();
                        break;
                    }
                }
            }
            foreach (var item in Common.Databases.ItemsDatabase.AllySaverItems.Where(i => menu.CheckBoxValue(args.Target.Name() + i.ItemInfo.Name)))
            {
                var isinrage = item.Range.Equals(0f) || item.IsInRange(args.Target);
                if (menu.CheckBoxValue(args.Target.Name() + item.ItemInfo.Name) && menu.SliderValue(item.ItemInfo.Name + "hp") >= args.Target.HealthPercent && isinrage)
                {
                    item.Cast(args.Target);
                    break;
                }
            }
        }
    }
}
