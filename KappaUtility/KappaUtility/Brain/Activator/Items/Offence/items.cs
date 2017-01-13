using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;
using KappaUtility.Common.Databases;
namespace KappaUtility.Brain.Activator.Items.Offence
{
    internal class items
    {
        private static Menu menu;

        internal static void Init()
        {
            menu = Activator.Load.MenuIni.AddSubMenu("Offence");

            foreach (var item in ItemsDatabase.AfterAttackItems)
            {
                menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
            }

            foreach (var item in ItemsDatabase.DamageItems)
            {
                menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
            }

            menu.AddSeparator(0);
            foreach (var item in ItemsDatabase.LifeStealItems)
            {
                menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
                menu.CreateSlider(item.ItemInfo.Name + "my", "Use " + item.ItemInfo.Name + " Under {0}% HP", 75);
                menu.CreateSlider(item.ItemInfo.Name + "ene", "Use " + item.ItemInfo.Name + " For Enemy Under {0}% HP", 75);
            }

            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if(!ItemsDatabase.DamageItems.Any(i => i.ItemReady(menu)) && !ItemsDatabase.LifeStealItems.Any(i => i.ItemReady(menu)))
                return;

            foreach (var item in ItemsDatabase.DamageItems.Where(i => i.ItemReady(menu)))
            {
                var target = TargetSelector.GetTarget(item.Range, DamageType.Magical);
                if (target.IsKillable())
                {
                    item.Cast(target.PrediectPosition(100 + Game.Ping));
                }
            }

            foreach (var item in ItemsDatabase.LifeStealItems.Where(i => i.ItemReady(menu)))
            {
                var target =
                    EntityManager.Heroes.Enemies.OrderByDescending(o => o.MaxHealth).FirstOrDefault(e => e.IsKillable(item.Range) && menu.SliderValue(item.ItemInfo.Name + "ene") > e.HealthPercent);
                if (target.IsKillable() && menu.SliderValue(item.ItemInfo.Name + "my") > Player.Instance.HealthPercent)
                {
                    item.Cast(target);
                    return;
                }
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target is AIHeroClient && target.IsValidTarget())
            {
                foreach (var item in ItemsDatabase.AfterAttackItems.Where(i => i.ItemReady(menu)))
                {
                    item.Cast();
                    return;
                }
            }
        }
    }
}
