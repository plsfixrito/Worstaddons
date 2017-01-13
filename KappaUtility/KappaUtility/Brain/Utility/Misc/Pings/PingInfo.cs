using System.Collections.Generic;
using EloBuddy;

namespace KappaUtility.Brain.Utility.Misc.Pings
{
    internal class PingInfo
    {
        public static List<PingInfo> DetectedPings = new List<PingInfo>();
        public TacticalMapPingEventArgs Info;
        public float StartTick;

        public PingInfo(TacticalMapPingEventArgs info, float starttick)
        {
            this.Info = info;
            this.StartTick = starttick;
        }
    }
}
