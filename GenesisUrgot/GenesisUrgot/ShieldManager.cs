using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using Settings = GenesisUrgot.Config.Modes.ShieldManager;

namespace GenesisUrgot
{
    internal class ShieldManager
    {
        internal static void Events_OnIncomingDamage(Events.InComingDamageEventArgs args)
        {
            if (args.Target == null || !args.Target.IsMe)
                return;
            var random = new Random();
            var randomNumber = Settings.UseHumanizer ? random.Next(10, 100) : 0;
            Core.DelayAction(delegate { SpellManager.W.Cast(); }, randomNumber);
        }

        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var random = new Random();
            int randomNumber;
            if (sender.IsAlly || !Settings.UseW)
                return;
            if (args.Target == null)
            {
                /*Logger.Debug("Seriously WTF!"); 
                Logger.Debug("WTF? args.SData.Name: " + args.SData.Name);
                Logger.Debug("WTF? sender.Distance(args.End): " + sender.Distance(args.End));
                Logger.Debug("WTF? Start: " + args.Start);
                Logger.Debug("WTF? Player.Instance.ServerPosition: " + Player.Instance.ServerPosition);
                Logger.Debug("WTF? PredictLinearMissile: " + PredictLinearMissile(Player.Instance, sender.Distance(args.End), 300, 0, 1000, 2, args.Start, true).HitChancePercent);
                Logger.Debug("WTF? PredictLinearMissile > 11: " + (PredictLinearMissile(Player.Instance, sender.Distance(args.End), 300, 0, 1000, 2, args.Start, true)
                        .HitChancePercent > 15));*/
                if (args.End.IsInRange(Player.Instance.ServerPosition, 100)
                    || (Prediction.Position.PredictLinearMissile(Player.Instance, sender.Distance(args.End), 300, 0, 1000, 2, args.Start, true).HitChancePercent > 15))
                {
                    randomNumber = Settings.UseHumanizer ? random.Next(10, 100) : 0;
                    Core.DelayAction(delegate { SpellManager.W.Cast(); }, randomNumber);
                }
            }

            if (args.Target == null || sender.IsMinion || !args.Target.IsMe)
            {
                return;
            }
            var hero = sender as AIHeroClient;
            if (hero == null)
                return;
            if (args.IsAutoAttack())
            {
                if (hero.GetAutoAttackDamage(Player.Instance) < Player.Instance.Health / 16)
                    return;
            }
            else
            {
                if (hero.GetSpellDamage(Player.Instance, args.Slot) < Player.Instance.Health / 16)
                    return;
            }
            randomNumber = Settings.UseHumanizer ? random.Next(10, 100) : 0;
            Core.DelayAction(delegate { SpellManager.W.Cast(); }, randomNumber);
        }
    }
}
