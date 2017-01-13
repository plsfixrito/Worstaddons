using EloBuddy;

namespace GenesisUrgot
{
    internal class LevelManager
    {
        public static void LevelUp()
        {
            var points = Player.Instance.SpellTrainingPoints;
            int[] skillPath = { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            while (points >= 1)
            {
                var skill = skillPath[Player.Instance.Level - points];
                switch (skill)
                {
                    case 1:
                        Player.LevelSpell(SpellSlot.Q);
                        break;
                    case 2:
                        Player.LevelSpell(SpellSlot.W);
                        break;
                    case 3:
                        Player.LevelSpell(SpellSlot.E);
                        break;
                    case 4:
                        Player.LevelSpell(SpellSlot.R);
                        break;
                }
                points--;
            }
        }

        public static void Initialize()
        {
        }
    }
}
