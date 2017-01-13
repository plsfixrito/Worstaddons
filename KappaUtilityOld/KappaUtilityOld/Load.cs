using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaUtilityOld.Items;
using KappaUtilityOld.Misc;
using KappaUtilityOld.Summoners;
using KappaUtilityOld.Trackers;

namespace KappaUtilityOld
{
    internal class Load
    {
        protected static bool loadedreveal = false;

        protected static bool loadedtrack = false;

        public static Menu UtliMenu;

        public static void Loading_OnLoadingComplete(EventArgs eventArgs)
        {
            UtliMenu = MainMenu.AddMenu("KappaUtilityOld", "KappaUtilityOld");
            UtliMenu.AddGroupLabel("Global Settings [Must F5 To Take Effect]");
            UtliMenu.Add("AutoLvlUp", new CheckBox("Enable AutoLvlUp"));
            UtliMenu.Add("AutoQSS", new CheckBox("Enable AutoQSS"));
            UtliMenu.Add("AutoTear", new CheckBox("Enable AutoTear"));
            UtliMenu.Add("AutoReveal", new CheckBox("Enable AutoReveal"));
            UtliMenu.Add("GanksDetector", new CheckBox("Enable GanksDetector"));
            UtliMenu.Add("Tracker", new CheckBox("Enable Tracker"));
            UtliMenu.Add("SkinHax", new CheckBox("Enable SkinHax"));
            UtliMenu.Add("Spells", new CheckBox("Enable SummonerSpells"));
            UtliMenu.Add("Potions", new CheckBox("Enable Potions"));
            UtliMenu.Add("Offensive", new CheckBox("Enable Offensive Items"));
            UtliMenu.Add("Defensive", new CheckBox("Enable Defensive Items"));
            if (UtliMenu["AutoLvlUp"].Cast<CheckBox>().CurrentValue)
            {
                AutoLvlUp.OnLoad();
            }
            if (UtliMenu["AutoQSS"].Cast<CheckBox>().CurrentValue)
            {
                AutoQSS.OnLoad();
            }
            if (UtliMenu["AutoTear"].Cast<CheckBox>().CurrentValue)
            {
                AutoTear.OnLoad();
            }
            if (UtliMenu["AutoReveal"].Cast<CheckBox>().CurrentValue)
            {
                AutoReveal.OnLoad();
                loadedreveal = true;
            }
            if (UtliMenu["GanksDetector"].Cast<CheckBox>().CurrentValue)
            {
                GanksDetector.OnLoad();
            }
            if (UtliMenu["Tracker"].Cast<CheckBox>().CurrentValue)
            {
                Tracker.OnLoad();
                Surrender.OnLoad();
                loadedtrack = true;
            }
            if (UtliMenu["SkinHax"].Cast<CheckBox>().CurrentValue)
            {
                SkinHax.OnLoad();
            }
            if (UtliMenu["Spells"].Cast<CheckBox>().CurrentValue)
            {
                Spells.OnLoad();
                Flash.FOnLoad();
            }
            if (UtliMenu["Potions"].Cast<CheckBox>().CurrentValue)
            {
                Potions.OnLoad();
            }
            if (UtliMenu["Offensive"].Cast<CheckBox>().CurrentValue)
            {
                Offensive.OnLoad();
            }
            if (UtliMenu["Defensive"].Cast<CheckBox>().CurrentValue)
            {
                Defensive.OnLoad();
            }

            Game.OnTick += GameOnTick;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += DrawingOnDraw;
        }

        private static void DrawingOnDraw(EventArgs args)
        {
            try
            {
                Spells.Drawings();
                GanksDetector.OnDraw();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            try
            {
                if (loadedtrack)
                {
                    Traps.Draw();
                    Tracker.HPtrack();
                    Tracker.track();
                }
                GanksDetector.OnEndScene();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void GameOnTick(EventArgs args)
        {
            try
            {
                var flags = Orbwalker.ActiveModesFlags;
                if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Offensive.Items();
                    Defensive.Items();
                }

                if (loadedreveal)
                {
                    AutoReveal.Reveal();
                }

                AutoLvlUp.Levelup();
                AutoTear.OnUpdate();
                GanksDetector.OnUpdate();
                Smite.Smiteopepi();
                Spells.Cast();
                AutoReveal.OnTick();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}