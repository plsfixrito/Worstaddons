using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace AFK_RIFT
{
    internal class Program
    {
        private static float StartTime;

        private static bool Ended;

        private static Vector3 Position
        {
            get
            {
                return Player.Instance.Team == GameObjectTeam.Order ? new Vector3(8622, 3258, 54.54328f) : new Vector3(6224, 11606, 56.76437f);
            }
        }

        private static Vector3 WardPosition
        {
            get
            {
                return Player.Instance.Team == GameObjectTeam.Order ? new Vector3(8398, 2798, 51.13f) : new Vector3(6454, 11984, 56.4768f);
            }
        }

        private static readonly List<ItemId> ItemsToBuy = new List<ItemId>() { ItemId.Vision_Ward, ItemId.Boots_of_Speed };

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            StartTime = Game.Time;
            if (Game.MapId != GameMapId.SummonersRift)
            {
                Chat.Print("AFK_RIFT Works Only In Summoners Rift");
                return;
            }
            if (!Player.Instance.IsRanged)
            {
                Chat.Print("AFK_RIFT Works Only For Ranged Champions");
                return;
            }

            foreach (var item in ItemsToBuy.Where(i => !new Item(i).IsOwned()))
            {
                Shop.BuyItem(item);
            }

            Orbwalker.OverrideOrbwalkPosition = OverrideOrbwalkPosition;
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Circle.Draw(Color.AliceBlue, 150, Position);
            Circle.Draw(Color.MediumPurple, 100, WardPosition);
        }

        private static Vector3? OverrideOrbwalkPosition()
        {
            return Position;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if(Game.Time - StartTime < 20) return;

            if (ObjectManager.Get<Obj_HQ>().Any(n => n.IsDead || n.Health <= 0) && !Ended)
            {
                Ended = true;
                var random = new Random().Next(5000, 25000);
                Core.DelayAction(() => Game.QuitGame(), random);
                Console.WriteLine("Game Ended ! Leaving Game In: " + random / 1000 + " Seconds.");
            }
            
            var visionward = new Item(ItemId.Vision_Ward, 600);
            if (visionward.IsInRange(WardPosition) && visionward.IsOwned(Player.Instance))
            {
                visionward.Cast(WardPosition);
            }

            Orbwalker.DisableMovement = Player.Instance.ServerPosition.IsInRange(Position, 1);
            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.JungleClear;
            Orbwalker.OrbwalkTo(Position);
        }
    }
}
