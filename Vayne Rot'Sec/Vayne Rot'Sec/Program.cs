using System;

namespace Vayne_Rot_Sec
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        public static Menu RotSec;

        public static readonly Item ZZrot = new Item(ItemId.ZzRot_Portal, 400);

        public static AIHeroClient Target;

        public static SpellDataInst E;

        public static void Execute()
        {
            if (Player.Instance.ChampionName != "Vayne")
            {
                return;
            }

            RotSec = MainMenu.AddMenu("Vayne Rot'Sec", "Vayne Rot'Sec");
            RotSec.Add("Kappa", new KeyBind("Rot'Sec Selected Target", false, KeyBind.BindTypes.HoldActive));
            RotSec.Add("delay", new Slider("ZZ'Rot Drop Delay {0}ms", 100, 100, 250));
            RotSec.Add("Erange", new CheckBox("Draw Rot'Sec Range"));
            RotSec.Add("pred", new CheckBox("Draw Rot'Sec Pred"));
            RotSec.AddSeparator();
            RotSec.AddGroupLabel("Small Guide:");
            RotSec.AddLabel("Select a Target then hold the KeyBind to do the Rot'Sec.");
            RotSec.AddLabel("It will only be casted in Range");

            E = Player.GetSpell(SpellSlot.E);
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (RotSec["Erange"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.MediumPurple, ZZrot.Range, Player.Instance.Position);
            }

            if (Target == null || Player.Instance.Distance(Target) > 1000)
            {
                return;
            }
            if (RotSec["pred"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawLine(
                    Drawing.WorldToScreen((Vector3)Target.Position.Extend(Player.Instance.Position, -100)),
                    Drawing.WorldToScreen((Vector3)Player.Instance.Position.Extend(Target.Position, -25)),
                    3,
                    System.Drawing.Color.White);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            Target = TargetSelector.SelectedTarget;
            if (Target == null || Player.Instance.Distance(Target) > 1000)
            {
                return;
            }
            if (RotSec["Kappa"].Cast<KeyBind>().CurrentValue && E.IsReady && ZZrot.IsReady() && Target.IsValidTarget(ZZrot.Range))
            {
                if (Player.CastSpell(SpellSlot.E, Target))
                {
                    Core.DelayAction(
                        () => { ZZrot.Cast(Target.Position.To2D().Extend(Player.Instance.ServerPosition.To2D(), -100).To3D()); },
                        RotSec["delay"].Cast<Slider>().CurrentValue);
                }
            }
        }
    }
}