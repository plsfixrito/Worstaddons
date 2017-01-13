using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;
using KappaUtility.Common.Misc.Entities;
using Extensions = KappaUtility.Common.Misc.Extensions;

namespace KappaUtility.Brain.Utility.Tracker.Units.Placements
{
    internal class WardsTracker
    {
        private static Text wardtext = new Text("", new Font("calibri", 15, FontStyle.Regular)) { Color = Color.AliceBlue };
        private static readonly List<string> WardNames = new List<string> { "SightWard", "VisionWard" };
        private static readonly List<Wards.DetectedWards> Detectedwards = new List<Wards.DetectedWards>();
        private static Menu menu;

        public static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("Wards Tracker");

                menu.AddGroupLabel("Wards Tracker");
                menu.CreateCheckBox("enable", "Enable Wards Tracker", false);
                menu.AddLabel("Tracks Enemy Wards");
                menu.AddLabel("Green Cricle = YellowTrinket / Sight Wards");
                menu.AddLabel("Blue Cricle = Blue Trinket Wards");
                menu.AddLabel("Purple Cricle = Vision Wards");

                Game.OnTick += Game_OnTick;
                GameObject.OnCreate += Obj_AI_Base_OnCreate;
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Tracker.Units.WardsTracker.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static float lastupdate;
        private static void Game_OnTick(EventArgs args)
        {
            if (!menu.CheckBoxValue("enable"))
                return;
            if (Core.GameTickCount - lastupdate > Utility.Load.FPSProtection)
            {
                foreach (var ward in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && WardNames.Contains(m.Name)))
                {
                    WardDetected(ward);
                }
                Detectedwards.RemoveAll(
                    w => (w.EndTime - Game.Time < 1 && !(w.Type.Equals(Wards.WardType.VisionWard) || w.Type.Equals(Wards.WardType.BlueWard))) || (w.Ward != null && (w.Ward.IsDead || w.Ward.Health < 1)));
                lastupdate = Core.GameTickCount;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!menu.CheckBoxValue("enable"))
                return;

            foreach (var ward in Detectedwards)
            {
                Circle.Draw(Wards.color(ward), 125, ward.Position);

                var endtime = (int)(ward.EndTime - Game.Time);
                var msg = endtime.ToString(CultureInfo.InvariantCulture);
                var pos = ward.Position.WorldToScreen();
                if (ward.EndTime > 0)
                    wardtext.Draw(msg, wardtext.Color, pos);
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.IsEnemy)
            {
                if (WardNames.Contains(minion.Name))
                {
                    WardDetected(minion);
                }
            }
        }

        private static void WardDetected(Obj_AI_Minion Detectedward)
        {
            if (Detectedward.IsDead || Detectedward.Health < 1)
                return;

            var buff = Detectedward.Buffs.Find(b => b.Name == "sharedwardbuff");
            var endtime = 0;
            if (buff != null)
            {
                endtime = (int)buff.EndTime;
            }

            var newward = new Wards.DetectedWards(Detectedward, Detectedward.ServerPosition, Detectedward.Type(), (int)Game.Time, endtime);
            if (Detectedwards.All(w => w.Position != newward.Position))
                Detectedwards.Add(newward);
        }
    }
}
