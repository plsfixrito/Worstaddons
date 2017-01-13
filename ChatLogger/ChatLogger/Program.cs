using System;
using System.IO;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace ChatLogger
{
    internal class Program
    {
        public static string Folder;
        public static string File;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\ChatLogger";

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            File = Folder + "\\" + Player.Instance.Name + " - " + Player.Instance.ChampionName + " - " + DateTime.Now.ToString("yy-MM-dd") + " - " + Game.GameId + ".txt";
            Chat.OnMessage += Chat_OnMessage;
        }

        private static void Chat_OnMessage(AIHeroClient sender, ChatMessageEventArgs args)
        {
            using (var stream = new StreamWriter(File, true))
            {
                var msg = args.Message.Replace("<font color=", "").Replace("\"#40c1ff\">", "").Replace("\"#ffffff\">", "").Replace("\"#ff3333\">", "").Replace("</font>", "");
                var ts = TimeSpan.FromSeconds(Game.Time);
                var time = $"{ts.Minutes}:{ts.Seconds:D2}";
                var finalmsg = "[" + time + "] " + msg;
                stream.WriteLine(finalmsg);
                stream.Close();
            }
        }
    }
}
