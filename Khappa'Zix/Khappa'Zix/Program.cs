using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using Khappa_Zix.Handler;
using Khappa_Zix.Handler.ModesManager;
using Khappa_Zix.Settings;

namespace Khappa_Zix
{

    internal class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName.Equals("KhaZix", StringComparison.CurrentCultureIgnoreCase))
            {
                new Config();
                new SpellHandler();
                new ModesManager();
                Console.Title = "Khappa'Zix took Over this console Keepo ( ͡° ͜ʖ ͡°)";
            }
        }
    }
}
