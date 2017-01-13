using System;
using EloBuddy;
using KappaUtility.Common.Misc;
using KappaUtility.Common.Misc.Entities;
using KappaUtility.Common.TeleportsHandler;
using KappaUtility.Common.Texture;

namespace KappaUtility.Common
{
    internal class Load
    {
        public static void Init()
        {
            try
            {
                KappaEvade.KappaEvade.Init();
                Events.OnInComingDamage.Init();
                switch (Game.MapId)
                {
                    case GameMapId.SummonersRift:
                        Mobs.JungleMobsNames = Mobs.SRJungleMobsNames;
                        break;
                    case GameMapId.TwistedTreeline:
                        Mobs.JungleMobsNames = Mobs.TTJungleMobsNames;
                        break;
                    case GameMapId.CrystalScar:
                        Mobs.JungleMobsNames = Mobs.ASCJungleMobsNames;
                        break;
                }
                
                TeleportsManager.Init();
            }
            catch (Exception ex)
            {
                Logger.Send("Error At Common.Load.Init", ex, Logger.LogLevel.Error);
            }
        }
    }
}
