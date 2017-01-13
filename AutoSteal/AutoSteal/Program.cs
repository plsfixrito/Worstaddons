using System;
using System.Collections.Generic;
using System.Linq;
using AutoSteal.Misc;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Spells;
using Spell = EloBuddy.SDK.Spell;

namespace AutoSteal
{
    public class Program
    {
        public static Menu MenuIni, KillStealMenu, JungleStealMenu;
        private static readonly HashSet<ISpells> Spells = new HashSet<ISpells>();

        private static void Main()
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        public static void OnLoad(EventArgs args)
        {
            switch (Game.MapId)
            {
                    case GameMapId.SummonersRift:
                    Common.JungleMobsNames = Common.SRJungleMobsNames;
                    break;
                case GameMapId.CrystalScar:
                    Common.JungleMobsNames = Common.ASCJungleMobsNames;
                    break;
                case GameMapId.TwistedTreeline:
                    Common.JungleMobsNames = Common.TTJungleMobsNames;
                    break;
            }

            var spells = SpellDatabase.GetSpellInfoList(Player.Instance.BaseSkinName);

            if(spells.Count == 0) return;

            foreach (var spell in spells)
            {
                var skillshot = new Spell.Skillshot(spell.Slot, (uint)spell.Range, Common.type(spell.Type), (int)spell.Delay, (int)spell.MissileSpeed);
                var ispell = new ISpells(skillshot, spell);
                Spells.Add(ispell);
            }

            MenuIni = MainMenu.AddMenu("Auto Steal " + Player.Instance.Hero, "Auto Steal " + Player.Instance.Hero);
            KillStealMenu = MenuIni.AddSubMenu("Kill Steal ", "Kill Steal");
            JungleStealMenu = MenuIni.AddSubMenu("Jungle Steal ", "Jungle Steal");

            KillStealMenu.AddGroupLabel("Spells");
            foreach (var spell in Spells.Select(s => s.Skillshot))
            {
                KillStealMenu.CreateCheckBox(spell.Slot.ToString(), "Use " + spell.Slot);
            }

            KillStealMenu.AddGroupLabel("Enemies");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                KillStealMenu.CreateCheckBox(enemy.Name(), "KS " + enemy.Name());
            }

            JungleStealMenu.AddGroupLabel("Spells");
            foreach (var spell in Spells.Select(s => s.Skillshot))
            {
                JungleStealMenu.CreateCheckBox(spell.Slot.ToString(), "Use " + spell.Slot);
            }

            JungleStealMenu.AddGroupLabel("Mobs");
            foreach (var name in Common.JungleMobsNames)
            {
                JungleStealMenu.CreateCheckBox(name, "JS " + name);
            }
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var spell in Spells)
            {
                foreach (var mob in Common.SupportedJungleMobs.Where(m => m.IsKillable(spell.Skillshot.Range) && JungleStealMenu.CheckBoxValue(spell.Skillshot.Slot.ToString()) && JungleStealMenu.CheckBoxValue(m.BaseSkinName) && spell.Skillshot.IsReady() && spell.Skillshot.WillKill(m)))
                {
                    ISpells.Cast.On(spell, mob);
                }

                foreach (var target in EntityManager.Heroes.Enemies.Where(m => m.IsKillable(spell.Skillshot.Range) && KillStealMenu.CheckBoxValue(spell.Skillshot.Slot.ToString()) && KillStealMenu.CheckBoxValue(m.Name()) && spell.Skillshot.IsReady() && spell.Skillshot.WillKill(m)))
                {
                    ISpells.Cast.On(spell, target);
                }
            }
        }
    }
}
