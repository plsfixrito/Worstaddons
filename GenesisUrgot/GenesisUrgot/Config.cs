using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace GenesisUrgot
{
    // I can't really help you with my layout of a good config class
    // since everyone does it the way they like it most, go checkout my
    // config classes I make on my GitHub if you wanna take over the
    // complex way that I use
    public static class Config
    {
        private const string MenuName = "GenesisUrgot";

        private static readonly Menu Menu;

        static Config()
        {
            // Initialize the menu
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Welcome to GenesisUrgot! The true Urgod!");
            // Initialize the modes
            Modes.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            private static readonly Menu ComboMenu;
            private static readonly Menu ShieldMenu;
            private static readonly Menu InterruptMenu;
            private static readonly Menu HarassMenu;
            private static readonly Menu MiscMenu;
            private static readonly Menu LCMenu;

            static Modes()
            {
                // Initialize the menu
                ComboMenu = Menu.AddSubMenu("Combo");
                Combo.Initialize();
                InterruptMenu = Menu.AddSubMenu("Interrupt");
                Interrupt.Initialize();
                ShieldMenu = Menu.AddSubMenu("Shield");
                ShieldManager.Initialize();
                HarassMenu = Menu.AddSubMenu("Harass");
                Harass.Initialize();
                LCMenu = Menu.AddSubMenu("LastHit");
                LC.Initialize();
                MiscMenu = Menu.AddSubMenu("Misc");
                PermaActive.Initialize();

                // Initialize all modes
                // Combo

                Menu.AddSeparator();

                // Harass
            }

            public static void Initialize()
            {
            }

            public static class PermaActive
            {
                private static readonly CheckBox _stackTearQ;
                private static readonly CheckBox _useR;

                public static bool StackTearQ
                {
                    get
                    {
                        return _stackTearQ.CurrentValue;
                    }
                }

                public static bool UseR
                {
                    get
                    {
                        return _useR.CurrentValue;
                    }
                }

                static PermaActive()
                {
                    MiscMenu.AddLabel("AlwaysOn");
                    _useR = MiscMenu.Add("AlwaysUseR", new CheckBox("Use R to hold champions under turret"));

                    _stackTearQ = MiscMenu.Add("AlwaysUseWFlee", new CheckBox("Use Q to stack tear"));
                }

                public static void Initialize()
                {
                }
            }

            public static class ShieldManager
            {
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useHumanizer;

                public static bool UseW
                {
                    get
                    {
                        return _useW.CurrentValue;
                    }
                }

                public static bool UseHumanizer
                {
                    get
                    {
                        return _useHumanizer.CurrentValue;
                    }
                }

                static ShieldManager()
                {
                    // Initialize the menu values
                    ShieldMenu.AddGroupLabel("Shield Manager");
                    _useW = ShieldMenu.Add("UseW", new CheckBox("Use W"));
                    _useHumanizer = ShieldMenu.Add("Humanizer", new CheckBox("Use Humanizer", false));
                }

                public static void Initialize()
                {
                }
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useR;

                public static bool UseQ
                {
                    get
                    {
                        return _useQ.CurrentValue;
                    }
                }

                public static bool UseW
                {
                    get
                    {
                        return _useW.CurrentValue;
                    }
                }

                public static bool UseE
                {
                    get
                    {
                        return _useE.CurrentValue;
                    }
                }

                public static bool UseR
                {
                    get
                    {
                        return _useR.CurrentValue;
                    }
                }

                static Combo()
                {
                    // Initialize the menu values
                    ComboMenu.AddGroupLabel("Combo");
                    _useQ = ComboMenu.Add("comboUseQ", new CheckBox("Use Q"));
                    _useW = ComboMenu.Add("comboUseW", new CheckBox("Use W"));
                    _useE = ComboMenu.Add("comboUseE", new CheckBox("Use E"));
                    _useR = ComboMenu.Add("comboUseR", new CheckBox("Use R"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Interrupt
            {
                private static readonly CheckBox _useR;

                public static bool UseR
                {
                    get
                    {
                        return _useR.CurrentValue;
                    }
                }

                static Interrupt()
                {
                    // Initialize the menu values
                    InterruptMenu.AddGroupLabel("Interrupt");
                    _useR = InterruptMenu.Add("InterruptUseQ", new CheckBox("Use R"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get
                    {
                        return HarassMenu["harassUseQ"].Cast<CheckBox>().CurrentValue;
                    }
                }
                public static bool UseE
                {
                    get
                    {
                        return HarassMenu["harassUseE"].Cast<CheckBox>().CurrentValue;
                    }
                }

                static Harass()
                {
                    HarassMenu.AddGroupLabel("Harass");
                    HarassMenu.Add("harassUseQ", new CheckBox("Use Q"));
                    HarassMenu.Add("harassUseE", new CheckBox("Use E"));
                }

                public static void Initialize()
                {
                }
            }

            public static class LC
            {
                private static readonly CheckBox _useQ;

                public static bool UseQ
                {
                    get
                    {
                        return _useQ.CurrentValue;
                    }
                }
                public static int QMana
                {
                    get
                    {
                        return LCMenu["QMana"].Cast<Slider>().CurrentValue;
                    }
                }

                static LC()
                {
                    // Initialize the menu values
                    LCMenu.AddGroupLabel("LastHit");
                    _useQ = LCMenu.Add("UseQ", new CheckBox("Use Q"));
                    LCMenu.Add("QMana", new Slider("Q until you reach Mana% ({0}%)", 40));
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}
