using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtility.Common.Misc;
using SharpDX;

namespace KappaUtility.Brain.Utility.Misc.Lantern
{
    internal class PickLantern
    {
        private static Menu menu;

        private static Obj_AI_Base ThreshLantern
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(l => l.IsValid && !l.IsDead && l.IsAlly && l.Name.Equals("ThreshLantern"));
            }
        }

        private static AIHeroClient Thresh
        {
            get
            {
                return EntityManager.Heroes.Allies.OrderBy(h => h.Distance(Player.Instance)).FirstOrDefault(h => h.Hero == Champion.Thresh && !h.IsMe && h.IsValidTarget());
            }
        }

        internal static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("ThreshLantern");
                if (!EntityManager.Heroes.Allies.Any(a => !a.IsMe && a.Hero == Champion.Thresh))
                {
                    menu.AddGroupLabel("There is no Thresh in your Team");
                    return;
                }

                menu.AddGroupLabel("Auto ThreshLantern");
                menu.CreateCheckBox("enable", "Enable");
                menu.CreateCheckBox("auto", "Auto Pick Lantern");
                menu.CreateCheckBox("safe", "Enable Safety Checks");
                menu.CreateCheckBox("orb", "Try to walk to Lantern");
                menu.CreateSlider("hp", "Pick Thresh Lantern Under {0}% HP", TargetSelector.GetPriority(Player.Instance) * 15);
                menu.CreateKeyBind("key", "Pick Lantern HotKey", false, KeyBind.BindTypes.HoldActive, 'A');

                Game.OnTick += Game_OnTick;
                Orbwalker.OverrideOrbwalkPosition = OverrideOrbwalkPosition;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Misc.Lantern.PickLantern.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static Vector3? OverrideOrbwalkPosition()
        {
            if (ThreshLantern == null || Thresh == null || !menu.CheckBoxValue("orb") || !menu.CheckBoxValue("enable") || ThreshLantern.Distance(Player.Instance) <= 400)
                return null;

            if (menu.KeyBindValue("key") || menu.SliderValue("hp") >= Player.Instance.HealthPercent && menu.CheckBoxValue("enable"))
            {
                if (menu.CheckBoxValue("safe") && Thresh.CountAlliesInRange(1000) >= Thresh.CountEnemiesInRange(1000) || !menu.CheckBoxValue("safe"))
                    return ThreshLantern.ServerPosition;
            }
            return null;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!menu.CheckBoxValue("enable") || ThreshLantern == null || Thresh == null)
                return;

            if (Player.Instance.IsInRange(ThreshLantern, 400))
            {
                if (menu.KeyBindValue("key") || menu.SliderValue("hp") >= Player.Instance.HealthPercent && menu.CheckBoxValue("auto"))
                {
                    if (menu.CheckBoxValue("safe") && Thresh.CountAlliesInRange(1000) >= Thresh.CountEnemiesInRange(1000) || !menu.CheckBoxValue("safe"))
                        Player.UseObject(ThreshLantern);
                }
            }
        }
    }
}
