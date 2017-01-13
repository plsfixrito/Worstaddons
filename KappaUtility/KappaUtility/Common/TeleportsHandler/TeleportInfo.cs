using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace KappaUtility.Common.TeleportsHandler
{
    internal class TeleportInfo
    {
        public TeleportInfo(AIHeroClient sender, Teleport.TeleportEventArgs args)
        {
            this.Sender = sender;
            this.Args = args;
            this.StartTick = Core.GameTickCount;
            this.EndTick = this.Args.Duration + this.StartTick;
            this.Duration = this.EndTick - this.StartTick;
        }
        public AIHeroClient Sender;
        public Teleport.TeleportEventArgs Args;
        public Obj_AI_Base EndTarget { get; set; }
        public float Duration;
        public float StartTick;
        public float EndTick;
        public float TimeLeft { get { return this.EndTick - Core.GameTickCount; } }
        public float CurrentPercent { get { return this.TimeLeft / this.Duration * 100; } }
        public float PercentInReverce { get { return 1 * (this.CurrentPercent / 100); } }
        public bool Ended { get { return this.TimeLeft < 1; } }
    }
}
