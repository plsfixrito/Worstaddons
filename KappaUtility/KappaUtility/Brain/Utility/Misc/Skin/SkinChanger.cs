using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Misc.Skin
{
    internal class SkinChanger
    {
        private static Menu menu;

        internal static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("Skin Changer");
                var PlayerName = Player.Instance.BaseSkinName;

                menu.AddGroupLabel("Skin Changer: " + PlayerName);
                menu.CreateCheckBox("enable" + PlayerName, "Enable", false);
                menu.CreateSlider("skinid" + PlayerName, "SkinID {0}", 0, 0, 15).OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        if (menu.CheckBoxValue("enable" + PlayerName))
                            Player.SetSkinId(args.NewValue);
                    };
                menu.AddLabel("Contains Lulu W Fix");
                if (menu.CheckBoxValue("enable" + PlayerName) && Player.Instance.SkinId < menu.SliderValue("skinid" + PlayerName))
                    Player.SetSkinId(menu.SliderValue("skinid" + PlayerName));

                var containslulu = EntityManager.Heroes.Enemies.All(h => h.Hero != Champion.Lulu);

                Game.OnTick += delegate
                    {
                        var enable = menu.CheckBoxValue("enable" + PlayerName);
                        if (containslulu && Player.Instance.Model.ToLower().Contains("lulu") && PlayerName != "Lulu")
                        {
                            Player.SetModel(PlayerName);
                            if (enable && Player.Instance.SkinId != menu.SliderValue("skinid" + PlayerName))
                                Player.SetSkinId(menu.SliderValue("skinid" + PlayerName));
                        }
                        if(!enable)
                            return;
                        if(Player.Instance.SkinId != menu.SliderValue("skinid" + PlayerName))
                            Player.SetSkinId(menu.SliderValue("skinid" + PlayerName));
                    };
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Misc.Skin.SkinChanger.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
