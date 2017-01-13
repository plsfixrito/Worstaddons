using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static KappaKled.Program;

namespace KappaKled.Modes
{
    class LaneClear
    {
        public static void Execute(bool Jungle = false)
        {
            foreach (var target in EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m != null && m.IsValidTarget()))
            {
                if (Q.IsReady() && LaneClearMenu.CheckBoxValue("Q"))
                {
                    Q.Cast(target, HitChance.Medium);
                }

                if (E.IsReady() && target.IsKillable(E.Range) && LaneClearMenu.CheckBoxValue("E"))
                {
                    E.Cast(target, HitChance.Medium);
                }
            }

            if(!Jungle) return;
            foreach (var target in EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m != null && m.IsValidTarget()))
            {
                if (Q.IsReady() && LaneClearMenu.CheckBoxValue("Q"))
                {
                    Q.Cast(target, HitChance.Medium);
                }

                if (E.IsReady() && target.IsKillable(E.Range) && LaneClearMenu.CheckBoxValue("E"))
                {
                    E.Cast(target, HitChance.Medium);
                }
            }
        }
    }
}
