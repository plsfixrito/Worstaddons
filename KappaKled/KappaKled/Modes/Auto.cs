using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static KappaKled.Program;

namespace KappaKled.Modes
{
    class Auto
    {
        public static void Execute()
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(e => e != null && e.IsKillable(1000)))
            {
                if (Q.WillKill(target) && Q.IsReady() && KillStealMenu.CheckBoxValue("Q"))
                {
                    Q.Cast(target, HitChance.High);
                }
                if (E.WillKill(target) && E.IsReady() && KillStealMenu.CheckBoxValue("E"))
                {
                    E.Cast(target, HitChance.High);
                }
            }
        }
    }
}
