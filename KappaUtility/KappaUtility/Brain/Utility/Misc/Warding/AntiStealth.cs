using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;
using SharpDX;
using static KappaUtility.Common.Databases.ItemsDatabase;

namespace KappaUtility.Brain.Utility.Misc.Warding
{
    internal class AntiStealth
    {
        internal class Stealth
        {
            public string Champion;
            public SpellSlot Slot;
            public string SpellName;
            public string BuffName;

            public Stealth(string champion, SpellSlot slot, string Spellname, string Buffname)
            {
                this.Champion = champion;
                this.Slot = slot;
                this.SpellName = Spellname;
                this.BuffName = Buffname;
            }
        }

        private static List<Stealth> StealthSpells = new List<Stealth>
            {
                new Stealth("Akali", SpellSlot.W, "", "akaliwstealth"),
                new Stealth("Talon", SpellSlot.R, "TalonShadowAssault", ""),
                new Stealth("Twitch", SpellSlot.Q, "TwitchHideInShadows", "TwitchHideInShadows"),
                new Stealth("MonkeyKing", SpellSlot.W, "MonkeyKingDecoy", ""),
                new Stealth("Shaco", SpellSlot.Q, "deceive", ""),
                new Stealth("KhaZix", SpellSlot.R, "KhaZixR", ""),
                new Stealth("Vayne", SpellSlot.Q, "vaynetumble", "vayneinquisition"),
            };

        private static Menu menu;

        public static void Init()
        {
            var i = 0;
            menu = Utility.Load.menu.AddSubMenu("Anti-Stealth");

            menu.AddGroupLabel("Anti-Stealth");
            menu.AddLabel("Reveals Hidden Enemies using Vision Items.");
            menu.AddSeparator(5);

            menu.AddGroupLabel("Anti-Stealth - Enemy Spells");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                foreach (var spell in StealthSpells.Where(a => enemy.ChampionName.Equals(a.Champion, StringComparison.CurrentCultureIgnoreCase)))
                {
                    menu.CreateCheckBox(enemy.Name(), $"{enemy.Name()} - {spell.Slot}");
                    i++;
                }
            }

            if (i == 0)
            {
                menu.AddLabel("No Stealth Spells Detected");
                return;
            }

            menu.AddSeparator(0);

            menu.AddGroupLabel("Anti-Stealth - Items");
            foreach (var item in VisionWardingItems)
            {
                menu.CreateCheckBox(item.ItemInfo.Name, $"Use {item.ItemInfo.Name}");
            }

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero == null || !hero.IsEnemy)
                return;

            var stealth = StealthSpells.FirstOrDefault(
                s => s.Champion.Equals(hero.ChampionName, StringComparison.CurrentCultureIgnoreCase) && args.Buff.Name.Equals(s.BuffName));

            if (stealth != null && menu.CheckBoxValue(hero.Name()))
            {
                RevealTarget(hero.ServerPosition, hero.PrediectPosition());
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if(hero == null || !hero.IsEnemy)
                return;

            var stealth = StealthSpells.FirstOrDefault(s => s.Champion.Equals(hero.ChampionName, StringComparison.CurrentCultureIgnoreCase)
            && s.Slot == args.Slot && args.SData.Name.Equals(s.SpellName, StringComparison.CurrentCultureIgnoreCase));

            if (stealth != null && menu.CheckBoxValue(hero.Name()))
            {
                var buff = stealth.BuffName == "" ||
                    hero.Buffs.Any(
                        b =>
                        b.IsActive && b.IsValid
                        && (b.Name.Equals(stealth.BuffName, StringComparison.CurrentCultureIgnoreCase) || b.DisplayName.Equals(stealth.BuffName, StringComparison.CurrentCultureIgnoreCase)));

                if (buff)
                {
                    RevealTarget(args.Start, args.End);
                }
            }
        }

        private static void RevealTarget(Vector3 start, Vector3 end)
        {
            foreach (var vision in VisionWardingItems.Where(i => i.ItemReady(menu)))
            {
                if (vision.IsInRange(start) && !vision.IsInRange(end))
                {
                    end = Player.Instance.ServerPosition.Extend(end, vision.Range).To3D();
                }

                var warded = ObjectManager.Get<Obj_AI_Base>().Any(o => o.IsVisionWard() && o.IsAlly && !o.IsDead && o.IsValid && o.IsInRange(end, 600));
                if(!warded)
                    vision.Cast(end);
                return;
            }
        }
    }
}
