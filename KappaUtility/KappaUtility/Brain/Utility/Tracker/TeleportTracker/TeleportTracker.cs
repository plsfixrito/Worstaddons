using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;
using KappaUtility.Common.TeleportsHandler;
using Color = SharpDX.Color;

namespace KappaUtility.Brain.Utility.Tracker.TeleportTracker
{
    internal class TeleportTracker
    {
        private static Menu menu;
        private static float Changed;
        private static Text trackettext = new Text("", new Font("calibri", 15, FontStyle.Regular)) { Color = System.Drawing.Color.AliceBlue };
        private static List<TeleportInfo> DetectedTeleports { get { return TeleportsManager.CurrentTeleports; } }

        public static void Init()
        {
            menu = Utility.Load.menu.AddSubMenu("TeleportsTracker");

            menu.AddGroupLabel("Teleports Tracker");
            menu.CreateCheckBox("enable", "Enable Teleports Tracker");
            menu.CreateCheckBox("chat", "Print Teleports In Chat");

            menu.AddGroupLabel("Allies To Track:");
            EntityManager.Heroes.Allies.ForEach(a => menu.CreateCheckBox(a.Name(), "Track " + a.Name()));
            menu.AddGroupLabel("Enemies To Track:");
            EntityManager.Heroes.Enemies.ForEach(a => menu.CreateCheckBox(a.Name(), "Track " + a.Name()));
            menu.AddSeparator(0);

            menu.AddGroupLabel("Drawings:");
            menu.Add("draw", new CheckBox("Draw Recall Bar"));
            var x = menu.Add("rbx", new Slider("RecallBar X", 0, -200, 200));
            x.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                if (changeArgs.NewValue != changeArgs.OldValue)
                {
                    Recallbar.X2 = changeArgs.NewValue * 10;
                    Changed = Core.GameTickCount;
                }
            };
            Recallbar.X2 = x.CurrentValue * 10;
            var y = menu.Add("rby", new Slider("RecallBar Y", 0, -200, 200));
            y.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                if (changeArgs.NewValue != changeArgs.OldValue)
                {
                    Recallbar.Y2 = changeArgs.NewValue * 10;
                    Changed = Core.GameTickCount;
                }
            };
            Recallbar.Y2 = y.CurrentValue * 10;

            Game.OnTick += Game_OnTick;
            Teleport.OnTeleport += Teleport_OnTeleport;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Core.GameTickCount - Changed < 3000)
            {
                Recallbar.RecallBarDraw(Player.Instance, null);
            }

            foreach (var tp in DetectedTeleports.Where(t => t.Sender != null && menu.CheckBoxValue(t.Sender.Name())))
            {
                var timer = (tp.TimeLeft / 1000).ToString("F1");
                if (tp.Sender != null)
                {
                    var c = Color.GreenYellow;
                    if (tp.Sender.IsEnemy)
                        c = Color.Red;
                    trackettext.Draw($"{tp.Sender.Name()}: {tp.Args.Type} {timer}", c, tp.Sender.ServerPosition.WorldToScreen());
                }
                if (tp.EndTarget != null)
                {
                    var c = Color.GreenYellow;
                    if (tp.EndTarget.IsEnemy)
                        c = Color.Red;
                    trackettext.Draw($"{tp.Sender.Name()}: {tp.Args.Type} {timer}", c, tp.EndTarget.ServerPosition.WorldToScreen());
                }

                if(menu.CheckBoxValue("draw"))
                    Recallbar.RecallBarDraw(tp.Sender, tp);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            DetectedTeleports.RemoveAll(t => t.Ended || t.Sender.IsDead || t.EndTarget != null && t.EndTarget.IsDead);
            foreach (var tp in DetectedTeleports)
            {
                if (tp.EndTarget == null)
                {
                    var endtarget =
                        ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.Buffs.Any(b => b.DisplayName.Equals("Teleport_Target") && b.IsActive && b.IsValid && b.Caster.IdEquals(tp.Sender)));
                    if (endtarget != null)
                        tp.EndTarget = endtarget;
                }
            }
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if(caster == null)
                return;
            
            var info = new TeleportInfo(caster, args);
            if (args.Status == TeleportStatus.Start)
            {
                if (!DetectedTeleports.Contains(info))
                {
                    Print(caster, args);
                }
            }
            else
            {
                Print(caster, args);
            }
        }

        private static void Print(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if(!menu.CheckBoxValue("chat") || !menu.CheckBoxValue(sender.Name()))
                return;

            var NameColor = "<font color='#53ff1a'>";
            if(sender.IsEnemy)
                NameColor = "<font color='#ff0000'>";
            var name = "<b>" + NameColor + sender.Name() + "</font></b>";
            string type;

            switch (args.Type)
            {
                case TeleportType.Shen:
                    type = "Shen Ult";
                    break;
                case TeleportType.TwistedFate:
                    type = "TwistedFate Ult";
                    break;
                case TeleportType.Unknown:
                    type = "Unknown Port Type";
                    break;
                default:
                    type = args.Type.ToString();
                    break;
            }

            string statuscolor;

            switch (args.Status)
            {
                    case TeleportStatus.Start:
                    statuscolor = "<font color='#53ff1a'>";
                    break;
                case TeleportStatus.Abort:
                    statuscolor = "<font color='#ff9933'>";
                    break;
                case TeleportStatus.Finish:
                    statuscolor = "<font color='#ff3333'>";
                    break;
                default:
                    statuscolor = "<font color='#ff3333'>";
                    break;
            }

            var status = statuscolor + args.Status + "ed</font> " + type + " !";

            Chat.Print("<font color='#e6e6e6'>[" + ToTimeSpan(Game.Time) + "]</font> " + name + " " + status);
        }

        private static string ToTimeSpan(double time)
        {
            var t = TimeSpan.FromSeconds(time);
            return $"{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }
}
