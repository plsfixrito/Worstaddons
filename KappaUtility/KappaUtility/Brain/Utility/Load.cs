using System;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;
using static KappaUtility.Common.KappaEvade.Database;

namespace KappaUtility.Brain.Utility
{
    internal class Load
    {
        internal static Menu menu;

        public static void Init()
        {
            try
            {
                menu = MainMenu.AddMenu("KappaUtility", "KappaUtility");
                var activator = menu.CreateCheckBox("Activator", "Enable Activators").CurrentValue;
                var tracker = menu.CreateCheckBox("tracker", "Enable Trackers").CurrentValue;
                var misc = menu.CreateCheckBox("misc", "Enable Miscs").CurrentValue;
                var damagehandler = menu.AddSubMenu("DamageHandler");

                damagehandler.AddGroupLabel("FPS Protection");
                menu.CreateCheckBox("fps", "Enable FPS Protection");

                menu.AddSeparator(5);
                damagehandler.AddGroupLabel("Damage Handler");
                damagehandler.AddGroupLabel("This used as Global Damage Detector for the addon");
                damagehandler.CreateCheckBox("Minions", "Detect Minions Damage", false);
                damagehandler.CreateCheckBox("Heros", "Detect Heros Damage");
                damagehandler.CreateCheckBox("Turrets", "Detect Turrets Damage");
                damagehandler.CreateSlider("Mod", "InComing Damage Modifier {0}%", 100, 0, 200);

                damagehandler.AddSeparator(5);

                damagehandler.AddGroupLabel("Spells Detector");
                damagehandler.CreateCheckBox("unlisted", "Try to detect Unlisted Spells");

                damagehandler.AddSeparator(5);
                damagehandler.AddGroupLabel("Detected Spells");

                var i = 0;
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (SkillShotSpells.SkillShotsSpellsList.Any(s => s.hero.Equals(enemy.Hero)) || TargetedSpells.TargetedSpellsList.Any(s => s.hero.Equals(enemy.Hero)))
                    {
                        damagehandler.AddGroupLabel(enemy.Name() + " Spells");
                        foreach (var spell in SkillShotSpells.SkillShotsSpellsList.Where(s => s.hero.Equals(enemy.Hero)))
                        {
                            damagehandler.CreateCheckBox(spell.slot + enemy.ChampionName, "Detect " + enemy.Name() + " " + spell.slot);
                            i++;
                        }
                        foreach (var spell in TargetedSpells.TargetedSpellsList.Where(s => s.hero.Equals(enemy.Hero)))
                        {
                            damagehandler.CreateCheckBox(spell.slot + enemy.ChampionName, "Detect " + enemy.Name() + " " + spell.slot);
                            i++;
                        }
                        damagehandler.AddSeparator(0);
                    }
                }

                damagehandler.LinkedValues.FirstOrDefault(v => v.Value.DisplayName == "Detected Spells").Value.DisplayName = "Detected Spells: " + i;

                if (activator)
                    Activator.Load.Init();

                if (tracker)
                    Tracker.Load.Init();

                if(misc)
                    Misc.Load.Init();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Init", ex, Logger.LogLevel.Error);
            }
        }

        public static float FPSProtection
        {
            get
            {
                return /*menu.CheckBoxValue("fps") && Game.FPS < 60 ? Game.FPS * 2 : Game.FPS*/ 100;
            }
        }
    }
}
