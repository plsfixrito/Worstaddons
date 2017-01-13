using EloBuddy;
using EloBuddy.SDK.Events;
using Settings = GenesisUrgot.Config.Modes.Interrupt;

namespace GenesisUrgot
{
    class InterruptManager
    {
        public static void Initialize()
        {
            // Let the static initializer do the job, this way we avoid multiple init calls aswell
        }

        internal static void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            var target = sender;
            if (target == null)
                return;
            if (!SpellManager.R.IsReady() || !Settings.UseR)
                return;
            if (SpellManager.R.IsInRange(target) && Config.Modes.Combo.UseR)
                SpellManager.R.Cast(target);
        }
    }
}
