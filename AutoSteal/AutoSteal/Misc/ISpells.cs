using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Spells;

namespace AutoSteal.Misc
{
    internal class ISpells
    {
        public Spell.Skillshot Skillshot;
        public SpellInfo Info;
        public ISpells(Spell.Skillshot skillshot, SpellInfo info)
        {
            this.Skillshot = skillshot;
            this.Info = info;
        }

        public static class Cast
        {
            private static readonly AIHeroClient player = Player.Instance;
            private static float LastCasted;

            public static void On(ISpells spell, Obj_AI_Base target)
            {
                var pred = spell.Skillshot.GetPrediction(target);
                
                if (pred.HitChance < HitChance.Medium || target == null)
                    return;

                if (spell.Info.Chargeable)
                {
                    Orbwalker.DisableAttacking = player.Spellbook.IsChanneling;
                    Orbwalker.DisableMovement = player.Spellbook.IsChanneling;

                    if (player.Spellbook.IsChanneling || player.Spellbook.IsCharging)
                    {
                        if (Core.GameTickCount - LastCasted > 1500)
                        {
                            {
                                spell.Skillshot.Cast(pred.CastPosition);
                            }
                        }
                        else
                        {
                            spell.Skillshot.Cast(target);
                            LastCasted = Core.GameTickCount;
                        }
                        return;
                    }
                }

                if (player.Hero == Champion.Viktor && spell.Skillshot.Slot == SpellSlot.E)
                {
                    spell.Skillshot.CastStartToEnd(target.ServerPosition, player.ServerPosition.Extend(target, 525).To3D());
                    return;
                }
                
                spell.Skillshot.Cast(target);
            }
        }
    }
}
