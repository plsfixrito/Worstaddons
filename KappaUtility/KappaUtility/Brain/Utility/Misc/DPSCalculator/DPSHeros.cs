using EloBuddy;

namespace KappaUtility.Brain.Utility.Misc.DPSCalculator
{
    internal class MyDPS
    {
        public float DamageOnHeros;
        public float DPSOnHeros { get { return this.DamageOnHeros / (Game.Time - main.GameStartTime); } }
        public float DamageOnMobs;
        public float DPSOnMobs { get { return this.DamageOnMobs / (Game.Time - main.GameStartTime); } }
        public float TotalDamage { get { return this.DamageOnHeros + this.DamageOnMobs; } }
        public float TotalDPS { get { return this.TotalDamage / (Game.Time - main.GameStartTime); } }
    }

    internal class EnemyDPS
    {
        public EnemyDPS(AIHeroClient hero)
        {
            this.Hero = hero;
        }
        public AIHeroClient Hero;
        public float DamageOnMe;
        public float DPSOnMe { get { return this.DamageOnMe / (Game.Time - main.GameStartTime); } }
    }

    internal class MobsDPS
    {
        public float DamageOnMe;
        public float DPSOnMe { get { return this.DamageOnMe / (Game.Time - main.GameStartTime); } }
    }
}
