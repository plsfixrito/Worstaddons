using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static KappaKled.Program;
using static KappaKled.Extentions;

namespace KappaKled.Modes
{
    class Combo
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var selected = TargetSelector.SelectedTarget;
            if (selected != null)
            {
                target = selected;
            }

            if (target == null || !target.IsKillable(Q.Range)) return;

            if (Q.IsReady() && ComboMenu.CheckBoxValue("Q"))
            {
                if (ComboMenu.CheckBoxValue("selectedQ") && State.MyCurrentState(State.Current.Skaarl))
                {
                    if(selected != null)
                        Q.Cast(selected, HitChance.Medium);
                }
                else
                {
                    Q.Cast(target, HitChance.Medium);
                }
            }

            if (E.IsReady() && target.IsKillable(E.Range) && ComboMenu.CheckBoxValue("E"))
            {
                if (!E2)
                {
                    E.Cast(target, HitChance.Medium);
                }
                else if(E2Target != null)
                {
                    if (ComboMenu.CheckBoxValue("E2mouse"))
                    {
                        if (user.ServerPosition.Extend(E2Target, 450).To3D().IsFacing(Game.CursorPos))
                        {
                            E.Cast(E2Target);
                        }
                    }
                    if (ComboMenu.CheckBoxValue("E2ally"))
                    {
                        if (EntityManager.Heroes.Allies.Any(a => a != null && user.ServerPosition.Extend(E2Target, 450).To3D().IsFacing(a.ServerPosition) && a.IsValidTarget() && !a.IsMe))
                        {
                            E.Cast(E2Target);
                        }
                    }

                    if (!ComboMenu.CheckBoxValue("E2mouse") && !ComboMenu.CheckBoxValue("E2ally"))
                    {
                        E.Cast(E2Target);
                    }
                }
            }
        }
    }
}
