using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using KappaUtility.Brain.Utility.Tracker.HUD;
using KappaUtility.Common.Misc;
using static KappaUtility.Common.Texture.TextureDownloader;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace KappaUtility.Common.Texture
{
    internal class LoadTexture
    {
        public static List<ChampionTexture> LoadedTexture = new List<ChampionTexture>();

        internal static readonly TextureLoader TextureLoader = new TextureLoader();
        private static string TextureName;
        private static SharpDX.Direct3D9.Texture LoadTextures
        {
            get
            {
                return TextureLoader[TextureName];
            }
        }

        public static void Init()
        {
            try
            {
                if (GameVersion.CurrentPatch == null)
                {
                    GameVersion.Init();
                }
                else
                {
                    TextureDownloader.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Send("ERROR", ex, Logger.LogLevel.Error);
            }
        }

        public static void LoadAndStore()
        {
            try
            {
                DisposeEverything();

                TextureLoader.Load("HP", Properties.Resources.HP);
                TextureLoader.Load("MP", Properties.Resources.MP);
                TextureLoader.Load("XP", Properties.Resources.XP);
                TextureLoader.Load("Empty", Properties.Resources.Empty);
                TextureLoader.Load("Recall", Properties.Resources.Recall);
                TextureLoader.Load("Teleport", Properties.Resources.Teleport);
                var hp = new Sprite(() => TextureLoader["HP"]);
                var mp = new Sprite(() => TextureLoader["MP"]);
                var xp = new Sprite(() => TextureLoader["XP"]);
                var emp = new Sprite(() => TextureLoader["Empty"]);
                var recall = new Sprite(() => TextureLoader["Recall"]);
                var tp = new Sprite(() => TextureLoader["Teleport"]);
                
                foreach (var hero in EntityManager.Heroes.AllHeroes)
                {
                    var AllTexture = new List<SharpDX.Direct3D9.Texture>();
                    AllTexture.Clear();
                    var AllImages = new List<Image>();
                    AllImages.Clear();

                    var heroicon = ResizeImage(Image.FromFile(ChampionIconsFolder + hero.ChampionName + "\\" + hero.ChampionName + ".png"));
                    var heroTexture = TextureLoader.Load(heroicon, out TextureName);
                    AllTexture.Add(heroTexture);
                    AllImages.Add(heroicon);

                    var herodeadIcon = ReColor(heroicon);
                    var herodeadTexture = TextureLoader.Load(herodeadIcon, out TextureName);
                    AllTexture.Add(herodeadTexture);
                    AllImages.Add(herodeadIcon);

                    foreach (var slot in spellSlots)
                    {
                        var spellicon = ResizeImage(Image.FromFile(ChampionIconsFolder + hero.ChampionName + "\\" + hero.ChampionName + slot + ".png"));
                        var SpellTexture = TextureLoader.Load(spellicon, out TextureName);
                        AllTexture.Add(SpellTexture);
                        AllImages.Add(spellicon);

                        var notreadyicon = ReColor(spellicon);
                        var notreadytexture = TextureLoader.Load(notreadyicon, out TextureName);
                        AllTexture.Add(notreadytexture);
                        AllImages.Add(notreadyicon);
                    }

                    foreach (var sum in SummonerSpells)
                    {
                        var spell = hero.Spellbook.GetSpell(sum);
                        var spellicon = ResizeImage(Image.FromFile(SummonersIconsFolder + spell.Name + ".png"));
                        var SpellTexture = TextureLoader.Load(spellicon, out TextureName);
                        AllTexture.Add(SpellTexture);
                        AllImages.Add(spellicon);

                        var notreadyicon = ReColor(spellicon);
                        var notreadytexture = TextureLoader.Load(notreadyicon, out TextureName);
                        AllTexture.Add(notreadytexture);
                        AllImages.Add(notreadyicon);
                    }

                    var newtexture = new ChampionTexture(hero, AllTexture, AllImages, hp, mp, xp, emp, recall, tp);
                    if (!LoadedTexture.Contains(newtexture))
                    {
                        LoadedTexture.Add(newtexture);
                        Logger.Send(hero.Name() + " Texture Stored");
                    }
                }
                TextureManager.FinishedLoadingTexture = true;
            }
            catch (Exception ex)
            {
                Logger.Send("ERROR", ex, Logger.LogLevel.Error);
                DisposeEverything();
            }
        }

        public static void DisposeEverything()
        {
            TextureLoader.Dispose();
        }

        private static Bitmap ResizeImage(Image imgToResize)
        {
            var sizemod = ModFromScreenResolution();

            var currentscreenusage = (float)(imgToResize.Width + imgToResize.Height);

            if (currentscreenusage.Equals(240f)) // Hero Icon
            {
            }
            else
            {
                sizemod = sizemod * 0.62f;
            }

            var size = new Size((int)(imgToResize.Width * sizemod), (int)(imgToResize.Height * sizemod));

            return new Bitmap(imgToResize, size);
        }

        public static Bitmap ReColor(Bitmap bi)
        {
            using (var grf = Graphics.FromImage(bi))
            {
                using (Brush brsh = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                {
                    grf.FillRectangle(brsh, new Rectangle(0, 0, bi.Width, bi.Height));
                }
            }

            using (var grf = Graphics.FromImage(bi))
            {
                using (Brush brsh = new TextureBrush(bi))
                {
                    grf.FillRectangle(brsh, 6, 6, bi.Width, bi.Height);
                }
            }

            using (var grf = Graphics.FromImage(bi))
            {
                grf.InterpolationMode = InterpolationMode.High;
                grf.CompositingQuality = CompositingQuality.HighQuality;
                grf.SmoothingMode = SmoothingMode.AntiAlias;
                grf.DrawImage(bi, new Rectangle(0, 0, bi.Width, bi.Height));
            }

            return bi;
        }

        private static float ModFromScreenResolution()
        {
            var screensize = Drawing.Width + Drawing.Height;
            var h1080 = 2900; //1960x1080
            var h1050 = 2400; //1400x1050
            var h768 = 2100; //1366x768
            var h720 = 1900; //1280x720
            var h480 = 900; //640x480
            float sizemod;

            if (screensize >= h1080)
            {
                sizemod = 0.69f;
            }
            else
            {
                if (screensize >= h1050)
                {
                    sizemod = 0.65f;
                }
                else
                {
                    if (screensize >= h768)
                    {
                        sizemod = 0.60f;
                    }
                    else
                    {
                        if (screensize >= h720)
                        {
                            sizemod = 0.55f;
                        }
                        else
                        {
                            if (screensize >= h480)
                            {
                                sizemod = 0.50f;
                            }
                            else
                            {
                                sizemod = 0.3f;
                            }
                        }
                    }
                }
            }

            return sizemod * HUDTracker.HUDSize;
        }
    }
}
