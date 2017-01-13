using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtilityOld.Common;

namespace KappaUtilityOld.Items
{
    internal class Defensive
    {
        public static readonly Item Zhonyas = new Item(3157);

        public static readonly Item Seraph = new Item(3048);

        public static readonly Item FOTM = new Item(3401, 600);

        public static readonly Item Solari = new Item(3190, 600);

        public static readonly Item Randuin = new Item(3143, 500f);

        public static int Seraphh => DefMenu.GetSlider("Seraphh");

        public static int Solarih => DefMenu.GetSlider("Solarih");

        public static int FaceOfTheMountainh => DefMenu.GetSlider("FaceOfTheMountainh");

        public static int Zhonyash => DefMenu.GetSlider("Zhonyash");

        public static int Seraphn => DefMenu.GetSlider("Seraphn");

        public static int Solarin => DefMenu.GetSlider("Solarin");

        public static int FaceOfTheMountainn => DefMenu.GetSlider("FaceOfTheMountainn");

        public static int Zhonyasn => DefMenu.GetSlider("Zhonyasn");

        public static bool Seraphc => DefMenu.GetCheckbox("Seraph") && Seraph.IsOwned(Player.Instance) && Seraph.IsReady();

        public static bool Solaric => DefMenu.GetCheckbox("Solari") && Solari.IsOwned(Player.Instance) && Solari.IsReady();

        public static bool FaceOfTheMountainc => DefMenu.GetCheckbox("FaceOfTheMountain") && FOTM.IsOwned(Player.Instance) && FOTM.IsReady();

        public static bool Zhonyasc => DefMenu.GetCheckbox("Zhonyas") && Zhonyas.IsOwned(Player.Instance) && Zhonyas.IsReady();

        public static Menu DefMenu { get; private set; }

        protected static bool loaded = false;

        internal static void OnLoad()
        {
            DefMenu = Load.UtliMenu.AddSubMenu("Defence Items");
            DefMenu.AddGroupLabel("Defence Settings");
            DefMenu.Checkbox("Zhonyas", "Use Zhonyas");
            DefMenu.Slider("Zhonyash", "Use Zhonyas health [{0}%]", 35);
            DefMenu.Slider("Zhonyasn", "Use Zhonyas if incoming Damage more than [{0}%]", 50);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("Seraph", "Use Seraph");
            DefMenu.Slider("Seraphh", "Use Seraph health [{0}%]", 45);
            DefMenu.Slider("Seraphn", "Use Seraph if incoming Damage more than [{0}%]", 45);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("FaceOfTheMountain", "Use Face Of The Mountain");
            DefMenu.Slider("FaceOfTheMountainh", "Use FOTM health [{0}%]", 50);
            DefMenu.Slider("FaceOfTheMountainn", "Use FOTM if incoming Damage more than [{0}%]", 50);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("Solari", "Use Solari");
            DefMenu.Slider("Solarih", "Use Solari health [{0}%]", 30);
            DefMenu.Slider("Solarin", "Use Solari if incoming Damage more than [{0}%]", 35);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("Randuin", "Use Randuin (Omen)");
            DefMenu.Slider("Randuinh", "Use Randuin On [{0}] Enemies", 2, 1, 5);
            DefMenu.AddSeparator();
            DefMenu.AddGroupLabel("Zhonya Danger Spells");
            DefMenu.Checkbox("ZhonyasD", "Deny Dangers Spells");
            DamageHandler.OnLoad();
            Zhonya.OnLoad();
            loaded = true;
        }

        internal static void Items()
        {
            if (!loaded)
            {
                return;
            }

            if (Randuin.IsReady() && Randuin.IsOwned(Player.Instance) && Helpers.CountEnemies((int)Randuin.Range) >= DefMenu.GetSlider("Randuinh")
                && DefMenu.GetCheckbox("Randuin"))
            {
                Randuin.Cast();
            }
        }

        public static void defcast(Obj_AI_Base caster, Obj_AI_Base target, Obj_AI_Base enemy, float dmg)
        {
            var damagepercent = (dmg / target.TotalShieldHealth()) * 100;
            var death = damagepercent >= target.HealthPercent || dmg >= target.TotalShieldHealth();

            if (target.IsValidTarget(Defensive.FOTM.Range) && Defensive.FaceOfTheMountainc)
            {
                if (Defensive.FaceOfTheMountainh >= target.HealthPercent || death || damagepercent >= Defensive.FaceOfTheMountainn)
                {
                    Defensive.FOTM.Cast(target);
                }
            }

            if (target.IsValidTarget(Defensive.Solari.Range) && Defensive.Solaric)
            {
                if (Defensive.Solarih >= target.HealthPercent || death || damagepercent >= Defensive.Solarin)
                {
                    Defensive.Solari.Cast();
                }
            }

            if (target.IsMe)
            {
                if (Defensive.Seraphc)
                {
                    if (Defensive.Seraphh >= target.HealthPercent || death || damagepercent >= Defensive.Seraphn)
                    {
                        Defensive.Seraph.Cast();
                    }
                }

                if (Defensive.Zhonyasc)
                {
                    if (Defensive.Zhonyash >= target.HealthPercent || death || damagepercent >= Defensive.Zhonyasn)
                    {
                        Defensive.Zhonyas.Cast();
                    }
                }
            }
        }
    }
}