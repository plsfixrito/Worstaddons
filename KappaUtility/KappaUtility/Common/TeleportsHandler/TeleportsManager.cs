using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;

namespace KappaUtility.Common.TeleportsHandler
{
    internal static class TeleportsManager
    {
        public static List<TeleportInfo> CurrentTeleports = new List<TeleportInfo>();

        public static bool Recalling(this AIHeroClient hero)
        {
            return CurrentTeleports.Any(h => h.Sender.IdEquals(hero));
        }

        public static TeleportInfo TeleportInfo(this AIHeroClient hero)
        {
            return CurrentTeleports.FirstOrDefault(h => h.Sender.IdEquals(hero));
        }

        public static bool PortTypeIsRecall(this AIHeroClient hero)
        {
            return TeleportInfo(hero).Args.Type == TeleportType.Recall;
        }

        public static bool PortTypeIsTeleport(this AIHeroClient hero)
        {
            return TeleportInfo(hero).Args.Type == TeleportType.Teleport || TeleportInfo(hero).Args.Type == TeleportType.Shen || TeleportInfo(hero).Args.Type == TeleportType.TwistedFate;
        }

        public static void Init()
        {
            Teleport.OnTeleport += Teleport_OnTeleport;
            Game.OnTick += delegate { CurrentTeleports.RemoveAll(t => t.Ended); };
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if(hero == null)
                return;

            var tpinfo = new TeleportInfo(hero, args);
            if (args.Status == TeleportStatus.Start && !Recalling(hero))
            {
                CurrentTeleports.Add(tpinfo);
            }
            if ((args.Status == TeleportStatus.Abort || args.Status == TeleportStatus.Finish || args.Status == TeleportStatus.Unknown) && Recalling(hero))
            {
                CurrentTeleports.RemoveAll(t => t.Sender.IdEquals(hero));
            }
        }
    }
}
