using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtilityOld.Common;

namespace KappaUtilityOld.Items
{
    internal class AutoTear
    {
        protected static bool loaded = false;

        public static readonly Item Tear = new Item(3070);

        public static readonly Item Tearcs = new Item(3073);

        public static readonly Item Arch = new Item(3003);

        public static readonly Item Archcs = new Item(3007);

        public static readonly Item Mana = new Item(3004);

        public static readonly Item Manacs = new Item(3008);

        public static readonly Item Mura = new Item(3043);

        public static readonly Item Sera = new Item(3048);

        public static Menu TearMenu { get; private set; }

        internal static void OnLoad()
        {
            TearMenu = Load.UtliMenu.AddSubMenu("AutoTearStacker");
            TearMenu.AddGroupLabel("AutoTear Settings");
            TearMenu.Add(Player.Instance.ChampionName + "enable", new KeyBind("Enable Toggle", false, KeyBind.BindTypes.PressToggle, 'M'));
            TearMenu.Checkbox("shop", "Stack Only In Shop Range");
            TearMenu.Checkbox("enemy", "Stop Stacking if Enemies Near");
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("Mana Manager");
            TearMenu.Slider("manasave", "Save Mana [{0}%] ", 85);
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("Item Settings");
            TearMenu.Checkbox("tear", "Stack Tear");
            TearMenu.Checkbox("arch", "Stack Archangels Staff");
            TearMenu.Checkbox("mana", "Stack Manamune");
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("Stacking Spell");
            TearMenu.Checkbox(Player.Instance.ChampionName + "Q", "Use Q");
            TearMenu.Checkbox(Player.Instance.ChampionName + "W", "Use W");
            TearMenu.Checkbox(Player.Instance.ChampionName + "E", "Use E");
            TearMenu.Checkbox(Player.Instance.ChampionName + "R", "Use R");
            loaded = true;
        }

        internal static void OnUpdate()
        {
            if (!loaded)
            {
                return;
            }

            if (TearMenu[Player.Instance.ChampionName + "enable"].Cast<KeyBind>().CurrentValue)
            {
                var items = ((Tearcs.IsOwned(Player.Instance) || Tear.IsOwned(Player.Instance)) && TearMenu.GetCheckbox("tear"))
                            || ((Arch.IsOwned() || Archcs.IsOwned()) && TearMenu.GetCheckbox("arch"))
                            || ((Manacs.IsOwned() || Mana.IsOwned()) && TearMenu.GetCheckbox("mana"));
                var items2 = Sera.IsOwned() || Mura.IsOwned();
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions;

                foreach (var minion in
                    minions.Where(minion => items && !items2 && Player.Instance.ManaPercent > TearMenu.GetSlider("manasave")))
                {
                    if (TearMenu.GetCheckbox("enemy") && Helpers.CountEnemies(1250) >= 1)
                    {
                        return;
                    }

                    if (TearMenu.GetCheckbox("shop") && !Player.Instance.IsInShopRange())
                    {
                        return;
                    }

                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) || Player.Instance.Spellbook.IsChanneling)
                    {
                        return;
                    }

                    Cast(minion);
                }
            }
        }

        internal static void Cast(Obj_AI_Base target)
        {
            var useQ = Player.GetSpell(SpellSlot.Q).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "Q");

            var useW = Player.GetSpell(SpellSlot.W).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "W");

            var useE = Player.GetSpell(SpellSlot.E).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "E");

            var useR = Player.GetSpell(SpellSlot.R).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "R");
            if (useQ)
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            }

            if (useW)
            {
                Player.CastSpell(SpellSlot.W, Game.CursorPos);
            }

            if (useE)
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }

            if (useR)
            {
                Player.CastSpell(SpellSlot.R, Game.CursorPos);
            }
        }
    }
}