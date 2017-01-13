using System.Collections.Generic;
using EloBuddy.SDK;

namespace KappaUtility.Common.Databases
{
    internal class ItemsDatabase
    {
        // Wards
        public static Item EyeOfTheEquinox = new Item(2303, 625);
        public static Item EyeOfTheOasis = new Item(2302, 625);
        public static Item EyeOfTheWatchers = new Item(2301, 625);
        public static Item TrackerKnife = new Item(3711, 625);
        public static Item TrackerKnife_Cinderhulk = new Item(1409, 625);
        public static Item TrackerKnife_Runic_Echoes = new Item(1410, 625);
        public static Item TrackerKnife_Warrior = new Item(1408, 625);
        public static Item SightStone = new Item(2049, 625);
        public static Item RubySightStone = new Item(2045, 625);
        public static Item ControlWard = new Item(2055, 725);
        public static Item YellowTrinket = new Item(3340, 625);
        public static Item BlueTrinket = new Item(3363, 600);
        public static Item RedTrinket = new Item(3341, 600);
        public static Item RedTrinket2 = new Item(3364, 600);

        // Qss
        public static Item Mercurial_Scimitar = new Item(3139);
        public static Item Quicksilver_Sash = new Item(3140);
        public static Item Dervish_Blade = new Item(3137);
        public static Item Mikaels = new Item(3222, 600);

        // AD
        public static Item Youmuus = new Item(3142);
        public static Item Cutlass = new Item(3144, 550);
        public static Item Botrk = new Item(3153, 550);
        public static Item Tiamat = new Item(3077, 200);
        public static Item Hydra = new Item(3074, 200);
        public static Item TitanicHydra = new Item(3748, 200);

        // AP
        public static Item Hextech_Gunblade = new Item(3146, 600);
        public static Item Hextech_ProtoBelt = new Item(3152, 600);
        public static Item Hextech_GLP = new Item(3030, 600);

        // Pots
        public static Item HealthPotion = new Item(2003);
        public static Item Biscuit = new Item(2010);
        public static Item RefillablePotion = new Item(2031);
        public static Item CorruptingPotion = new Item(2033);
        public static Item HuntersPotion = new Item(2032);

        // Tank
        public static Item Seraphs = new Item(3048);
        public static Item Zhonyas = new Item(3157);
        public static Item Solari = new Item(3190, 600);
        public static Item Randuins = new Item(3143, 450);
        public static Item FOTM = new Item(3401, 900);
        public static Item Redemption = new Item(3107, 5500);
        public static Item EdgeofNight = new Item(3814);

        public static List<Item> SelfQssItems = new List<Item> { Mercurial_Scimitar, Quicksilver_Sash, Dervish_Blade };
        public static List<Item> AllyQssItems = new List<Item> { Mikaels };
        public static List<Item> AfterAttackItems = new List<Item> { Youmuus, Tiamat, Hydra, TitanicHydra };
        public static List<Item> LifeStealItems = new List<Item> { Hextech_Gunblade, Cutlass, Botrk };
        public static List<Item> DamageItems = new List<Item> { Hextech_ProtoBelt, Hextech_GLP };
        public static List<Item> SelfSaverItems = new List<Item> { Seraphs, Zhonyas, Randuins, EdgeofNight };
        public static List<Item> AllySaverItems = new List<Item> { Solari, FOTM, Redemption };
        public static List<Item> WardingItems = new List<Item>
            {
            EyeOfTheEquinox, EyeOfTheOasis, EyeOfTheWatchers,
            TrackerKnife, TrackerKnife_Cinderhulk, TrackerKnife_Runic_Echoes, TrackerKnife_Warrior,
            SightStone, RubySightStone, YellowTrinket, BlueTrinket
            };
        public static List<Item> Potions = new List<Item>
            {
                HealthPotion, Biscuit, RefillablePotion, CorruptingPotion, HuntersPotion
            };
        public static List<Item> VisionWardingItems = new List<Item> { ControlWard, RedTrinket, RedTrinket2 };
    }
}
