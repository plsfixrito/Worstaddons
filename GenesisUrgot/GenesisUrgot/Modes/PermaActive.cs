using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = GenesisUrgot.Config.Modes.PermaActive;

namespace GenesisUrgot.Modes
{
    public sealed class PermaActive : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
        }

        public override void Execute()
        {
            /*
            if (Player.Instance.SpellTrainingPoints > 0)
            {
                LevelManager.LevelUp();
            }*/
            var turret = EntityManager.Turrets.Allies.FirstOrDefault(tower => Player.Instance.Distance(tower) < tower.AttackRange - 100 && !tower.IsDead);
            if (turret != null)
            {
                var enemy = EntityManager.Heroes.Enemies.OrderBy(TargetSelector.GetPriority).FirstOrDefault(e => e.Distance(turret) < turret.AttackRange - 100);
                if (enemy != null && Settings.UseR && this.R.IsInRange(enemy))
                {
                    this.R.Cast(enemy);
                }
            }
            if (Config.Modes.LC.UseQ)
                LaneClear.LastHit(this.Q1);

            if (Player.Instance.HasItem(ItemId.Tear_of_the_Goddess) || Player.Instance.HasItem(ItemId.Archangels_Staff) || Player.Instance.HasItem(ItemId.Manamune))
            {
                if (EntityManager.MinionsAndMonsters.EnemyMinions.Count(minion => Player.Instance.Distance(minion) < 1700) > 1
                    || EntityManager.Heroes.Enemies.Count(hero => Player.Instance.Distance(hero) < 1700) > 1 || Environment.TickCount < Program.TearTick || Player.Instance.IsRecalling()
                    || Player.Instance.ManaPercent < 50f || !Settings.StackTearQ)
                    return;
                this.Q1.Cast(Player.Instance.ServerPosition.Extend(Game.CursorPos, Player.Instance.MoveSpeed).To3DWorld());
                Program.TearTick = Environment.TickCount + 4000;
            }
        }
    }
}
