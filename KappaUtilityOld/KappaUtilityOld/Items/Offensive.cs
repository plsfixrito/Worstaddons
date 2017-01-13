using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtilityOld.Common;

namespace KappaUtilityOld.Items
{
    internal class Offensive
    {
        public static readonly Item Hydra = new Item(3074, 250f);

        public static readonly Item Titanic = new Item(3748, Player.Instance.GetAutoAttackRange());

        public static readonly Item Timat = new Item(3077, 250f);

        public static readonly Item Cutlass = new Item(3144, 550);

        public static readonly Item Botrk = new Item(3153, 550);

        public static readonly Item Youmuu = new Item(3142);

        protected static readonly Item Gunblade = new Item(3146, 700f);

        protected static readonly Item ProtoBelt = new Item(3152, 600);

        protected static readonly Item GLP = new Item(3030, 600);

        public static Menu OffMenu { get; private set; }

        protected static bool loaded = false;

        internal static void OnLoad()
        {
            OffMenu = Load.UtliMenu.AddSubMenu("Offense Items");
            OffMenu.AddGroupLabel("Offense Settings");
            OffMenu.Checkbox("Hydra", " Hydra / Timat / Titanic");
            OffMenu.Checkbox("useGhostblade", " Use Youmuu's Ghostblade");
            OffMenu.Checkbox("UseBOTRK", " Use Blade of the Ruined King");
            OffMenu.Checkbox("UseBilge", " Use Bilgewater Cutlass");
            OffMenu.Checkbox("UseGunblade", " Use Hextech Gunblade");
            OffMenu.Checkbox("UseBelt", " Use Hextech ProtoBelt-01");
            OffMenu.Checkbox("UseGLP", " Use Hextech GLP-800");
            OffMenu.AddSeparator();
            OffMenu.AddGroupLabel("Settings");
            OffMenu.Checkbox("UseKS", " Use for KillSteal");
            OffMenu.Checkbox("UseCombo", " Use In Combo");
            OffMenu.Slider("eL", "Use On Enemy health", 65);
            OffMenu.Slider("oL", "Use On My health", 65);

            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            loaded = true;
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!loaded)
            {
                return;
            }
            if (!target.IsEnemy || !(target is AIHeroClient))
            {
                return;
            }

            var useHydra = OffMenu["Hydra"].Cast<CheckBox>().CurrentValue
                           && ((Hydra.IsOwned(Player.Instance) && Hydra.IsReady()) || (Timat.IsOwned(Player.Instance) && Timat.IsReady())
                               || (Titanic.IsOwned(Player.Instance) && Titanic.IsReady()));
            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Youmuu.IsReady() && Youmuu.IsOwned(Player.Instance) && target.IsValidTarget(500) && OffMenu.GetCheckbox("useGhostblade"))
                {
                    Youmuu.Cast();
                }
                if (useHydra)
                {
                    if (Hydra.IsOwned() && Hydra.IsReady() && Hydra != null)
                    {
                        if (Hydra.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                        }
                    }

                    if (Timat.IsOwned() && Timat.IsReady() && Timat != null)
                    {
                        if (Timat.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                        }
                    }
                    if (Titanic.IsOwned() && Titanic.IsReady() && Titanic != null)
                    {
                        if (Titanic.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                        }
                    }
                }
            }
        }

        internal static void Items()
        {
            if (!loaded)
            {
                return;
            }

            if (OffMenu["UseKS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy != null && enemy.IsKillable() && enemy.IsValidTarget(600))
                    {
                        if (OffMenu.GetCheckbox("UseGunblade") && Player.Instance.GetItemDamage(enemy, ItemId.Hextech_Gunblade) >= enemy.Health)
                        {
                            Gunblade.Cast(enemy);
                        }
                        if (OffMenu.GetCheckbox("UseBOTRK") && Player.Instance.GetItemDamage(enemy, ItemId.Blade_of_the_Ruined_King) >= enemy.Health)
                        {
                            Botrk.Cast(enemy);
                        }
                        if (OffMenu.GetCheckbox("UseBilge") && Player.Instance.GetItemDamage(enemy, ItemId.Bilgewater_Cutlass) >= enemy.Health)
                        {
                            Cutlass.Cast(enemy);
                        }
                        if (OffMenu.GetCheckbox("Hydra"))
                        {
                            if (Hydra.IsOwned(Player.Instance) && Hydra.IsReady() && Player.Instance.GetItemDamage(enemy, Hydra.Id) >= enemy.Health)
                            {
                                Hydra.Cast();
                            }
                            if (Timat.IsOwned(Player.Instance) && Timat.IsReady() && Player.Instance.GetItemDamage(enemy, Timat.Id) >= enemy.Health)
                            {
                                Timat.Cast();
                            }
                            if (Titanic.IsOwned(Player.Instance) && Titanic.IsReady()
                                && Player.Instance.GetItemDamage(enemy, Titanic.Id) >= enemy.Health)
                            {
                                Titanic.Cast();
                            }
                        }
                    }
                }
            }

            var target = TargetSelector.GetTarget(600, DamageType.Physical);
            if (target == null || !target.IsValidTarget() || !OffMenu.GetCheckbox("UseCombo"))
            {
                return;
            }

            if (target.HealthPercent <= OffMenu.GetSlider("eL") || Player.Instance.HealthPercent <= OffMenu.GetSlider("oL"))
            {
                if (Gunblade.IsReady() && Gunblade.IsOwned(Player.Instance) && target.IsValidTarget(Gunblade.Range)
                    && OffMenu.GetCheckbox("UseGunblade"))
                {
                    Gunblade.Cast(target);
                }

                if (Botrk.IsReady() && Botrk.IsOwned(Player.Instance) && target.IsValidTarget(Botrk.Range) && OffMenu.GetCheckbox("UseBOTRK"))
                {
                    Botrk.Cast(target);
                }

                if (Cutlass.IsReady() && Cutlass.IsOwned(Player.Instance) && target.IsValidTarget(Cutlass.Range) && OffMenu.GetCheckbox("UseBilge"))
                {
                    Cutlass.Cast(target);
                }

                if (ProtoBelt.IsOwned(Player.Instance) && ProtoBelt.IsReady() && OffMenu.GetCheckbox("UseBelt"))
                {
                    ProtoBelt.Cast(target.ServerPosition);
                }

                if (GLP.IsOwned(Player.Instance) && GLP.IsReady() && OffMenu.GetCheckbox("UseGLP"))
                {
                    GLP.Cast(target.ServerPosition);
                }
            }
        }
    }
}