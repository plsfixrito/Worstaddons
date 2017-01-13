using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Tracker.Units.Placements
{
    internal class TrapsTracker
    {
        private static List<Trap> DetectedTraps = new List<Trap>();
        private static List<TrapData> TrapsData = new List<TrapData>();
        private static Menu menu;
        private static bool SpectatorMode;

        public TrapsTracker(bool Spectator = false, Menu Main = null)
        {
            SpectatorMode = Spectator;
            var mainmenu = Spectator ? Main : Utility.Load.menu;
            TrapsData.Add(new TrapData("JhinTrap", "JhinETrap", "Jhin E"));
            TrapsData.Add(new TrapData("TeemoMushroom", "BantamTrap", "Teemo R"));
            TrapsData.Add(new TrapData("ShacoBox", "JackInTheBox", "Shaco W"));
            TrapsData.Add(new TrapData("CaitlynTrap", "CaitlynYordleTrap", "Caitlyn W"));

            if(mainmenu == null)
                return;

            menu = mainmenu.AddSubMenu("Traps Tracker");
            menu.AddGroupLabel("Traps Tracker");
            menu.CreateCheckBox("enable", "Enable");
            menu.AddSeparator(0);

            var blue = string.Format(Spectator ? "{0}" : "{1}", "Blue", "Ally");
            var red = string.Format(Spectator ? "{0}" : "{1}", "Red", "Enemy");

            menu.AddGroupLabel("Teams");
            menu.CreateCheckBox(blue, $"Track {blue} Team Traps");
            menu.CreateCheckBox(red, $"Track {red} Team Traps");
            menu.AddSeparator(0);

            menu.AddGroupLabel("Traps To Track");
            TrapsData.ForEach(t => menu.CreateCheckBox(t.TrapBaseSkinName, $"Track {t.DisplayName}"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(!menu.CheckBoxValue("enable"))
                return;

            foreach (var trap in DetectedTraps.Where(t => menu.CheckBoxValue(t.theTrap.BaseSkinName)
            && (SpectatorMode ? t.theTrap.Team == GameObjectTeam.Order && menu.CheckBoxValue("Blue") || t.theTrap.Team == GameObjectTeam.Chaos && menu.CheckBoxValue("Red")
            : t.IsEnemy && menu.CheckBoxValue("Enemy") || !t.IsEnemy && menu.CheckBoxValue("Ally"))))
            {
                var pos = trap.theTrap.ServerPosition.WorldToScreen();
                trap.theTrap.DrawCircle(100, trap.IsEnemy ? Color.Red : Color.GreenYellow);
                Drawing.DrawText(pos, Color.AliceBlue, $"{trap.DisplayName}: {trap.TimeLeft.ToString("F1")}", 10);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            foreach (var trap in ObjectManager.Get<Obj_AI_Base>().Where(o => TrapsData.Any(d => d.TrapBaseSkinName.Equals(o.BaseSkinName)) && !DetectedTraps.Any(t => t.theTrap.IdEquals(o)) && !o.IsDead && o.IsValid))
            {
                var data = TrapsData.FirstOrDefault(t => t.TrapBaseSkinName.Equals(trap.BaseSkinName));
                if(data != null)
                    DetectedTraps.Add(new Trap(trap, data));
            }
            DetectedTraps.RemoveAll(t => t.Ended || t.theTrap.IsDead || !t.theTrap.IsValid);
        }

        public class Trap
        {
            public Trap(Obj_AI_Base trap, TrapData data)
            {
                this.theTrap = trap;
                this.Data = data;
                this.DisplayName = data.DisplayName;
            }
            public Obj_AI_Base theTrap;
            public TrapData Data;
            public string DisplayName;
            public bool IsEnemy
            {
                get
                {
                    var buff = this.theTrap.GetBuff(this.Data.BuffName);
                    return buff != null && (SpectatorMode ? buff.Caster.Team == GameObjectTeam.Chaos : buff.Caster.IsEnemy);
                }
            }
            public float TimeLeft
            {
                get
                {
                    var buff = this.theTrap.GetBuff(this.Data.BuffName);
                    return buff != null ? this.theTrap.GetBuff(this.Data.BuffName).EndTime - Game.Time : 69;
                }
            }

            public bool Ended { get { return this.TimeLeft <= 0; } }
        }
        public class TrapData
        {
            public TrapData(string name, string buff, string display)
            {
                this.TrapBaseSkinName = name;
                this.BuffName = buff;
                this.DisplayName = display;
            }
            public string TrapBaseSkinName;
            public string BuffName;
            public string DisplayName;
        }
    }
}
