using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Spells;

namespace AutoSteal.Misc
{
    internal static class Common
    {
        public static bool WillKill(this Spell.Skillshot spell, Obj_AI_Base target)
        {
            var traveltime = target.Distance(Player.Instance) / spell.Speed * 1000 + spell.CastDelay;
            return target.IsKillable(spell.Range) && Player.Instance.GetSpellDamage(target, spell.Slot) >= Prediction.Health.GetPrediction(target, (int)traveltime);
        }

        public static SkillShotType type(SpellType spelltype)
        {
            switch (spelltype)
            {
                case SpellType.Circle:
                case SpellType.MissileAoe:
                case SpellType.Self:
                    return SkillShotType.Circular;
                case SpellType.Line:
                case SpellType.Arc:
                case SpellType.MissileLine:
                    return SkillShotType.Linear;
                case SpellType.Cone:
                case SpellType.Ring:
                    return SkillShotType.Cone;
                default:
                    return SkillShotType.Linear;
            }
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this AIHeroClient target, float range)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.HasUndyingBuff(true) && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range);
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this AIHeroClient target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.HasUndyingBuff(true) && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target, float range)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range);
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        /// <summary>
        ///     Creates a checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(this Menu m, string id, string name, bool defaultvalue = true)
        {
            return m.Add(id, new CheckBox(name, defaultvalue));
        }

        /// <summary>
        ///     Returns CheckBox Value.
        /// </summary>
        public static bool CheckBoxValue(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        ///     Returns a recreated name of the target.
        /// </summary>
        public static string Name(this Obj_AI_Base target)
        {
            return target.BaseSkinName + "(" + target.Name + ")";
        }

        /// <summary>
        ///     Supported Jungle Mobs BaseSkinNames.
        /// </summary>
        public static string[] JungleMobsNames;

        /// <summary>
        ///     SummonersRift Supported Jungle Mobs.
        /// </summary>
        public static string[] SRJungleMobsNames = { "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "Sru_Crab", "SRU_Murkwolf", "SRU_Blue", "SRU_Red", "SRU_RiftHerald" };

        /// <summary>
        ///     TwistedTreeline Supported Jungle Mobs.
        /// </summary>
        public static string[] TTJungleMobsNames = { "TT_NWraith", "TT_NWolf", "TT_NGolem", "TT_Spiderboss" };

        /// <summary>
        ///     Ascenion Supported Jungle Mobs.
        /// </summary>
        public static string[] ASCJungleMobsNames = { "AscXerath" };

        /// <summary>
        ///     Returns Supported Jungle Mobs.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> SupportedJungleMobs
        {
            get
            {
                return EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => JungleMobsNames.Any(j => j.Equals(m.BaseSkinName)));
            }
        }
    }
}
