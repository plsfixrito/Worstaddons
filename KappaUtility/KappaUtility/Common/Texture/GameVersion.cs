using System;
using System.Net;
using KappaUtility.Common.Misc;
using Newtonsoft.Json.Linq;

namespace KappaUtility.Common.Texture
{
    class GameVersion
    {
        public static Version CurrentPatch;
        private static string VersionUrl = "http://ddragon.leagueoflegends.com/api/versions.json";

        public static void Init()
        {
            try
            {
                if (CurrentPatch != null)
                    return;

                var WebClient = new WebClient();
                WebClient.DownloadStringCompleted += WebOnDownloadStringCompleted;
                WebClient.DownloadStringTaskAsync(VersionUrl);
            }
            catch (Exception ex)
            {
                Logger.Send("ERROR", ex, Logger.LogLevel.Error);
            }
        }

        private static void WebOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            try
            {
                var versionJson = JArray.Parse(downloadStringCompletedEventArgs.Result);
                var stringversion = versionJson.First.ToObject<string>();
                CurrentPatch = new Version(stringversion);

                Logger.Send("LiveVersion = " + CurrentPatch);

                TextureDownloader.ChampionIconsFolder = main.KappaUtilityFolder + "\\" + CurrentPatch + "\\ChampionIcons\\";
                TextureDownloader.SummonersIconsFolder = main.KappaUtilityFolder + "\\" + CurrentPatch + "\\SummonerSpellsIcons\\";

                TextureDownloader.Start();
            }
            catch (Exception e)
            {
                Logger.Send(e.ToString());
            }
        }
    }
}
