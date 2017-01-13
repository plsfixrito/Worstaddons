using System;
using System.Globalization;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace KappaUtilityOld.Trackers
{
    using Color = System.Drawing.Color;

    internal class Traps : Tracker
    {
        internal static void Draw()
        {
            if (!TrackMenu["Tracktraps"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var traps = ObjectManager.Get<Obj_AI_Minion>();
            {
                foreach (var trap in traps.Where(trap => trap != null && trap.IsEnemy))
                {
                    switch (trap.Name)
                    {
                        case "Cupcake Trap":
                            {
                                if (trap.GetBuff("CaitlynYordleTrap") != null)
                                {
                                    var endTime = Math.Max(0, trap.GetBuff("CaitlynYordleTrap").EndTime - Game.Time);
                                    Drawing.DrawText(
                                        Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                        Color.White,
                                        "Caitlyn Trap Expire: " + Convert.ToString(endTime, CultureInfo.InstalledUICulture),
                                        2);
                                }
                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius + 10, trap.Position);
                            }
                            break;

                        case "Noxious Trap":
                            if (trap.BaseSkinName == "NidaleeSpear")
                            {
                                var endTime = Math.Max(0, Game.Time);
                                Drawing.DrawText(Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30), Color.White, "Nidalee Trap", 2);
                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius + 25, trap.Position);
                            }

                            if (trap.BaseSkinName == "TeemoMushroom")
                            {
                                if (trap.GetBuff("BantamTrap") != null)
                                {
                                    var endTime = Math.Max(0, trap.GetBuff("BantamTrap").EndTime - Game.Time);
                                    Drawing.DrawText(
                                        Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                        Color.White,
                                        "Teemo Mushroom Expire: " + Convert.ToString(endTime, CultureInfo.InstalledUICulture),
                                        2);
                                }

                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius * 3, trap.Position);
                            }
                            if (trap.BaseSkinName == "JhinTrap")
                            {
                                if (trap.GetBuff("JhinETrap") != null)
                                {
                                    var endTime = Math.Max(0, trap.GetBuff("JhinETrap").EndTime - Game.Time);
                                    Drawing.DrawText(
                                        Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                        Color.White,
                                        "Jhin Trap Expire: " + Convert.ToString(endTime, CultureInfo.InstalledUICulture),
                                        2);
                                }

                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius * 3, trap.Position);
                            }
                            break;

                        case "Jack In The Box":
                            if (trap.GetBuff("JackInTheBox") != null)
                            {
                                var endTime = Math.Max(0, trap.GetBuff("JackInTheBox").EndTime - Game.Time);
                                Drawing.DrawText(
                                    Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                    Color.White,
                                    "Shaco Box Expire: " + Convert.ToString(endTime, CultureInfo.InvariantCulture),
                                    2);
                            }

                            Circle.Draw(SharpDX.Color.White, trap.BoundingRadius * 15, trap.Position);
                            break;
                    }
                }
            }
        }
    }
}