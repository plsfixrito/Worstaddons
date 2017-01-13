using EloBuddy;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Tracker.HUD
{
    class DeathTimers
    {
        public DeathTimers(AIHeroClient hero)
        {
            this.Hero = hero;
            this.DeathTime = hero.DeathTimer();
            this.StartTime = Game.Time;
            this.EndTime = this.StartTime + this.DeathTime;
        }

        public AIHeroClient Hero;
        public float DeathTime;
        public float StartTime;
        public float EndTime;
        public int CurrentTimer { get { return (int)(this.EndTime - Game.Time); } }
    }
}
