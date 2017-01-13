namespace KappAzir
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Utility;

    internal static class Menus
    {
        public static Menu Menuini, Auto, JumperMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillstealMenu, DrawMenu, ColorMenu;

        public static void Execute()
        {
            Menuini = MainMenu.AddMenu("KappAzir", "KappAzir");
            Auto = Menuini.AddSubMenu("Auto Settings");
            JumperMenu = Menuini.AddSubMenu("Jumper Settings");
            ComboMenu = Menuini.AddSubMenu("Combo Settings");
            HarassMenu = Menuini.AddSubMenu("Harass Settings");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
            KillstealMenu = Menuini.AddSubMenu("KillSteal Settings");
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            ColorMenu = Menuini.AddSubMenu("ColorPicker");

            Auto.AddGroupLabel("Settings");
            Auto.Add("gap", new CheckBox("Anti-GapCloser"));
            Auto.Add("int", new CheckBox("Interrupter"));
            Auto.Add("danger", new ComboBox("Interrupter DangerLevel", 1, "High", "Medium", "Low"));
            Auto.AddGroupLabel("Turret Settings");
            Auto.Add("tower", new CheckBox("Create Turrets"));
            Auto.Add("Tenemy", new Slider("Create Turret If [{0}] Enemies Near", 3, 1, 6));
            Auto.AddGroupLabel("Anti GapCloser Spells");
            foreach (var spell in
                from spell in Gapcloser.GapCloserList
                from enemy in EntityManager.Heroes.Enemies.Where(enemy => spell.ChampName == enemy.ChampionName)
                select spell)
            {
                Auto.Add(spell.SpellName, new CheckBox(spell.ChampName + " " + spell.SpellSlot));
            }

            if (EntityManager.Heroes.Enemies.Any(e => e.Hero == Champion.Rengar))
            {
                Auto.Add("rengar", new CheckBox("Rengar Leap"));
            }

            JumperMenu.Add("jump", new KeyBind("WEQ Flee Key", false, KeyBind.BindTypes.HoldActive, 'A'));
            JumperMenu.Add("normal", new KeyBind("Normal Insec Key", false, KeyBind.BindTypes.HoldActive, 'S'));
            JumperMenu.Add("new", new KeyBind("New Insec Key", false, KeyBind.BindTypes.HoldActive, 'Z'));
            JumperMenu.Add("flash", new CheckBox("Use Flash for Possible AoE"));
            JumperMenu.Add("delay", new Slider("Delay EQ", 200, 0, 500));
            JumperMenu.Add("range", new Slider("Check for soldiers Range", 800, 0, 1000));

            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, 32));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Q Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("WQ", new CheckBox("Use W > Q"));
            ComboMenu.Add("Qaoe", new CheckBox("Use Q Aoe", false));
            ComboMenu.Add("QS", new Slider("Soldiers To Use Q", 1, 1, 3));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("W Settings");
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("Wsave", new CheckBox("Save 1 W Stack", false));
            ComboMenu.Add("WS", new Slider("Soldier Limit To Create", 3, 1, 3));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("E Settings");
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("Ekill", new CheckBox("E Killable Enemy Only"));
            ComboMenu.Add("Edive", new CheckBox("E Dive Turrets", false));
            ComboMenu.Add("EHP", new Slider("Only E if my HP is more than [{0}%]", 50));
            ComboMenu.Add("Esafe", new Slider("Dont E Into [{0}] Enemies", 3, 1, 6));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("R Settings");
            ComboMenu.Add("R", new CheckBox("Use R"));
            ComboMenu.Add("Rkill", new CheckBox("R Finisher"));
            ComboMenu.Add("insec", new CheckBox("Try to insec in Combo"));
            ComboMenu.Add("Raoe", new Slider("R AoE Hit [{0}] Enemies", 3, 1, 6));
            ComboMenu.Add("Rsave", new CheckBox("R Save Self"));
            ComboMenu.Add("RHP", new Slider("Push Enemy If my health is less than [{0}%]", 35));

            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("key", new KeyBind("Harass Key", false, KeyBind.BindTypes.HoldActive, 'C'));
            HarassMenu.Add("toggle", new KeyBind("Auto Harass Key", false, KeyBind.BindTypes.PressToggle, 'H'));
            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("Q Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("WQ", new CheckBox("Use W > Q"));
            HarassMenu.Add("QS", new Slider("Soldiers To Use Q", 1, 1, 3));
            HarassMenu.Add("Qmana", new Slider("Stop using Q if Mana < [{0}%]", 65));
            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("W Settings");
            HarassMenu.Add("W", new CheckBox("Use W"));
            HarassMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
            HarassMenu.Add("WS", new Slider("Soldier Limit To Create", 3, 1, 3));
            HarassMenu.Add("Wmana", new Slider("Stop using W if Mana < [{0}%]", 65));
            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("E Settings");
            HarassMenu.Add("E", new CheckBox("Use E"));
            HarassMenu.Add("Edive", new CheckBox("E Dive Turrets", false));
            HarassMenu.Add("EHP", new Slider("Only E if my HP is more than [{0}%]", 50));
            HarassMenu.Add("Esafe", new Slider("Dont E Into [{0}] Enemies", 3, 1, 6));
            HarassMenu.Add("Emana", new Slider("Stop using E if Mana < [{0}%]", 65));

            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("key", new KeyBind("LaneClear Key", false, KeyBind.BindTypes.HoldActive, 'V'));
            LaneClearMenu.Add("Q", new CheckBox("Use Q"));
            LaneClearMenu.Add("Qmana", new Slider("Stop using Q if Mana < [{0}%]", 65));
            LaneClearMenu.Add("W", new CheckBox("Use W"));
            LaneClearMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
            LaneClearMenu.Add("Wmana", new Slider("Stop using W if Mana < [{0}%]", 65));

            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("key", new KeyBind("JungleClear Key", false, KeyBind.BindTypes.HoldActive, 'V'));
            JungleClearMenu.Add("Q", new CheckBox("Use Q"));
            JungleClearMenu.Add("Qmana", new Slider("Stop using Q if Mana < [{0}%]", 65));
            JungleClearMenu.Add("W", new CheckBox("Use W"));
            JungleClearMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
            JungleClearMenu.Add("Wmana", new Slider("Stop using W if Mana < [{0}%]", 65));

            KillstealMenu.AddGroupLabel("KillSteal Settings");
            KillstealMenu.Add("Q", new CheckBox("Use Q"));
            KillstealMenu.Add("E", new CheckBox("Use E"));
            KillstealMenu.Add("R", new CheckBox("Use R"));

            foreach (var spell in Azir.SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
            }

            DrawMenu.Add("insec", new CheckBox("Draw Insec Helpers"));
        }

        public static int combobox(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        public static int slider(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        public static bool checkbox(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        public static bool keybind(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        public static System.Drawing.Color Color(this Menu m, string id)
        {
            return m[id].Cast<ColorPicker>().CurrentValue;
        }
    }
}