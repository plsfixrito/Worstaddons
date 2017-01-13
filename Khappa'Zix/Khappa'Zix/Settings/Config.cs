using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Khappa_Zix.Common;

namespace Khappa_Zix.Settings
{
    internal class Config
    {
        public static Menu MenuIni;

        public Config()
        {
            MenuIni = MainMenu.AddMenu("Khappa'Zix", "Khappa'Zix");
            
            var combo = new Combo();
            var harass = new Harass();
            var laneClear = new LaneClear();
            var jungleClear = new JungleClear();
            var killSteal = new KillSteal();
            var drawing = new Drawing();
        }

        public class Combo
        {
            public static Menu ComboMenu;
            private static CheckBox Q;
            public static bool UseQ { get { return Q.CurrentValue; } }

            private static CheckBox W;
            public static bool UseW { get { return W.CurrentValue; } }

            private static CheckBox E;
            public static bool UseE { get { return E.CurrentValue; } }

            private static CheckBox R;
            public static bool UseR { get { return R.CurrentValue; } }

            private static CheckBox Isolated;
            public static bool IsolatedCombo { get { return Isolated.CurrentValue; } }

            private static CheckBox IsolatedOnly;
            public static bool IsolatedComboOnly { get { return IsolatedOnly.CurrentValue; } }

            private static CheckBox DoubleJump;
            public static bool DoubleJumpCombo { get { return DoubleJump.CurrentValue; } }

            private static CheckBox AAStealth;
            public static bool NoAAStealth { get { return AAStealth.CurrentValue; } }

            private static CheckBox ETurret;
            public static bool NoETurrets { get { return ETurret.CurrentValue; } }

            private static CheckBox EExcute;
            public static bool ExecuteTargetE { get { return EExcute.CurrentValue; } }

            private static Slider Edis;
            public static int MinEDistance { get { return Edis.CurrentValue; } }

            private static Slider EEnemyCount;
            public static int NoEIntoCount { get { return EEnemyCount.CurrentValue; } }

            public Combo()
            {
                ComboMenu = MenuIni.AddSubMenu("Combo Settings");
                ComboMenu.AddGroupLabel("Settings");
                Q = ComboMenu.CreateCheckBox("Q", "Use Q");
                W = ComboMenu.CreateCheckBox("W", "Use W");
                E = ComboMenu.CreateCheckBox("E", "Use E");
                R = ComboMenu.CreateCheckBox("R", "Use R");
                ComboMenu.AddGroupLabel("Misc");
                Isolated = ComboMenu.CreateCheckBox("Isolated", "Focus Isolated Targets");
                IsolatedOnly = ComboMenu.CreateCheckBox("IsolatedOnly", "Cast Spells On Isolated Targets Only", false);
                DoubleJump = ComboMenu.CreateCheckBox("DoubleJump", "Enable DoubleJump Combo");
                AAStealth = ComboMenu.CreateCheckBox("AAStealth", "No AA While Stealth");
                ETurret = ComboMenu.CreateCheckBox("ETurret", "No E Under Turrets");
                EExcute = ComboMenu.CreateCheckBox("EExcute", "Ignore Safety Checks To Execute Target", false);
                EEnemyCount = ComboMenu.CreateSlider("EEnemyCount", "Dont E Into {0} or more Enemies", 3, 1, 6);
                Edis = ComboMenu.CreateSlider("Edis", "Min Distance To Cast E", 450, 0, 950);
            }
        }

        public class Harass
        {
            public static Menu HarassMenu;
            private static CheckBox Q;
            public static bool UseQ { get { return Q.CurrentValue; } }
            private static Slider QM;
            public static int QMana { get { return QM.CurrentValue; } }
            private static CheckBox W;
            public static bool UseW { get { return W.CurrentValue; } }
            private static Slider WM;
            public static int wMana { get { return WM.CurrentValue; } }
            private static CheckBox IsolatedOnly;
            public static bool IsolatedComboOnly { get { return IsolatedOnly.CurrentValue; } }
            public Harass()
            {
                HarassMenu = MenuIni.AddSubMenu("Harass Settings");

                IsolatedOnly = HarassMenu.CreateCheckBox("IsolatedOnly", "Cast Spells On Isolated Targets Only", false);

                HarassMenu.AddGroupLabel("Settings");
                Q = HarassMenu.CreateCheckBox("Q", "Use Q");
                QM = HarassMenu.CreateSlider("Qmana", "Q Mana Manager {0}%", 60);

                W = HarassMenu.CreateCheckBox("W", "Use W");
                WM = HarassMenu.CreateSlider("Wmana", "W Mana Manager {0}%", 60);
            }
        }

        public class LaneClear
        {
            public static Menu LaneMenu;
            private static CheckBox Q;
            public static bool UseQ { get { return Q.CurrentValue; } }
            private static CheckBox LQ;
            public static bool UseLQ { get { return LQ.CurrentValue; } }
            private static Slider QM;
            public static int QMana { get { return QM.CurrentValue; } }
            private static CheckBox W;
            public static bool UseW { get { return W.CurrentValue; } }
            private static Slider WM;
            public static int wMana { get { return WM.CurrentValue; } }
            public LaneClear()
            {
                LaneMenu = MenuIni.AddSubMenu("LaneClear Settings");
                LaneMenu.AddGroupLabel("Settings");
                Q = LaneMenu.CreateCheckBox("Q", "Use Q");
                LQ = LaneMenu.CreateCheckBox("LQ", "Last Hit Q Only");
                QM = LaneMenu.CreateSlider("Qmana", "Q Mana Manager {0}%", 60);

                W = LaneMenu.CreateCheckBox("W", "Use W");
                WM = LaneMenu.CreateSlider("Wmana", "W Mana Manager {0}%", 60);
            }
        }

        public class JungleClear
        {
            public static Menu JungleMenu;
            private static CheckBox Q;
            public static bool UseQ { get { return Q.CurrentValue; } }
            private static Slider QM;
            public static int QMana { get { return QM.CurrentValue; } }
            private static CheckBox QIsolated;
            public static bool QIsolatedOnly { get { return QIsolated.CurrentValue; } }
            private static CheckBox W;
            public static bool UseW { get { return W.CurrentValue; } }
            private static Slider WM;
            public static int wMana { get { return WM.CurrentValue; } }

            public JungleClear()
            {
                JungleMenu = MenuIni.AddSubMenu("JungleClear Settings");
                JungleMenu.AddGroupLabel("Settings");
                Q = JungleMenu.CreateCheckBox("Q", "Use Q");
                QIsolated = JungleMenu.CreateCheckBox("QIsolated", "Use Q On Isolated Mobs Only");
                QM = JungleMenu.CreateSlider("Qmana", "Q Mana Manager {0}%", 60);

                W = JungleMenu.CreateCheckBox("W", "Use W");
                WM = JungleMenu.CreateSlider("Wmana", "W Mana Manager {0}%", 60);
            }
        }

        public class KillSteal
        {
            public static Menu KillStealMenu;
            private static CheckBox Q;
            public static bool UseQ { get { return Q.CurrentValue; } }
            private static CheckBox W;
            public static bool UseW { get { return W.CurrentValue; } }
            private static CheckBox E;
            public static bool UseE { get { return E.CurrentValue; } }
            public KillSteal()
            {
                KillStealMenu = MenuIni.AddSubMenu("KillSteal Settings");
                KillStealMenu.AddGroupLabel("Settings");
                Q = KillStealMenu.CreateCheckBox("Q", "Use Q");
                W = KillStealMenu.CreateCheckBox("W", "Use W");
                E = KillStealMenu.CreateCheckBox("E", "Use E");
            }
        }

        public class Drawing
        {
            public static Menu menu;
            private static CheckBox Q;
            public static bool UseQ { get { return Q.CurrentValue; } }
            private static CheckBox W;
            public static bool UseW { get { return W.CurrentValue; } }
            private static CheckBox E;
            public static bool UseE { get { return E.CurrentValue; } }
            public Drawing()
            {
                menu = MenuIni.AddSubMenu("Drawing Settings");
                menu.AddGroupLabel("Settings");
                Q = menu.CreateCheckBox("Q", "Draw Q Range");
                W = menu.CreateCheckBox("W", "Draw W Range");
                E = menu.CreateCheckBox("E", "Draw E Range");
            }
        }
    }
}
