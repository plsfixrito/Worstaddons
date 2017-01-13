using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtility.Common.Misc;
using static KappaUtility.Brain.Utility.Misc.AutoLvlup.Sequence;

namespace KappaUtility.Brain.Utility.Misc.AutoLvlup
{
    internal class Autolvlup
    {
        private static readonly List<int> CustomSet = new List<int>();

        private static readonly List<int[]> sets = new List<int[]> { QWER, QEWR, WQER, WEQR, EQWR, EWQR };
        private static int[] LevelSet = { };

        private static Menu menu;

        internal static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("AutoLvlUp");

                menu.AddGroupLabel("AutoLvlUp - " + Player.Instance.ChampionName);
                menu.CreateCheckBox("Enable" + Player.Instance.ChampionName, "Enable AutoLvlUp", false);
                menu.CreateCheckBox("autoR", "Always Auto Level R", false);

                var min = menu.CreateSlider("min", "Min Delay [{0}]ms", 500, 0, 2500);
                var max = menu.CreateSlider("max", "Max Delay [{0}]ms", 2500, min.CurrentValue, 5000);

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

                var levelmode = menu.Add("lvlmode" + Player.Instance.ChampionName, new ComboBox("Level Mode", 0, "Pre-made", "Custom"));
                var mode = menu.Add(
                    "mode" + Player.Instance.ChampionName,
                    new ComboBox("Level Set", 0, "Q > W > E > R", "Q > E > W > R", "W > Q > E > R", "W > E > Q > R", "E > Q > W > R", "E > W > Q > R"));

                menu.AddLabel("Custom LevelSet = 1 = Q | 2 = W | 3 = E | 4 = R");
                CreateCustomLevelMenu();

                mode.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args) { LevelSet = sets[args.NewValue]; };
                LevelSet = sets[mode.CurrentValue];

                menu.AddSeparator(0);

                levelmode.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        mode.IsVisible = args.NewValue == 0;
                        ShowCustomLevelMenu(args.NewValue == 1);
                    };

                mode.IsVisible = levelmode.CurrentValue == 0;
                ShowCustomLevelMenu(levelmode.CurrentValue == 1);

                Game.OnTick += Game_OnTick;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Misc.AutoLvlup.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static bool Leveling;

        private static void CreateCustomLevelMenu()
        {
            for (var i = 0; i < 18; i++)
            {
                menu.CreateSlider(i + Player.Instance.ChampionName, "Level: " + (i + 1), 1, 1, 4);
                CustomSet.Add(menu.SliderValue(i + Player.Instance.ChampionName));
            }
        }

        private static void ShowCustomLevelMenu(bool Show)
        {
            for (var i = 0; i < 18; i++)
            {
                menu.Get<Slider>(i + Player.Instance.ChampionName).IsVisible = Show;
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (menu.CheckBoxValue("autoR") && Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.R))
                Player.LevelSpell(SpellSlot.R);

            if (!menu.CheckBoxValue("Enable" + Player.Instance.ChampionName))
                return;
            
            if (menu.ComboBoxValue("lvlmode" + Player.Instance.ChampionName) == 1)
            {
                for (var i = 0; i < 18; i++)
                {
                    CustomSet[i] = menu.SliderValue(i + Player.Instance.ChampionName);
                }

                LevelSet = CustomSet.ToArray();
            }

            LevelSpells();
        }

        private static void LevelSpells()
        {
            var qL = Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level;
            var wL = Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level;
            var eL = Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level;
            var rL = Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qL + wL + eL + rL < Player.Instance.Level && !Leveling && Player.Instance.SpellTrainingPoints > 0)
            {
                var level = new[] { 0, 0, 0, 0 };
                Leveling = true;
                for (var i = 0; i < Player.Instance.Level; i++)
                {
                    if (LevelSet != null)
                    {
                        level[LevelSet[i] - 1] = level[LevelSet[i] - 1] + 1;
                    }
                }

                if (rL < level[3] && Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.R))
                {
                    LevelSpell(SpellSlot.R);
                }
                if (qL < level[0] && Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.Q))
                {
                    LevelSpell(SpellSlot.Q);
                }
                if (wL < level[1] && Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.W))
                {
                    LevelSpell(SpellSlot.W);
                }
                if (eL < level[2] && Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.E))
                {
                    LevelSpell(SpellSlot.E);
                }
            }
        }

        private static void LevelSpell(SpellSlot slot)
        {
            var min = menu.SliderValue("min");
            var max = menu.SliderValue("max");
            var rnd = new Random().Next(min, max) + Game.Ping;
            Core.DelayAction(
                () =>
                    {
                        Player.LevelSpell(slot);
                        Leveling = false;
                    },
                rnd);
        }
    }
}
