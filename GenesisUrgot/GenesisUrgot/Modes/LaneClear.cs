using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

//using EloBuddy.SDK.Utils;

namespace GenesisUrgot.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on laneclear mode
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
            }
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (!Config.Modes.LC.UseQ)
                return;

            LastHit(this.Q1);

            if (EntityManager.MinionsAndMonsters.CombinedAttackable.Any(m => m.Distance(Player.Instance) < this.Q1.Range && m.Health <= 2 * Player.Instance.GetSpellDamage(m, SpellSlot.Q)))
                return;
            //Logger.Debug("Hi, I'm trying to lane clear");
            var laneClear = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.Health > 2.5 * Player.Instance.GetSpellDamage(m, SpellSlot.Q));
            if (laneClear == null || !laneClear.IsValid || laneClear.IsDead)
                return;
            {
                if (this.Q1.GetPrediction(laneClear).HitChancePercent > 25)
                    this.Q1.Cast(laneClear);
            }
        }

        public static void LastHit(Spell.Skillshot Q1)
        {
            if (LastTarget == null)
                return;
            Obj_AI_Base lastHit =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(minion => minion.Distance(Player.Instance) < Q1.Range && Player.Instance.GetSpellDamage(minion, SpellSlot.Q) > (minion.Health))
                    .OrderBy(minion => minion.Health)
                    .FirstOrDefault();

            if (lastHit == null || !lastHit.IsValid || lastHit.IsDead || DontQ || (Player.Instance.IsInAutoAttackRange(lastHit) && Orbwalker.CanAutoAttack)
                || (LastTarget == lastHit && !Orbwalker.CanAutoAttack))
                return;
            if (Q1.GetPrediction(lastHit).HitChancePercent > 25)
                Q1.Cast(lastHit);
        }

        public static Obj_AI_Base LastTarget;
        public static bool DontQ;

        public static void OnAttack(AttackableUnit target, EventArgs args)
        {
            LastTarget = target as Obj_AI_Base;
            DontQ = true;
            Core.DelayAction(delegate { DontQ = false; }, (int)Player.Instance.Distance(target) / 3);
        }
    }
}
