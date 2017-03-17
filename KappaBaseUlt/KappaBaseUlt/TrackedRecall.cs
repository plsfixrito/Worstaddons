using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace KappaBaseUlt
{
    public class TrackedRecall
    {
        public AIHeroClient Caster;
        public Vector3 EndPosition;

        public Vector3 CastPosition(Baseult spell, Obj_AI_Base source)
        {
            if (spell.SkillType != null && spell.SkillType == SkillShotType.Circular)
            {
                if (source.Distance(this.EndPosition) > spell.CurrentRange)
                {
                    if (this.EndPosition.IsInRange(source, spell.CurrentRange + (spell.Width / 2f)))
                    {
                        var modRange = source.Distance(this.EndPosition) - spell.Range;
                        return this.EndPosition.Extend(source, modRange).To3DWorld();
                    }
                }
            }

            return this.EndPosition;
        }
        public float StartTick;
        public float EndTick => this.RecallDuration + this.StartTick;
        public float RecallDuration;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => Core.GameTickCount - this.StartTick;
        public bool Ulted;
        public bool Ended => 0 > this.TicksLeft || Core.GameTickCount > this.EndTick;
    }
}
