using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace KappaUtility.Common.Databases
{
    internal class SheildsDatabase
    {
        public static List<Shield> Shields = new List<Shield>
        {
            new Shield(Champion.Alistar, new Spell.Active(SpellSlot.E, 575), Shield.SheildType.AllyHeal),
            new Shield(Champion.Alistar, new Spell.Active(SpellSlot.R), Shield.SheildType.SelfSaveior),
            new Shield(Champion.Bard, new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular), Shield.SheildType.AllyHeal),
            new Shield(Champion.Bard, new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Circular), Shield.SheildType.AllySaveior),
            new Shield(Champion.Braum, new Spell.Targeted(SpellSlot.W, 650), Shield.SheildType.AllyShield),
            new Shield(Champion.Braum, new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Linear), Shield.SheildType.Wall),
            new Shield(Champion.Diana, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.DrMundo, new Spell.Active(SpellSlot.R), Shield.SheildType.Self),
            new Shield(Champion.Ekko, new Spell.Active(SpellSlot.R), Shield.SheildType.SelfSaveior),
            new Shield(Champion.Evelynn, new Spell.Skillshot(SpellSlot.R, 650, SkillShotType.Circular), Shield.SheildType.CastOnEnemy),
            new Shield(Champion.Fiora, new Spell.Skillshot(SpellSlot.W, 400, SkillShotType.Linear), Shield.SheildType.Self),
            new Shield(Champion.Fizz, new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Linear), Shield.SheildType.Self),
            new Shield(Champion.Galio, new Spell.Targeted(SpellSlot.W, 800), Shield.SheildType.AllyShield),
            new Shield(Champion.Gangplank, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Garen, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Janna, new Spell.Targeted(SpellSlot.E, 800), Shield.SheildType.AllyShield),
            new Shield(Champion.JarvanIV, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Karma, new Spell.Targeted(SpellSlot.E, 675), Shield.SheildType.AllyShield),
            new Shield(Champion.Kayle, new Spell.Targeted(SpellSlot.W, 900), Shield.SheildType.AllyHeal),
            new Shield(Champion.Kayle, new Spell.Targeted(SpellSlot.R, 900), Shield.SheildType.AllySaveior),
            new Shield(Champion.Kindred, new Spell.Active(SpellSlot.R, 500), Shield.SheildType.AllySaveior),
            new Shield(Champion.LeeSin, new Spell.Targeted(SpellSlot.W, 700), Shield.SheildType.AllyShield),
            new Shield(Champion.Leona, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Lissandra, new Spell.Targeted(SpellSlot.R, 600), Shield.SheildType.SelfSaveior),
            new Shield(Champion.Lulu, new Spell.Targeted(SpellSlot.E, 650), Shield.SheildType.AllyShield),
            new Shield(Champion.Lulu, new Spell.Targeted(SpellSlot.R, 650), Shield.SheildType.AllySaveior),
            new Shield(Champion.Lux, new Spell.Skillshot(SpellSlot.W, 1075, SkillShotType.Linear), Shield.SheildType.AllyShield),
            new Shield(Champion.Morgana, new Spell.Targeted(SpellSlot.E, 800), Shield.SheildType.AllyShield),
            new Shield(Champion.Nami, new Spell.Targeted(SpellSlot.W, 725), Shield.SheildType.AllyHeal),
            new Shield(Champion.Nasus, new Spell.Active(SpellSlot.R), Shield.SheildType.SelfSaveior),
            new Shield(Champion.Nautilus, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Nidalee, new Spell.Targeted(SpellSlot.E, 600), Shield.SheildType.AllyHeal),
            new Shield(Champion.Nocturne, new Spell.Active(SpellSlot.W), Shield.SheildType.SpellBlock),
            new Shield(Champion.Orianna, new Spell.Targeted(SpellSlot.E, 1100), Shield.SheildType.AllyShield),
            new Shield(Champion.Renekton, new Spell.Active(SpellSlot.R), Shield.SheildType.SelfSaveior),
            //new Shield(Champion.Rengar, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Riven, new Spell.Skillshot(SpellSlot.E, 325, SkillShotType.Linear), Shield.SheildType.Self),
            new Shield(Champion.Rumble, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Sion, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Sivir, new Spell.Active(SpellSlot.E), Shield.SheildType.SpellBlock),
            new Shield(Champion.Skarner, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Sona, new Spell.Active(SpellSlot.W, 1000), Shield.SheildType.AllyHeal),
            new Shield(Champion.Soraka, new Spell.Targeted(SpellSlot.W, 550), Shield.SheildType.AllyHeal),
            new Shield(Champion.Soraka, new Spell.Active(SpellSlot.R, int.MaxValue), Shield.SheildType.AllySaveior),
            new Shield(Champion.Shen, new Spell.Targeted(SpellSlot.R, int.MaxValue), Shield.SheildType.AllySaveior),
            new Shield(Champion.TahmKench, new Spell.Targeted(SpellSlot.W, 250), Shield.SheildType.AllyShield),
            new Shield(Champion.TahmKench, new Spell.Active(SpellSlot.E), Shield.SheildType.Self),
            new Shield(Champion.Taric, new Spell.Active(SpellSlot.Q, 350), Shield.SheildType.AllyHeal),
            new Shield(Champion.Taric, new Spell.Targeted(SpellSlot.W, 800), Shield.SheildType.AllyShield),
            new Shield(Champion.Taric, new Spell.Active(SpellSlot.R, 400), Shield.SheildType.AllySaveior),
            new Shield(Champion.Thresh, new Spell.Active(SpellSlot.W, 950), Shield.SheildType.AllyShield),
            new Shield(Champion.Tryndamere, new Spell.Active(SpellSlot.R), Shield.SheildType.SelfSaveior),
            new Shield(Champion.Urgot, new Spell.Active(SpellSlot.W), Shield.SheildType.Self),
            new Shield(Champion.Viktor, new Spell.Targeted(SpellSlot.Q, 600), Shield.SheildType.CastOnEnemy),
            new Shield(Champion.Yasuo, new Spell.Skillshot(SpellSlot.W, 400, SkillShotType.Linear), Shield.SheildType.Wall),
            new Shield(Champion.Zilean, new Spell.Targeted(SpellSlot.R, 900), Shield.SheildType.AllySaveior),
        };

        public class Shield
        {
            public Champion Hero;
            public Spell.SpellBase Spell;
            public enum SheildType { Self, AllyShield, AllyHeal, Wall, CastOnEnemy, SelfSaveior, AllySaveior, SpellBlock }
            public SheildType Type;
            public Shield(Champion hero, Spell.SpellBase spell, SheildType type)
            {
                this.Hero = hero;
                this.Spell = spell;
                this.Type = type;
            }
        }
    }
}
