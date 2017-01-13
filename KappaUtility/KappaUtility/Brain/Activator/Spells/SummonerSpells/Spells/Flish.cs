using System;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Misc;
using static EloBuddy.SDK.Spells.SummonerSpells;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Flish
    {
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Flash Settings");
                Summs.menu.CreateCheckBox("Flash", "Extend Flash");
                Summs.menu.CreateSlider("range", "Extended Flash Range", 425, 1, 450);
                Summs.menu.AddSeparator(5);
                Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Flish.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe || args.Slot != Flash.Slot || !Summs.menu.CheckBoxValue("Flash"))
                return;

            if (args.EndPosition.Distance(args.StartPosition) < Summs.menu.SliderValue("range"))
            {
                args.Process = false;
            }
            else
            {
                Player.CastSpell(Flash.Slot, Player.Instance.ServerPosition.Extend(args.EndPosition, Summs.menu.SliderValue("range")).To3D());
            }
        }
    }
}
