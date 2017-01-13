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
using KappaUtility.Common.TeleportsHandler;
using KappaUtility.Common.Texture;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaUtility.Brain.Utility.Tracker.HUD
{
    internal class HUDTracker
    {
        private static Text DeathTimerText;
        private static Text CDText;
        private static Text Levelext;
        private static List<DeathTimers> DeathTimes = new List<DeathTimers>();

        private static bool Spectating;

        private static bool Enabled { get { return menu.CheckBoxValue("enable"); } }

        private static bool DrawBlue { get { return menu.CheckBoxValue(Spectating ? "blue" : "ally"); } }
        private static bool DrawRed { get { return menu.CheckBoxValue(Spectating ? "red" : "enemy"); } }

        private static bool DrawXP { get { return menu.CheckBoxValue("XP"); } }
        private static bool DrawHP { get { return menu.CheckBoxValue("HP"); } }
        private static bool DrawMP { get { return menu.CheckBoxValue("MP"); } }

        private static bool DrawLevel { get { return menu.CheckBoxValue("level"); } }
        private static bool DrawDeath { get { return menu.CheckBoxValue("death"); } }
        
        private static bool DrawCD { get { return menu.CheckBoxValue("cd"); } }

        private static bool TrackTeleports { get { return menu.CheckBoxValue("tp"); } }

        public static float HUDSize { get { return menu.SliderValue("size") * 0.01f; } }

        public static Menu menu;

        public HUDTracker(bool Spectator = false, Menu mainMenu = null)
        {
            Spectating = Spectator;
            menu = Spectator ? mainMenu : Utility.Load.menu.AddSubMenu("HUD Tracker");

            if (menu == null)
            {
                Logger.Send("HUDTracker: ERROR While Loading Spectator Mode", Logger.LogLevel.Error);
                return;
            }

            menu.AddGroupLabel("Options");
            menu.CreateCheckBox("enable", "Enable");

            var blue = string.Format(Spectator ? "{0}" : "{1}", "Blue", "Ally");
            var red = string.Format(Spectator ? "{0}" : "{1}", "Red", "Enemy");
            menu.AddGroupLabel("Sides");
            menu.Add("blueside", new ComboBox($"{blue} Team Side", 0, "Left Side", "Right Side"));
            menu.Add("redside", new ComboBox($"{red} Team Side", 1, "Left Side", "Right Side"));

            menu.AddSeparator(0);
            menu.AddGroupLabel("Teams");
            menu.CreateCheckBox(Spectator ? "blue" : "ally", $"Draw {blue} Side");
            menu.CreateCheckBox(Spectator ? "red" : "enemy", $"Draw {red} Side");

            menu.AddSeparator(0);
            menu.AddGroupLabel("Health, Mana and Experience");
            menu.CreateCheckBox("XP", "Show Hero XP (Experience)");
            menu.CreateCheckBox("HP", "Show Hero HP (Health)");
            menu.CreateCheckBox("MP", "Show Hero MP (Mana)");

            menu.AddSeparator(0);
            menu.AddGroupLabel("Hero stats");
            menu.CreateCheckBox("level", "Show Hero Level");
            menu.CreateCheckBox("death", "Show Hero Death Timer");
            menu.CreateCheckBox("tp", "Track Hero Teleports");

            menu.AddSeparator(0);
            menu.AddGroupLabel("Spells");
            menu.CreateCheckBox("cd", "Show Hero Spells CD (CoolDown)");

            menu.AddSeparator(0);
            menu.AddGroupLabel("Drawings Options");
            var height = Drawing.Height;
            var width = Drawing.Width;
            menu.Add("size", new Slider("HUD Size {0}% (Require Reload After Change)", 100, 0, 125));
            menu.Add("space", new Slider("Spacing", 25, 0, (int)(height * 0.1f)));
            menu.AddLabel("Left Side");
            menu.Add("leftX", new Slider("Left Side X Position", 0, -(int)(width * 0.05f), width));
            menu.Add("leftY", new Slider("Left Side Y Position", 0, -(int)(height * 0.1f), height));
            menu.AddLabel("Right Side");
            menu.Add("rightX", new Slider("Right Side X Position", 0, -width, (int)(width * 0.05f)));
            menu.Add("rightY", new Slider("Right Side Y Position", 0, -height, (int)(height * 0.1f)));

            new TextureManager();
            Game.OnTick += Game_OnTick;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if(!Enabled)
                return;

            foreach (var hero in EntityManager.Heroes.AllHeroes)
            {
                if (hero.IsDead && !DeathTimes.Any(d => d.Hero.IdEquals(hero)))
                {
                    var newdeath = new DeathTimers(hero);
                    DeathTimes.Add(newdeath);
                }
            }
            DeathTimes.RemoveAll(d => d.CurrentTimer < 0);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!Enabled)
                return;
            try
            {
                if (!TextureManager.FinishedLoadingTexture)
                    return;

                float sideX = Drawing.Width;
                float sideY = Drawing.Height;
                var side = new Vector2(sideX, sideY);

                var allydiffer = 0;
                var enemydiffer = 0;

                var space = menu["space"].Cast<Slider>().CurrentValue;
                var blueside = menu["blueside"].Cast<ComboBox>().CurrentValue;
                var redside = menu["redside"].Cast<ComboBox>().CurrentValue;

                var leftX = menu["leftX"].Cast<Slider>().CurrentValue;
                var leftY = menu["leftY"].Cast<Slider>().CurrentValue;
                var rightX = menu["rightX"].Cast<Slider>().CurrentValue;
                var rightY = menu["rightY"].Cast<Slider>().CurrentValue;

                foreach (var hero in EntityManager.Heroes.AllHeroes.OrderBy(a => a.ChampionName).Where(h => Spectating ? h.Team == GameObjectTeam.Order && DrawBlue || h.Team == GameObjectTeam.Chaos && DrawRed : h.IsAlly && DrawBlue || h.IsEnemy && DrawRed))
                {
                    var team = Spectating ? hero.Team == GameObjectTeam.Order : hero.IsAlly;
                    var herosDiffer = team ? allydiffer : enemydiffer;
                    
                    if (team)
                    {
                        if (blueside == 0)
                        {
                            sideX = leftX + Drawing.Width * 0.05f;
                            sideY = leftY + Drawing.Height * 0.1f;
                        }
                        else
                        {
                            sideX = rightX + Drawing.Width * 0.85f;
                            sideY = rightY + Drawing.Height * 0.1f;
                        }
                    }
                    else
                    {
                        if (redside == 0)
                        {
                            sideX = leftX + Drawing.Width * 0.05f;
                            sideY = leftY + Drawing.Height * 0.1f;
                        }
                        else
                        {
                            sideX = rightX + Drawing.Width * 0.85f;
                            sideY = rightY + Drawing.Height * 0.1f;
                        }
                    }

                    side = new Vector2(sideX, sideY);

                    var texture = hero.HeroIcons();

                    if (texture != null)
                    {
                        // The hero Icon
                        var heroiconheight = 0;
                        var heroiconwidth = 0;
                        var heroiconpos = new Vector2(sideX, sideY + herosDiffer);
                        hero.HeroIcon()?.Draw(heroiconpos);

                        var Y2 = herosDiffer;
                        var X2 = 0;
                        var textsize = 0;
                        var cdtextsize = 0f;
                        if (texture.HeroIcon.Rectangle.HasValue)
                        {
                            textsize += texture.HeroIcon.Rectangle.Value.Width + texture.HeroIcon.Rectangle.Value.Height;
                            X2 += texture.HeroIcon.Rectangle.Value.Width;
                            heroiconheight = texture.HeroIcon.Rectangle.Value.Height;
                            heroiconwidth = texture.HeroIcon.Rectangle.Value.Width;
                        }

                        // Spells and Summoner Spells
                        float textwidth = 0;
                        foreach (var spell in texture.Spells)
                        {
                            var spellsprite = hero.SpellSprite(spell);
                            if (cdtextsize.Equals(0f) && spellsprite.Rectangle != null)
                            {
                                textwidth = spellsprite.Rectangle.Value.Width;
                                cdtextsize += (spellsprite.Rectangle.Value.Height + spellsprite.Rectangle.Value.Width);
                            }

                            var spellpos = new Vector2(side.X + X2, side.Y + Y2);
                            spellsprite.Draw(spellpos);

                            // Shows Spell CoolDown
                            if (hero.IsOnCoolDown(spell.Slot) && DrawCD)
                            {
                                if (CDText == null)
                                {
                                    CDText = new Text("", new Font(FontFamily.GenericSerif, cdtextsize * 0.23f, FontStyle.Regular)) { Color = Color.AliceBlue };
                                }
                                var cdtextpos = new Vector2(spellpos.X + (textwidth * 1.1f), spellpos.Y);
                                CDText.Draw(hero.Spellbook.GetSpell(spell.Slot).CoolDown(), Color.AliceBlue, cdtextpos);
                            }

                            if (spellsprite.Rectangle.HasValue)
                            {
                                Y2 += spellsprite.Rectangle.Value.Height;
                            }
                        }

                        if (texture.HeroIcon.Rectangle.HasValue)
                        {
                            if (team)
                                allydiffer += texture.HeroIcon.Rectangle.Value.Height + space;
                            else
                                enemydiffer += texture.HeroIcon.Rectangle.Value.Height + space;
                        }

                        // XP Health and Mana bars
                        var HPEmptybar = texture.EmptyBar;
                        var MPEmptybar = texture.EmptyBar;
                        var XPEmptybar = texture.EmptyBar;
                        var HPbar = texture.HPBar;
                        var MPbar = texture.MPBar;
                        var XPbar = texture.XPBar;

                        var barheight = 0;
                        if (DrawXP)
                        {
                            //draw XP bar
                            var xpbarpos = new Vector2(sideX, sideY + Y2);
                            XPbar.Scale = new Vector2(1 * (hero.CurrentXPPercent() / 100), 1);
                            XPEmptybar.Draw(xpbarpos);
                            XPbar.Draw(xpbarpos);

                            if (XPbar.Rectangle.HasValue)
                                barheight += XPbar.Rectangle.Value.Height;
                        }

                        if (DrawHP)
                        {
                            //draw HP bar
                            var hpbarpos = new Vector2(sideX, sideY + Y2 + barheight);
                            HPbar.Scale = new Vector2(1 * (hero.HealthPercent / 100), 1);
                            HPEmptybar.Draw(hpbarpos);
                            HPbar.Draw(hpbarpos);

                            if (HPbar.Rectangle.HasValue)
                                barheight += HPbar.Rectangle.Value.Height;
                        }

                        if (DrawMP)
                        {
                            //draw MP bar
                            var mpbarpos = new Vector2(sideX, sideY + Y2 + barheight);
                            MPbar.Scale = new Vector2(1 * (hero.ManaPercent / 100), 1);
                            MPEmptybar.Draw(mpbarpos);
                            MPbar.Draw(mpbarpos);
                        }

                        //recall / teleport Tracker
                        if (TrackTeleports && hero.Recalling())
                        {
                            var type = hero.PortTypeIsRecall() ? texture.Recall : texture.Teleport;
                            var recallpos = new Vector2(heroiconpos.X, heroiconpos.Y + heroiconheight);
                            var info = hero.TeleportInfo();
                            type.Scale = new Vector2(1, info.PercentInReverce).Rotated((float)(180 * Math.PI / 180));
                            type.Draw(recallpos);
                        }

                        if (DrawLevel)
                        {
                            // Hero Level
                            if (Levelext == null)
                                Levelext = new Text("", new Font(FontFamily.GenericSerif, textsize * 0.1f, FontStyle.Bold)) { Color = Color.AliceBlue };

                            var levelpos = new Vector2(heroiconpos.X + heroiconwidth * 0.72f, heroiconpos.Y + heroiconheight * 0.69f);
                            Levelext.Draw(hero.Level.ToString(), Color.AliceBlue, levelpos);
                        }

                        if (DrawDeath)
                        {
                            // Death Timer
                            var dead = DeathTimes.FirstOrDefault(d => d.Hero.IdEquals(hero));
                            if (dead != null && hero.IsDead)
                            {
                                textsize = (int)(textsize * 0.15f);
                                if (DeathTimerText == null)
                                {
                                    DeathTimerText = new Text("", new Font(FontFamily.GenericSerif, textsize, FontStyle.Bold)) { Color = Color.Red };
                                }
                                DeathTimerText.Draw(dead.CurrentTimer.ToString(), Color.Red, heroiconpos);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("InvalidCall"))
                {
                    Logger.Send("Reloading Texture", Logger.LogLevel.Error);
                    TextureManager.FinishedLoadingTexture = false;
                    DeathTimerText = null;
                    CDText = null;
                    Levelext = null;
                    LoadTexture.LoadAndStore();
                }
            }
        }
    }
}
