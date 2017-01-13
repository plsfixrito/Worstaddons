using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;
using KappaUtility.Common.Misc.Entities;
using static EloBuddy.SDK.Spells.SummonerSpells;
using static KappaUtility.Common.Misc.Extensions;

namespace KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells
{
    internal class Smote
    {
        private static Text smitestat = new Text("", new Font("calibri", 15, FontStyle.Regular)) { Color = Color.AliceBlue };
        internal static void Init()
        {
            try
            {
                Summs.menu.AddGroupLabel("Smite Settings");
                Summs.menu.CreateCheckBox("SmiteKillsteal", "Smite Killsteal");
                Summs.menu.CreateCheckBox("KillstealSave1", "(Killsteal)Save 1 Smite Stack", false);
                Summs.menu.CreateCheckBox("SmiteCombo", "Smite Combo Mode");
                Summs.menu.CreateCheckBox("ComboSave1", "(Combo)Save 1 Smite Stack", false);
                Summs.menu.CreateCheckBox("SmiteJungle", "Smite Jungle Mobs");
                Summs.menu.CreateCheckBox("JungleSave1", "(Jungle)Save 1 Smite Stack", false);
                Summs.menu.CreateCheckBox("drawsmite", "Draw Smite Status");
                Summs.menu.CreateKeyBind("disable", "Smite Disable Key", false, KeyBind.BindTypes.PressToggle);

                Summs.menu.AddGroupLabel("Jungle Mobs To Use On: ");
                foreach (var Mob in Mobs.JungleMobsNames)
                {
                    Summs.menu.CreateCheckBox(Mob, "Smite " + Mob);
                }
                Summs.menu.AddSeparator(0);
                Summs.menu.AddGroupLabel("Enemies To Use On: ");
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    Summs.menu.CreateCheckBox("smite" + enemy.Name(), "Smite " + enemy.Name(), TargetSelector.GetPriority(enemy) > 1);
                }

                Summs.menu.AddSeparator(5);

                Game.OnTick += Game_OnTick;
                Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Activator.Spells.SummonerSpells.Spells.Smote.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Summs.menu.CheckBoxValue("drawsmite"))
                return;

            var c = !Summs.menu.KeyBindValue("disable") ? Color.AliceBlue : Color.Red;
            var msg = "Smite: " + (!Summs.menu.KeyBindValue("disable") ? "On" : "Off");
            var pos = Player.Instance.Position.WorldToScreen();
            smitestat.Draw(msg, c, pos);
        }

        internal static void Game_OnTick(EventArgs args)
        {
            if (!Smite.IsReady() || Summs.menu.KeyBindValue("disable"))
                return;

            if (((Summs.menu.CheckBoxValue("JungleSave1") && Smite.Handle.Ammo > 1) || !Summs.menu.CheckBoxValue("JungleSave1")) && Summs.menu.CheckBoxValue("SmiteJungle"))
            {
                var killable =
                    Mobs.SupportedJungleMobs.OrderByDescending(m => m.MaxHealth)
                        .FirstOrDefault(
                            m => m.IsKillable(600) && Summs.menu.CheckBoxValue(m.BaseSkinName) && Player.Instance.GetSummonerSpellDamage(m, DamageLibrary.SummonerSpells.Smite) >= m.TotalShieldHealth());

                if (killable != null)
                    Smite.Cast(killable);
            }

            if (Smite.Name.ToLower().Contains("summonersmiteduel"))
                return;

            if (((Summs.menu.CheckBoxValue("KillstealSave1") && Smite.Handle.Ammo > 1) || !Summs.menu.CheckBoxValue("KillstealSave1")) && Summs.menu.CheckBoxValue("SmiteKillsteal"))
            {
                var killable =
                    EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority)
                        .FirstOrDefault(
                            m =>
                            m.IsKillable(600) && Summs.menu.CheckBoxValue("smite" + m.Name()) && Player.Instance.GetSummonerSpellDamage(m, DamageLibrary.SummonerSpells.Smite) >= m.TotalShieldHealth());
                if (killable != null)
                    Smite.Cast(killable);
            }
        }

        public static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if ((Orbwalker.ActiveModesFlags & Orbwalker.ActiveModes.Combo) == 0 || target.Type != GameObjectType.AIHeroClient || Summs.menu.KeyBindValue("disable"))
                return;

            var t = target as AIHeroClient;
            if (t != null && t.IsValidTarget(600) && Smite.IsReady() && Summs.menu.CheckBoxValue("smite" + t.Name()) && Summs.menu.CheckBoxValue("Smite")
                && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (((Summs.menu.CheckBoxValue("ComboSave1") && Smite.Handle.Ammo > 1) || !Summs.menu.CheckBoxValue("ComboSave1")) && Summs.menu.CheckBoxValue("SmiteCombo"))
                {
                    Smite.Cast(t);
                }
            }
        }
    }
}
