using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static KappaKled.Program;

namespace KappaKled.Modes
{
    class Harass
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null || !target.IsKillable(Q.Range)) return;

            if (Q.IsReady() && ComboMenu.CheckBoxValue("Q"))
            {
                Q.Cast(target, HitChance.Medium);
            }

            if (E.IsReady() && target.IsKillable(E.Range) && ComboMenu.CheckBoxValue("E"))
            {
                E.Cast(target, HitChance.Medium);
            }
        }
    }
}
