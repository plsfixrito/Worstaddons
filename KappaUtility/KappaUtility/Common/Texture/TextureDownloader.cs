using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.Misc;

namespace KappaUtility.Common.Texture
{
    internal class TextureDownloader
    {
        private static int i;
        private static bool Loaded;
        public static string ChampionIconsFolder = main.KappaUtilityFolder;
        public static string SummonersIconsFolder = main.KappaUtilityFolder;
        public static string ChampionsIconsUrl = "http://ddragon.leagueoflegends.com/cdn/{0}/img/champion/";
        public static string AbilitiesIconsUrl = "http://ddragon.leagueoflegends.com/cdn/{0}/img/spell/";
        internal static readonly List<SpellSlot> spellSlots = new List<SpellSlot>() { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        internal static readonly List<SpellSlot> SummonerSpells = new List<SpellSlot>() { SpellSlot.Summoner1, SpellSlot.Summoner2 };

        public static void Start()
        {
            try
            {
                ChampionsIconsUrl = string.Format(ChampionsIconsUrl, GameVersion.CurrentPatch);
                AbilitiesIconsUrl = string.Format(AbilitiesIconsUrl, GameVersion.CurrentPatch);

                foreach (var hero in EntityManager.Heroes.AllHeroes)
                {
                    var chmpdirc = ChampionIconsFolder + hero.ChampionName + "\\";
                    if (!Directory.Exists(chmpdirc))
                    {
                        Directory.CreateDirectory(chmpdirc);
                    }

                    if (!Directory.GetFiles(chmpdirc).Contains(chmpdirc + hero.ChampionName + ".png"))
                    {
                        Logger.Send("Downloading " + hero.ChampionName + " Icon !");
                        var webClient = new WebClient();
                        i++;
                        webClient.DownloadFileAsync(new Uri(ChampionsIconsUrl + hero.ChampionName + ".png"), chmpdirc + hero.ChampionName + ".png");
                        webClient.DownloadFileCompleted += delegate
                        {
                            webClient.Dispose();
                            i--;
                        };
                    }

                    GetSpellSlots(hero, chmpdirc);

                    GetSummonerSpells(hero);
                }

                Game.OnTick += delegate
                {
                    if (Loaded)
                    {
                        return;
                    }

                    if (i == 0 && !Loaded)
                    {
                        Logger.Send("Your Champion Icons Are Updated !");
                        Loaded = true;
                        LoadTexture.LoadAndStore();
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Send("ERROR", ex, Logger.LogLevel.Error);
            }
        }

        private static void GetSpellSlots(AIHeroClient hero, string dir)
        {
            foreach (var slot in spellSlots)
            {
                var spell = hero.Spellbook.GetSpell(slot);
                var filename = spell.Name + ".png";
                if (!Directory.GetFiles(dir).Contains(dir + hero.ChampionName + slot + ".png"))
                {
                    Logger.Send("Downloading " + filename);
                    var webClient = new WebClient();
                    i++;
                    webClient.DownloadFileAsync(new Uri(AbilitiesIconsUrl + filename), dir + hero.ChampionName + slot + ".png");
                    webClient.DownloadFileCompleted += delegate
                    {
                        webClient.Dispose();
                        i--;
                    };
                }
            }
        }

        private static void GetSummonerSpells(AIHeroClient hero)
        {
            foreach (var slotname in SummonerSpells.Select(spell => hero.Spellbook.GetSpell(spell).Name + ".png"))
            {
                if (!Directory.Exists(SummonersIconsFolder))
                {
                    Directory.CreateDirectory(SummonersIconsFolder);
                }

                if (!Directory.GetFiles(SummonersIconsFolder).Contains(SummonersIconsFolder + slotname))
                {
                    Logger.Send("Downloading " + slotname);
                    var webClient = new WebClient();
                    i++;
                    webClient.DownloadFileAsync(new Uri(AbilitiesIconsUrl + slotname), SummonersIconsFolder + slotname);
                    webClient.DownloadFileCompleted += delegate
                    {
                        webClient.Dispose();
                        i--;
                    };
                }
            }
        }
    }
}
