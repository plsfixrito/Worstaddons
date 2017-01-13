using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class balls
    {
        private static string ballName;
        private static SpellSlot ballslot;
        private static Spell.Skillshot snowball;

        internal static void Init()
        {
            try
            {
                var poroking = Player.Spells.FirstOrDefault(s => s.Name.Equals("SummonerPoroThrow"));
                if (poroking != null)
                {
                    ballName = poroking.Name;
                    ballslot = poroking.Slot;
                }
                if (Mark.IsVaild())
                {
                    ballName = Mark.Name;
                    ballslot = Mark.Slot;
                }
                snowball = new Spell.Skillshot(ballslot, (uint)(poroking != null ? 2000 : 1600), SkillShotType.Linear, 0, 1000, 60) { DamageType = DamageType.True, AllowedCollisionCount = 0 };
                Summs.menu.AddGroupLabel("SnowBall Settings");
                Summs.menu.CreateCheckBox(ballName, "Use SnowBall");
                Summs.menu.AddSeparator(5);

                Game.OnTick += Game_OnTick;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Borrier.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            var target = snowball.GetTarget();
            if (Summs.menu.CheckBoxValue(ballName) && snowball.IsReady() && target != null && target.IsKillable(snowball.Range) && snowball.Name.Equals(ballName))
            {
                snowball.Cast(target, 45);
            }
        }
    }
}
