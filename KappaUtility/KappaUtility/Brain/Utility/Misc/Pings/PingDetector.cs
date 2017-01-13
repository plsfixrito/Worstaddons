using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Misc.Pings
{
    internal class PingDetector
    {
        private static Menu menu;

        private static Text pingtext = new Text("", new Font("calibri", 15, FontStyle.Regular)) { Color = Color.AliceBlue };

        internal static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("PingsDetector");
                menu.AddGroupLabel("PingsDetector");
                menu.CreateCheckBox("enable", "Draw Ping Sender Name", false);
                menu.AddSeparator(0);

                menu.AddGroupLabel("PingsBlocker");
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    menu.CreateCheckBox(ally.Name(), "Block Pings from " + ally.Name(), false);
                }

                TacticalMap.OnPing += TacticalMap_OnPing;
                Game.OnTick += delegate { PingInfo.DetectedPings.RemoveAll(p => Core.GameTickCount - p.StartTick > 2000); };
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Misc.Pings.PingDetector.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(!menu.CheckBoxValue("enable"))
                return;

            foreach (var ping in PingInfo.DetectedPings)
            {
                var sender = ping.Info.Source as AIHeroClient;
                if (sender != null)
                {
                    var msg = sender.Name();
                    var pos = ping.Info.Position.To3DWorld().WorldToScreen();
                    pingtext.Draw(msg, pingtext.Color, pos);
                }
            }
        }

        private static void TacticalMap_OnPing(TacticalMapPingEventArgs args)
        {
            var hero = args.Source as AIHeroClient;
            if(hero == null)
                return;

            if (menu.CheckBoxValue(hero.Name()))
            {
                args.Process = false;
                return;
            }

            PingInfo.DetectedPings.Add(new PingInfo(args, Core.GameTickCount));
        }
    }
}
