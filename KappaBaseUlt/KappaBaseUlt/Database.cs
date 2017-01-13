using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Enumerations;

namespace KappaBaseUlt
{
    internal class Database
    {
        internal class Hero
        {
            public Champion Champion;
            public SpellSlot Slot;
            public SkillShotType Type;
            public uint Range;
            public int Width;
            public int CastDelay;
            public int Speed;
            public float AllowedCollisionCount;
            public Hero(Champion champion, SpellSlot slot, SkillShotType type, uint range, int width, int castdelay, int speed, int allowedCollisionCount)
            {
                this.Champion = champion;
                this.Slot = slot;
                this.Type = type;
                this.Range = range;
                this.Width = width;
                this.CastDelay = castdelay;
                this.Speed = speed;
                this.AllowedCollisionCount = allowedCollisionCount;
            }
        }

        internal static readonly List<Hero> Champions = new List<Hero>
        {
            new Hero(Champion.Ashe, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 250, 250, 1600, 0),
            new Hero(Champion.Draven, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 160, 300, 2000, int.MaxValue),
            new Hero(Champion.Ezreal, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 160, 1000, 2000, int.MaxValue),
            new Hero(Champion.Karthus, SpellSlot.R, SkillShotType.Circular, int.MaxValue, 0, 4700, int.MaxValue, int.MaxValue),
            new Hero(Champion.Lux, SpellSlot.R, SkillShotType.Linear, 3340, 250, 1000, int.MaxValue, int.MaxValue),
            new Hero(Champion.Pantheon, SpellSlot.R, SkillShotType.Circular, 5500, 600, 4700, int.MaxValue, int.MaxValue),
            new Hero(Champion.Gangplank, SpellSlot.R, SkillShotType.Circular, int.MaxValue, 600, 250, int.MaxValue, int.MaxValue),
            new Hero(Champion.Jinx, SpellSlot.R, SkillShotType.Linear, int.MaxValue, 140, 600, 2100, 0),
            new Hero(Champion.Ziggs, SpellSlot.R, SkillShotType.Circular, 5300, 500, 250, 1500, int.MaxValue)
        };

        /*
        internal struct Damage
        {
            public Champion Champion;

            public DamageType DamageType;

            public float[] Floats;

            public float Float;
        }

        internal static readonly List<Damage> Damages = new List<Damage>
        {
            new Damage
        {
                    Champion = Champion.Ashe, DamageType = DamageType.Magical,
                    Floats = new float[] { 250, 425, 600 }, Float = 1f
        },
            new Damage
        {
                    Champion = Champion.Draven, DamageType = DamageType.Physical,
                    Floats = new float[] { 175, 275, 375 }, Float = 1.1f
        },
            new Damage
        {
                    Champion = Champion.Ezreal, DamageType = DamageType.Magical,
                    Floats = new float[] { 350, 500, 600 }, Float = 1f
        },
            new Damage
        {
                    Champion = Champion.Lux, DamageType = DamageType.Magical,
                    Floats = new float[] { 300, 400, 500 }, Float = 0.75f
        },
            new Damage
        {
                    Champion = Champion.Karthus, DamageType = DamageType.Magical,
                    Floats = new float[] { 300, 400, 500 }, Float = 0.6f
        },
            new Damage
        {
                    Champion = Champion.Pantheon, DamageType = DamageType.Magical,
                    Floats = new float[] { 200, 350, 500 }, Float = 0.5f
        },
            new Damage
        {
                    Champion = Champion.Gangplank, DamageType = DamageType.Magical,
                    Floats = new float[] { 50, 70, 90 }, Float = 0.1f
        },
            new Damage
        {
                    Champion = Champion.Jinx, DamageType = DamageType.Physical,
                    Floats = new float[] { 200, 300, 400 }, Float = 0.1f
        },
            new Damage
        {
                    Champion = Champion.Ziggs, DamageType = DamageType.Magical,
                    Floats = new float[] { 200, 300, 400 }, Float = 0.73f
        }
        };*/
    }
}
