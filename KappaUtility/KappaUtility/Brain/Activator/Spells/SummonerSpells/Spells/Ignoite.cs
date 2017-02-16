using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Ignoite
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Ignite Settings");
                Summs.menu.CreateCheckBox("Ignite", "Use Ignite KillSteal");
                Summs.menu.AddGroupLabel("Enemies to use Ignite On: ");
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    Summs.menu.CreateCheckBox("ignite" + enemy.Name(), "Ignite " + enemy.Name());
                }
                Summs.menu.AddSeparator(5);

                Game.OnTick += Game_OnTick;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Ignoite.Init", ex, Logger.LogLevel.Error);
            }
        }

        public static void Game_OnTick(EventArgs args)
        {
            if (!Summs.menu.CheckBoxValue("Ignite"))
                return;

            if(!Ignite.IsReady())
                return;

            var targets = EntityManager.Heroes.Enemies.Where(e => e.IsKillable(600) && Player.Instance.GetSummonerSpellDamage(e, DamageLibrary.SummonerSpells.Ignite) >= e.TotalShieldHealth());
            var target = targets.OrderByDescending(t => t.Distance(Player.Instance)).FirstOrDefault(t => Summs.menu.CheckBoxValue("ignite" + t.Name()) && t.Health >= Player.Instance.GetAutoAttackDamage(t));

            if (target != null)
                Ignite.Cast(target);
        }
    }
}
