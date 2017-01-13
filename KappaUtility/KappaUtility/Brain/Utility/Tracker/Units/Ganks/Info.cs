using EloBuddy;

namespace KappaUtility.Brain.Utility.Tracker.Units.Ganks
{
    internal class GankInfo
    {
        public AIHeroClient Sender;
        public float StartTick;
        public float LastPinged;

        public GankInfo(AIHeroClient sender, float starttick)
        {
            this.Sender = sender;
            this.StartTick = starttick;
        }
    }
}
