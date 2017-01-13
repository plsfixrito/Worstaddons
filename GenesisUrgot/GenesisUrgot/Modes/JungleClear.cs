using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace GenesisUrgot.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on jungleclear mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            var jungleMinion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, 500).OrderByDescending(m => m.Health).FirstOrDefault();
            if (jungleMinion == null)
                return;
            this.E.Cast(jungleMinion);
            this.Q1.Cast(jungleMinion);

            if (jungleMinion.Health > Player.Instance.FlatPhysicalDamageMod * 2)
            {
                this.W.Cast();
            }
        }
    }
}
