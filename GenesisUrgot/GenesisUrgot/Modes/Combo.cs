using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
// Using the config like this makes your life easier, trust me
using Settings = GenesisUrgot.Config.Modes.Combo;

namespace GenesisUrgot.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            if (Settings.UseE)
            {
                var target = TargetSelector.GetTarget(this.E.Range, DamageType.Physical);
                if (target != null)
                {
                    var pred = this.E.GetPrediction(target);
                    if (pred.HitChancePercent >= 80)
                    {
                        this.E.Cast(target);
                    }
                }
            }
            if (Settings.UseQ && this.Q1.IsReady())
            {
                //EntityManager.Heroes.Enemies.ForEach(e => e.Buffs.ForEach(b=>Logger.Debug(b.Name)));
                var target =
                    EntityManager.Heroes.Enemies.Where(hero => hero.Distance(Player.Instance) < 1200 && hero.HasBuff("urgotcorrosivedebuff"))
                        .OrderByDescending(TargetSelector.GetPriority)
                        .FirstOrDefault();
                if (target != null)
                {
                    this.Q2.Cast(target);
                }
                else
                {
                    target = TargetSelector.GetTarget(this.Q1.Range, DamageType.Physical);
                    if (target != null)
                    {
                        this.Q1.Cast(target);
                    }
                }
            }
            if (Settings.UseR && this.R.IsReady())
            {
                var target = TargetSelector.GetTarget(this.R.Range, DamageType.Mixed);
                if (target == null)
                    return;
                if (EntityManager.Heroes.Allies.Count(o => o.IsValidTarget(o.GetAutoAttackRange())) > EntityManager.Heroes.Enemies.Count(o => o.IsValidTarget(o.GetAutoAttackRange()))
                    && !target.IsUnderHisturret())
                {
                    //Should I ever use R during combo?
                    this.R.Cast(target);
                }
            }
        }
    }
}
