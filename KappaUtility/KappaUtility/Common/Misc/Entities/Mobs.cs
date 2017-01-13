using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappaUtility.Common.Misc.Entities
{
    internal static class Mobs
    {
        /// <summary>
        ///     SummonersRift Supported Jungle Mobs.
        /// </summary>
        public static string[] JungleMobsNames;

        /// <summary>
        ///     SummonersRift Supported Jungle Mobs.
        /// </summary>
        public static string[] SRJungleMobsNames =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug",
                "SRU_Razorbeak", "Sru_Crab", "SRU_Murkwolf", "SRU_Blue", "SRU_Red", "SRU_RiftHerald"
            };

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

        /// <summary>
        ///     Returns true if the target is big minion (Siege / Super Minion).
        /// </summary>
        public static bool IsBigMinion(this Obj_AI_Base target)
        {
            return target.BaseSkinName.ToLower().Contains("siege") || target.BaseSkinName.ToLower().Contains("super");
        }
    }
}
