using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace GenesisUrgot
{
    public static class SpellManager
    {
        // You will need to edit the types of spells you have for each champ as they
        // don't have the same type for each champ, for example Xerath Q is chargeable,
        // right now it's  set to Active.
        public static Spell.Skillshot Q1 { get; private set; }
        public static Spell.Skillshot Q2 { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Targeted R { get; private set; }

        static SpellManager()
        {
            // Initialize spells
            Q1 = new Spell.Skillshot(SpellSlot.Q, 975, SkillShotType.Linear, 175, 1600, 50);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Circular, 0, null, 1) { MinimumHitChance = HitChance.Impossible };
            // TODO: Uncomment the other spells to initialize them
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 875, SkillShotType.Circular, 250, 1750, 230)
            {MinimumHitChance = HitChance.High};
            R = new Spell.Targeted(SpellSlot.R,550);
        }

        public static void Initialize()
        {
            // Let the static initializer do the job, this way we avoid multiple init calls aswell
        }
    }
}
