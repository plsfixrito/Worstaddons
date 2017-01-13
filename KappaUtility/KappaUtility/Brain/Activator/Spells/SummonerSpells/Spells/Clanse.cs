using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Clanse
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Cleanse Settings");
                Summs.menu.CreateCheckBox("Cleanse", "Use Cleanse");
                Summs.menu.CreateSlider("CleanseHP", "Use Cleanse under {0}% HP", TargetSelector.GetPriority(Player.Instance) * 10);
                var min = Summs.menu.CreateSlider("CleanseMin", "Cleanse Min Delay {0}%", 100, 0, 400);
                var max = Summs.menu.CreateSlider("CleanseMax", "Cleanse Max Delay {0}%", 500, 0, 1500);
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
                Summs.menu.AddGroupLabel("Buffs to Cleanse");
                foreach (var buff in Common.Misc.Extensions.CCbuffs)
                {
                    Summs.menu.CreateCheckBox(buff.ToString(), "Cleanse " + buff);
                }
                Summs.menu.AddSeparator(5);
                Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Clanse.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!Common.Misc.Extensions.CCbuffs.Contains(args.Buff.Type) || !Summs.menu.CheckBoxValue("Cleanse"))
                return;

            if (!sender.IsMe || !Cleanse.IsReady() || !Summs.menu.CheckBoxValue(args.Buff.Type.ToString()) || Player.Instance.HealthPercent > Summs.menu.SliderValue("CleanseHP"))
                return;

            Core.DelayAction(() => Cleanse.Cast(), new Random().Next(Summs.menu.SliderValue("CleanseMin"), Summs.menu.SliderValue("CleanseMax")));
        }
    }
}
