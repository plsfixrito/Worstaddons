using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtility.Common.Databases;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Activator.Items.Defence
{
    internal class Qss
    {
        private static Menu menu;

        internal static void Init()
        {
            try
            {
                menu = Activator.Load.MenuIni.AddSubMenu("Cleanse");

                menu.AddGroupLabel("Cleanse Settings");
                menu.CreateCheckBox("enable", "Enable Cleanse");
                foreach (var item in ItemsDatabase.SelfQssItems)
                {
                    menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
                }
                foreach (var item in ItemsDatabase.AllyQssItems)
                {
                    menu.CreateCheckBox(item.ItemInfo.Name, "Use " + item.ItemInfo.Name);
                }

                menu.AddGroupLabel("Allies Settings");
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    menu.CreateCheckBox(ally.Name(), "Use For " + ally.Name());
                    menu.CreateSlider(ally.Name() + "hp", "Use For " + ally.Name() + " Under {0}% HP", TargetSelector.GetPriority(ally) * 10);
                }

                menu.AddGroupLabel("Buffs to Qss");
                foreach (var buff in Common.Misc.Extensions.CCbuffs)
                {
                    menu.CreateCheckBox(buff.ToString(), "Qss " + buff);
                }

                menu.AddGroupLabel("Humanizer Settings");
                var min = menu.CreateSlider("QssMin", "Qss Min Delay {0}", 100, 0, 400);
                var max = menu.CreateSlider("QssMax", "Qss Max Delay {0}", 500, 0, 1500);
                min.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        if (args.NewValue >= max.CurrentValue)
                        {
                            max.MinValue = args.NewValue + 100;
                        }
                    };
                max.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        if (args.NewValue >= min.MaxValue)
                        {
                            min.MaxValue = args.NewValue - 100;
                        }
                    };

                Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Items.Defence.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (!menu.CheckBoxValue("enable") || caster == null || !Common.Misc.Extensions.CCbuffs.Contains(args.Buff.Type))
                return;
            
            if (!caster.IsAlly || caster.Distance(Player.Instance) > 1000 || !menu.CheckBoxValue(args.Buff.Type.ToString())
                || !menu.CheckBoxValue(caster.Name()) || caster.HealthPercent > menu.SliderValue(caster.Name() + "hp"))
                return;

            var delay = new Random().Next(menu.SliderValue("QssMin"), menu.SliderValue("QssMax"));
            if (caster.IsMe)
            {
                foreach (var item in ItemsDatabase.SelfQssItems.Where(i => i.ItemReady(menu)))
                {
                    Core.DelayAction(() => item.Cast(), delay);
                    return;
                }
                foreach (var item in ItemsDatabase.AllyQssItems.Where(i => i.ItemReady(menu)))
                {
                    Core.DelayAction(() => item.Cast(caster), delay);
                    return;
                }
            }
            else
                foreach (var item in ItemsDatabase.AllyQssItems.Where(i => i.ItemReady(menu)))
                {
                    Core.DelayAction(() => item.Cast(caster), delay);
                    return;
                }
        }
    }
}
