using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace KappaBaseUlt
{
    public class Baseult
    {
        public Baseult(SpellSlot slot, DamageType damageType, float range, float width, float castDelay, float speed)
        {
            this.Slot = slot;
            this.DamageType = damageType;
            this.Range = range;
            this.Width = width;
            this.CastDelay = castDelay;
            this.Speed = speed;
        }
        
        public SpellSlot Slot;
        public DamageType DamageType;
        public SkillShotType? SkillType;
        public string NameCheck;
        public float Range;
        public float CastDelay;
        public float Speed;
        public float Width;
        public float? RangeGrow;
        public float CurrentRange => this.RangeGrow == null ? this.Range : this.RangeGrow.Value * Player.Instance.Spellbook.GetSpell(this.Slot).Level + this.Range;
        public int AllowedCollisionCount = 0;
        public delegate float RawSpellDamage();
        public RawSpellDamage RawDamage;
        public float Damage => this.RawDamage();

        public bool IsInRange(Vector3 pos)
        {
            if (this.Range >= int.MaxValue)
                return true;
            
            return Player.Instance.IsInRange(pos, this.CurrentRange);
        }

        public bool Cast(Vector3 pos)
        {
            var spell = Player.Instance.Spellbook.GetSpell(this.Slot);
            if (!string.IsNullOrEmpty(NameCheck) && spell.SData.Name != NameCheck)
                return false;

            if (!spell.IsLearned || !spell.IsReady)
                return false;
            
            return Player.CastSpell(this.Slot, pos);
        }

        public float TravelTime(Vector3 pos)
        {
            if (this.Speed >= int.MaxValue)
                return this.CastDelay;

            return Player.Instance.Distance(pos) / this.Speed * 1000 + this.CastDelay;
        }

        public float CalculateDamage(Obj_AI_Base source, Obj_AI_Base target)
        {
            if (target == null)
                return -1f;

            var rawDamage = this.Damage;

            if (source.BaseSkinName.Equals("Jinx"))
            {
                rawDamage += (0.2f + 0.05f * source.Spellbook.GetSpell(this.Slot).Level) * (target.MaxHealth - target.Health);
            }

            return source.CalculateDamageOnUnit(target, DamageType, rawDamage);
        }
    }
}