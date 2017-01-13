namespace Kalista_FlyHack
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Program
    {
        private static Menu flymenu;

        public static float LastAATick;

        public static void Execute()
        {
            if (Player.Instance.ChampionName != "Kalista")
            {
                return;
            }

            flymenu = MainMenu.AddMenu("KalistaFlyHack", "KalistaFlyHack");
            flymenu.AddLabel("ONLY Works with combo mode.");
            flymenu.Add("Fly", new CheckBox("Use FlyHack", false));
            flymenu.AddSeparator();
            flymenu.AddGroupLabel("READ BEFORE USING !");
            flymenu.AddLabel("Using This Script Can Lead Into Perma Bans.");
            flymenu.AddLabel("Use It With YOUR OWN RISK.");

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (flymenu["Fly"].Cast<CheckBox>().CurrentValue && Player.Instance.AttackSpeedMod >= 2.5)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    var target = TargetSelector.GetTarget(ObjectManager.Player.GetAutoAttackRange(), DamageType.Physical);
                    if (target.IsValidTarget(ObjectManager.Player.GetAutoAttackRange()))
                    {
                        if (Core.GameTickCount - LastAATick <= 150)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                        if (Core.GameTickCount - LastAATick >= 50)
                        {
                            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            LastAATick = Core.GameTickCount;
                        }
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
            }
        }
    }
}