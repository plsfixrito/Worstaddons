using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Activator.Items.Defence
{
    internal class Potions
    {
        private static readonly List<string> PotBuffs = new List<string> { "Health Potion", "ItemCrystalFlask", "ItemDarkCrystalFlask", "ItemMiniRegenPotion" };
        private static Menu menu;

        internal static void Init()
        {
            try
            {
                menu = Activator.Load.MenuIni.AddSubMenu("Potions");

                menu.AddGroupLabel("Potions Settings");
                foreach (var item in Common.Databases.ItemsDatabase.Potions)
                {
                    menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
                    menu.CreateSlider(item.ItemInfo.Name + "hp", "Use " + item.ItemInfo.Name + " On {0}% HP", TargetSelector.GetPriority(Player.Instance) * 15);
                    menu.AddSeparator(0);
                }
                Common.Events.OnInComingDamage.OnIncomingDamage += OnInComingDamage_OnIncomingDamage;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Items.Defence.Potions.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void OnInComingDamage_OnIncomingDamage(Common.Events.OnInComingDamage.InComingDamageEventArgs args)
        {
            if (!args.Target.IsMe)
                return;

            foreach (var item in Common.Databases.ItemsDatabase.Potions.Where(i => i.ItemReady(menu)))
            {
                if (!Player.Instance.Buffs.Any(a => PotBuffs.Any(b => a.DisplayName.Equals(b))) && Player.Instance.HealthPercent <= menu.SliderValue(item.ItemInfo.Name + "hp"))
                {
                    item.Cast();
                    return;
                }
            }
        }
    }
}
