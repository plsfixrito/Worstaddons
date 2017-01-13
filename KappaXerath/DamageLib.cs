namespace KappaXerath
{
    using EloBuddy;
    using EloBuddy.SDK;

    internal static class DamageLib
    {
        public static float GetDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            var sLevel = spell.Level - 1;
            var ap = Player.Instance.TotalMagicalDamage;
            var dmg = 0f;

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    {
                        dmg += new float[] { 80, 120, 160, 200, 240 }[sLevel] + 0.75f * ap;
                    }
                    break;
                case SpellSlot.W:
                    {
                        dmg += new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.6f * ap;
                    }
                    break;
                case SpellSlot.E:
                    {
                        dmg += new float[] { 80, 110, 140, 170, 200 }[sLevel] + 0.45f * ap;
                    }
                    break;
                case SpellSlot.R:
                    {
                        dmg += new float[] { 200, 230, 260 }[sLevel] + 0.43f * ap;
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, dmg);
        }
    }
}