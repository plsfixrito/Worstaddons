using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtility.Common.Misc;

namespace KappaUtility.Brain.Utility.Misc.DPSCalculator
{
    internal class DPS
    {
        private static MyDPS dps = new MyDPS();
        private static MobsDPS DPSmobs = new MobsDPS();
        private static List<EnemyDPS> EnemiesDPS = new List<EnemyDPS>();

        private static Menu menu;
        public static void Init()
        {
            menu = Utility.Load.menu.AddSubMenu("DPS Calculator");

            menu.AddGroupLabel("DPS Calculator");
            menu.AddLabel("DPS Is Calculated for your Hero Only !");

            menu.CreateCheckBox("enable", "Enable");
            menu.CreateCheckBox("onlyme", "Draw Only me", false);
            menu.AddSeparator(5);
            menu.AddGroupLabel("Drawings:");
            menu.CreateSlider("X", "Text X Offset", 0, -100);
            menu.CreateSlider("Y", "Text Y Offset", 0, -100);
            menu.AddSeparator(5);
            menu.AddGroupLabel("My DPS:");
            menu.CreateCheckBox("enablemydps", "Enable My DPS Calculations");
            menu.CreateCheckBox("DPSOnHeros", "Draw My DPS On Heros");
            menu.CreateCheckBox("DPSOnMobs", "Draw My DPS On Mobs");
            menu.CreateCheckBox("MyDPSTotal", "Draw My Total DPS");
            menu.CreateCheckBox("DamageOnHeros", "Draw My Damage On Heros");
            menu.CreateCheckBox("DamageOnMobs", "Draw My Damage On Mobs");
            menu.CreateCheckBox("MyDamageTotal", "Draw My Total Damage");

            menu.AddSeparator(0);
            menu.AddGroupLabel("Mobs DPS:");
            menu.CreateCheckBox("MobsDPS", "Draw Mobs DPS");
            menu.CreateCheckBox("MobsDamage", "Draw Mobs Damage");
            menu.AddSeparator(0);

            EntityManager.Heroes.Enemies.ForEach(
                e =>
                    {
                        EnemiesDPS.Add(new EnemyDPS(e));
                        menu.AddGroupLabel(e.Name() + ":");
                        menu.CreateCheckBox(e.Name() + "enable", "Enable " + e.Name() + " Calculations");
                        menu.CreateCheckBox(e.Name() + "DPS", "Draw " + e.Name() + " DPS");
                        menu.CreateCheckBox(e.Name() + "Damage", "Draw " + e.Name() + " Damage");
                        menu.AddSeparator(1);
                    });
            
            Drawing.OnDraw += Drawing_OnDraw;
            AttackableUnit.OnDamage += AttackableUnit_OnDamage;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!menu.CheckBoxValue("enable"))
                return;

            string text = "";

            var MyDPS = menu.CheckBoxValue("enablemydps");
            var DPSOnHeros = menu.CheckBoxValue("DPSOnHeros");
            var DPSOnMobs = menu.CheckBoxValue("DPSOnMobs");
            var MyDPSTotal = menu.CheckBoxValue("MyDPSTotal");
            var DamageOnHeros = menu.CheckBoxValue("DamageOnHeros");
            var DamageOnMobs = menu.CheckBoxValue("DamageOnMobs");
            var MyDamageTotal = menu.CheckBoxValue("MyDamageTotal");

            if (MyDPS)
            {
                var any = DPSOnHeros || DPSOnMobs || MyDPSTotal;

                if (any)
                {
                    text += $"-{Player.Instance.Name()}:\n";
                }
                if (DPSOnHeros)
                {
                    text += $" | HDPS: {dps.DPSOnHeros.ToString("F1")}";
                }
                if (DPSOnMobs)
                {
                    text += $" | MDPS: {dps.DPSOnMobs.ToString("F1")}";
                }
                if (MyDPSTotal)
                {
                    text += $" | TDPS: {dps.TotalDPS.ToString("F1")}";
                }
                if (any)
                {
                    text += "\n";
                }

                any = DamageOnHeros || DamageOnMobs || MyDamageTotal;

                if (DamageOnHeros)
                {
                    text += $" | HDMG: {dps.DamageOnHeros.ToString("F1")}";
                }
                if (DamageOnMobs)
                {
                    text += $" | MDMG: {dps.DamageOnMobs.ToString("F1")}";
                }
                if (MyDamageTotal)
                {
                    text += $" | TDMG: {dps.TotalDamage.ToString("F1")}";
                }
                if (any)
                {
                    text += "\n";
                }
            }

            var x = Drawing.Width * 0.045f + menu.SliderValue("X") * 17;
            var y = Drawing.Height * 0.045f + menu.SliderValue("Y") * 17;
            if (menu.CheckBoxValue("onlyme"))
            {
                Drawing.DrawText(x, y, Color.AliceBlue, text);
                return;
            }

            var mobs = menu.CheckBoxValue("MobsDPS") || menu.CheckBoxValue("MobsDamage");
            if (mobs)
            {
                if (DPSmobs.DamageOnMe > 0)
                {
                    text += "-Mobs:\n";
                    if (menu.CheckBoxValue("MobsDPS"))
                    {
                        text += $" | DPS: {DPSmobs.DPSOnMe.ToString("F1")}";
                    }
                    if (menu.CheckBoxValue("MobsDamage"))
                    {
                        text += $" | DMG: {DPSmobs.DamageOnMe.ToString("F1")}";
                    }
                    text += "\n";
                }
            }

            foreach (var enemy in EnemiesDPS.OrderByDescending(a => a.DPSOnMe))
            {
                var hero = enemy.Hero;
                var enabled = menu.CheckBoxValue(hero.Name() + "enable");
                var usedps = menu.CheckBoxValue(hero.Name() + "DPS");
                var usedamage = menu.CheckBoxValue(hero.Name() + "Damage");
                if (menu.CheckBoxValue(hero.Name() + "enable"))
                {
                    if(enabled && enemy.DamageOnMe > 0)
                    {
                        var any = usedps || usedamage;
                        if (any)
                        {
                            text += $"-{hero.Name()}:\n";
                        }
                        if (usedps)
                        {
                            text += $" | DPS: {enemy.DPSOnMe.ToString("F1")}";
                        }
                        if (usedamage)
                        {
                            text += $" | DMG: {enemy.DamageOnMe.ToString("F1")}";
                        }
                        if (any)
                        {
                            text += "\n";
                        }
                    }
                }
            }
            
            Drawing.DrawText(x, y, Color.AliceBlue, text);
        }

        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if(args.Target == null || args.Source == null)
                return;

            if (args.Target.IsMe)
            {
                if (args.Source is Obj_AI_Minion)
                    DPSmobs.DamageOnMe += args.Damage;

                var hero = args.Source as AIHeroClient;
                if (hero != null && hero.IsEnemy)
                {
                    var edps = EnemiesDPS.FirstOrDefault(e => e.Hero.IdEquals(hero));
                    if(edps != null)
                        edps.DamageOnMe += args.Damage;
                }
            }

            if (args.Source.IsMe)
            {
                if (args.Target is AIHeroClient)
                    dps.DamageOnHeros += args.Damage;
                if (args.Target is Obj_AI_Minion)
                    dps.DamageOnMobs += args.Damage;
            }
        }
    }
}
