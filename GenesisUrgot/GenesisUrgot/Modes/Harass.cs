using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
// Using the config like this makes your life easier, trust me
using Settings = GenesisUrgot.Config.Modes.Harass;

namespace GenesisUrgot.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on harass mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Player.Instance.ManaPercent < 25)
                return;
            // TODO: Add harass logic here
            // See how I used the Settings.UseQ and Settings.Mana here, this is why I love
            // my way of using the menu in the Config class!
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
                    EntityManager.Heroes.Enemies.Where(hero => hero.Distance(Player.Instance) < 1200 && hero.HasBuff("urgotcorrosivedebuff")).OrderBy(TargetSelector.GetPriority).FirstOrDefault();
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
        }
    }
}
