using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;
using Extensions = KappaUtility.Common.Misc.Extensions;

namespace KappaUtility.Brain.Utility.Tracker.Units.Ganks
{
    internal static class GanksDetector
    {
        private static Text ganktext = new Text("", new Font("calibri", 15, FontStyle.Regular)) { Color = System.Drawing.Color.AliceBlue };

        private static readonly List<GankInfo> DetectedGanks = new List<GankInfo>();

        private static readonly List<GankInfo> AlreadyDetected = new List<GankInfo>();

        private static Menu menu;

        private static float LastRangeChanged;
        
        internal static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("GanksDetector");

                menu.AddGroupLabel("GanksDetector");
                menu.CreateCheckBox("enable", "Enable GanksDetector");
                menu.CreateCheckBox("ping", "Ping On Incoming Ganks");
                menu.CreateCheckBox("fow", "Detect Ganks From FoW");
                menu.CreateCheckBox("jungler", "Detect Junglers Only", false);
                menu.CreateCheckBox("drawrange", "Preview Changes on Detection Range");
                menu.Add("Allypingtype", new ComboBox("Ally Ping Type", 2, Extensions.Pingtypes));
                menu.Add("Enemypingtype", new ComboBox("Enemy Ping Type", 0, Extensions.Pingtypes));
                menu.Add("pingmode", new ComboBox("Mode", 0, "Local", "Public"));
                
                menu.CreateSlider("range", "Range to Detect Ganks [{0}]", 5000, 1000, 25000).OnValueChange += delegate { LastRangeChanged = Core.GameTickCount; };
                menu.CreateSlider("time", "Time For the Gank To Expire [{0}]Second/s", 5, 1, 15);
                menu.CreateSlider("pingdelay", "Ping Delay [{0}]Second/s", 10, 1, 25);
                menu.CreateSlider("delay", "Ganks Detection Delay [{0}]Second/s", 15, 1, 60);

                menu.AddSeparator(0);

                menu.AddGroupLabel("Allies");
                menu.CreateCheckBox("ally", "Detect Ally Ganks");
                menu.AddLabel("Detects Incoming Ganks from Allies");
                foreach (var hero in EntityManager.Heroes.Allies)
                {
                    if (!hero.IsMe)
                        menu.CreateCheckBox(hero.Name(), "Detect From " + hero.Name() + (hero.IsJungler() ? "(Jungler)" : ""));
                }
                menu.AddSeparator(0);

                menu.AddGroupLabel("Enemies");
                menu.CreateCheckBox("enemy", "Detect Enemy Ganks");
                menu.AddLabel("Detects Incoming Ganks from Enemies");
                foreach (var hero in EntityManager.Heroes.Enemies)
                {
                    menu.CreateCheckBox(hero.Name(), "Detect From " + hero.Name() + (hero.IsJungler() ? "(Jungler)" : ""));
                }

                menu.AddSeparator(5);
                menu.AddGroupLabel("Misc Settings");
                menu.CreateSlider("linewidth", "Line Width", 5, 1, 25);
                menu.CreateSlider("textdistance", "Gank Sender Name Distance", 600, 100, 1000);

                DetectedGanks.Clear();
                AlreadyDetected.Clear();

                Game.OnTick += Game_OnTick;
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Tracker.Ganks.GanksDetector.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static int LineWidth => menu.SliderValue("linewidth");
        private static float SenderNameDistance => menu.SliderValue("textdistance");
        private static float DetectionRange => menu.SliderValue("range");
        private static float ExpireTime => menu.SliderValue("time") * 1000;
        private static float PingDelay => menu.SliderValue("pingdelay") * 1000;
        private static float DetectionDelay => menu.SliderValue("delay") * 1000;

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (menu.CheckBoxValue("drawrange") && 3750 > Core.GameTickCount - LastRangeChanged)
            {
                Circle.Draw(SharpDX.Color.OrangeRed, DetectionRange, 5F, Player.Instance);
            }
            if (!menu.CheckBoxValue("enable"))
                return;

            foreach (var Gank in
                DetectedGanks.Where(
                    g =>
                    g?.Sender != null && !g.Sender.IsMe && menu.CheckBoxValue(g.Sender.Name())))
            {
                if (Gank != null && (menu.CheckBoxValue("jungler") && Gank.Sender.IsJungler() || !menu.CheckBoxValue("jungler"))
                    && ((Gank.Sender.IsEnemy && menu.CheckBoxValue("enemy")) || (Gank.Sender.IsAlly && menu.CheckBoxValue("ally"))))
                {
                    SendPing(Gank);
                    Extensions.DrawLine(Gank.Sender.ServerPosition, Player.Instance.Position, LineWidth, LineColor(Gank));
                    var msg = Gank.Sender.Name();
                    var Position = Player.Instance.ServerPosition.Extend(Gank.Sender.ServerPosition, LineLength(Gank)).To3DWorld().WorldToScreen();
                    var c = Color.White;
                    ganktext.Draw(msg, c, Position);
                }
            }
        }

        private static float lastupdate;
        private static void Game_OnTick(EventArgs args)
        {
            if (!menu.CheckBoxValue("enable"))
                return;

            if (Core.GameTickCount - lastupdate > Utility.Load.FPSProtection)
            {
                foreach (var hero in EntityManager.Heroes.AllHeroes.Where(h => !h.IsMe && h.FoW() && h.IsInRange(Player.Instance, DetectionRange)))
                {
                    var gank = new GankInfo(hero, Core.GameTickCount);
                    if (!DetectedGanks.Any(g => g.Sender.IdEquals(hero)) && !DetectedGanks.Contains(gank))
                    {
                        if (!AlreadyDetected.Any(g => g.Sender.IdEquals(hero)) && !AlreadyDetected.Contains(gank))
                        {
                            DetectedGanks.Add(gank);
                            AlreadyDetected.Add(gank);
                        }
                    }
                }

                DetectedGanks.RemoveAll(g => g == null || g.Sender.IsMe || g.Sender.IsDead || Core.GameTickCount - g.StartTick > ExpireTime);
                AlreadyDetected.RemoveAll(h => h == null || h.Sender.IsMe || h.Sender.IsDead || h.Sender.Distance(Player.Instance) > DetectionRange + h.Sender.BoundingRadius && Core.GameTickCount - h.StartTick > DetectionDelay);
                lastupdate = Core.GameTickCount;
            }
        }

        private static float LineLength(GankInfo Gank)
        {
            return Gank.Sender.ServerPosition.Distance(Player.Instance) > SenderNameDistance ? SenderNameDistance : Gank.Sender.ServerPosition.Distance(Player.Instance);
        }

        private static Color LineColor(GankInfo gank)
        {
            return gank.Sender.IsAlly ? Color.Chartreuse : Color.Red;
        }

        private static void SendPing(GankInfo gank)
        {
            if (gank.Sender.IsMe || PingDelay > Core.GameTickCount - gank.LastPinged || !menu.CheckBoxValue("ping"))
                return;

            var local = menu.ComboBoxValue("pingmode") == 0;

            if (local)
                TacticalMap.ShowPing(pingtype(gank), gank.Sender, true);
            else
                TacticalMap.SendPing(pingtype(gank), gank.Sender);
            gank.LastPinged = Core.GameTickCount;
        }

        private static PingCategory pingtype(GankInfo Gank)
        {
            switch (Gank.Sender.IsAlly ? menu.ComboBoxValue("Allypingtype") : menu.ComboBoxValue("Enemypingtype"))
            {
                case 0:
                    return PingCategory.Danger;
                case 1:
                    return PingCategory.Fallback;
                case 2:
                    return PingCategory.OnMyWay;
                case 3:
                    return PingCategory.AssistMe;
                case 4:
                    return PingCategory.EnemyMissing;
                case 5:
                    return PingCategory.Normal;
                default:
                    return PingCategory.Normal;
            }
        }

        private static bool FoW(this Obj_AI_Base target)
        {
            return target != null && (menu.CheckBoxValue("fow") ? !target.IsDead && target.IsValid : !target.IsDead && target.IsValid && target.IsHPBarRendered);
        }

        private static bool IsJungler(this Obj_AI_Base target)
        {
            return target != null && target.Spellbook.Spells.Any(s => s.Name.ToLower().Contains("summonersmite"));
        }
    }
}
