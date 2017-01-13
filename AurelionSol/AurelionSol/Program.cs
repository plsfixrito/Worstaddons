namespace AurelionSol
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        private static Geometry.Polygon.Circle QCircle;

        private static MissileClient QMissle;

        private static readonly AIHeroClient player = ObjectManager.Player;

        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Active W { get; private set; }

        public static Spell.Skillshot R { get; private set; }

        public static Menu ComboMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        private static Menu menuIni;

        public static void Execute()
        {
            if (player.ChampionName != "AurelionSol")
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, (uint)650f, SkillShotType.Linear, 250, 850, 180);
            W = new Spell.Active(SpellSlot.W, 725);
            R = new Spell.Skillshot(SpellSlot.R, 1500, SkillShotType.Linear, 250, 1750, 180);

            menuIni = MainMenu.AddMenu("AurelionSol", "AurelionSol");
            menuIni.AddGroupLabel("Welcome to the Worst AurelionSol addon!");
            menuIni.AddGroupLabel("Global Settings");
            menuIni.Add("Combo", new CheckBox("Use Combo?"));
            menuIni.Add("Harass", new CheckBox("Use Harass?"));
            menuIni.Add("Clear", new CheckBox("Use Lane Clear?"));
            menuIni.Add("Drawings", new CheckBox("Use Drawings?"));

            ComboMenu = menuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("Q2", new CheckBox("Follow Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("W2", new CheckBox("Use W2"));
            ComboMenu.Add("R", new CheckBox("Use R"));
            ComboMenu.Add("Rhit", new Slider("Use R Hit", 2, 1, 5));

            HarassMenu = menuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("W", new CheckBox("Use W", false));
            HarassMenu.Add("W2", new CheckBox("Use W2", false));
            HarassMenu.Add("Mana", new Slider("Save Mana %", 30, 0, 100));

            LaneMenu = menuIni.AddSubMenu("Farm");
            LaneMenu.AddGroupLabel("LaneClear Settings");
            LaneMenu.Add("Q", new CheckBox("Use Q"));
            LaneMenu.Add("W", new CheckBox("Use W", false));
            LaneMenu.Add("Mana", new Slider("Save Mana %", 30, 0, 100));

            MiscMenu = menuIni.AddSubMenu("Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("gapcloserQ", new CheckBox("Anti-GapCloser (Q)"));
            MiscMenu.Add("gapcloserR", new CheckBox("Anti-GapCloser (R)"));
            MiscMenu.Add("KillStealQ", new CheckBox("KillSteal (Q)"));
            MiscMenu.Add("KillStealR", new CheckBox("KillSteal (R)"));
            MiscMenu.Add("AQ", new Slider("Auto Trigger Q On hit (Size inc)", 1, 1, 5));

            DrawMenu = menuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("Q", new CheckBox("Draw Q"));
            DrawMenu.Add("W", new CheckBox("Draw W"));
            DrawMenu.Add("E", new CheckBox("Draw E"));
            DrawMenu.Add("R", new CheckBox("Draw R"));
            DrawMenu.Add("QS", new CheckBox("Draw Q Size"));

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGap;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss != null && miss.IsValid)
            {
                if (miss.SpellCaster.IsMe && miss.SpellCaster.IsValid && miss.SData.Name.Contains("AurelionSolQMissile"))
                {
                    QMissle = miss;
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid)
            {
                return;
            }
            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe
                && miss.SData.Name.Contains("AurelionSolQMissile"))
            {
                QMissle = null;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                Combo();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && menuIni.Get<CheckBox>("Harass").CurrentValue)
            {
                Harass();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && menuIni.Get<CheckBox>("Clear").CurrentValue)
            {
                Clear();
            }

            var qsize = QMissle?.StartPosition.Distance(QMissle.Position);
            var f = (qsize + Q.Width) / 16;
            if (f != null && (QMissle?.Position.CountEnemiesInRange((float)f) >= MiscMenu.Get<Slider>("AQ").CurrentValue && Q.Handle.ToggleState == 2))
            {
                Q.Cast(Game.CursorPos);
            }

            Orbwalker.DisableAttacking = Q.Handle.ToggleState == 2;
            KS();
        }

        private static void Gapcloser_OnGap(AIHeroClient Sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Misc").CurrentValue || Sender == null || !Sender.IsEnemy)
            {
                return;
            }

            if (MiscMenu.Get<CheckBox>("gapcloserQ").CurrentValue)
            {
                var pred = Q.GetPrediction(Sender);
                if (Q.Handle.ToggleState == 1 && Sender.IsValidTarget(Q.Range))
                {
                    Q.Cast(pred.CastPosition);
                }

                if (Q.Handle.ToggleState == 2)
                {
                    var qsize = QMissle.StartPosition.Distance(QMissle.Position);
                    if (QMissle.Position.IsInRange(Sender, (qsize + Q.Width) / 16))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
            }

            if (MiscMenu.Get<CheckBox>("gapcloserR").CurrentValue)
            {
                var pred = R.GetPrediction(Sender);
                if (args.SenderMousePos.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
                {
                    R.Cast(pred.CastPosition);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!player.IsDead && menuIni.Get<CheckBox>("Drawings").CurrentValue)
            {
                if (DrawMenu.Get<CheckBox>("Q").CurrentValue)
                {
                    Circle.Draw(Color.Blue, Q.Range, Player.Instance.Position);
                }

                if (DrawMenu.Get<CheckBox>("W").CurrentValue)
                {
                    Circle.Draw(Color.Blue, W.Range, Player.Instance.Position);
                    Circle.Draw(Color.Blue, W.Range - 250, Player.Instance.Position);
                }

                if (DrawMenu.Get<CheckBox>("R").CurrentValue)
                {
                    Circle.Draw(Color.Blue, R.Range, Player.Instance.Position);
                }

                if (DrawMenu.Get<CheckBox>("QS").CurrentValue && QMissle != null)
                {
                    var Qsize = QMissle.StartPosition.Distance(QMissle.Position);
                    Circle.Draw(Color.White, Q.Width + Qsize / 16, QMissle.Position);
                }
            }
        }

        private static void KS()
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(target => target != null))
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && Damage.Q(target) >= target.Health)
                {
                    if (MiscMenu["KillStealQ"].Cast<CheckBox>().CurrentValue)
                    {
                        var pred = Q.GetPrediction(target);
                        if (Q.Handle.ToggleState == 1)
                        {
                            Q.Cast(pred.CastPosition);
                        }

                        if (Q.Handle.ToggleState == 2)
                        {
                            var qsize = QMissle.StartPosition.Distance(QMissle.Position);
                            if (QMissle.Position.IsInRange(target, (qsize + Q.Width) / 16))
                            {
                                Q.Cast(Game.CursorPos);
                            }
                        }
                    }
                }

                if (MiscMenu["KillStealR"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(R.Range) && R.IsReady()
                    && Damage.R(target) >= target.Health)
                {
                    R.Cast(target.Position);
                }
            }
        }

        private static void Combo()
        {
            var fQ = ComboMenu["Q2"].Cast<CheckBox>().CurrentValue;
            var useQ = ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var useW = ComboMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var useW2 = ComboMenu["W2"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var useR = ComboMenu["R"].Cast<CheckBox>().CurrentValue && R.IsReady();
            var Rhit = ComboMenu["Rhit"].Cast<Slider>().CurrentValue;
            var Qtarget = TargetSelector.GetTarget(Q.Range * 2, DamageType.Magical);
            var Wtarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var Rtarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (useQ && Qtarget != null && Qtarget.IsValidTarget(Q.Range))
            {
                var pred = Q.GetPrediction(Qtarget);
                if (Q.Handle.ToggleState == 1)
                {
                    Q.Cast(pred.CastPosition);
                }

                if (Q.Handle.ToggleState == 2 && QMissle != null)
                {
                    var qsize = QMissle.StartPosition.Distance(QMissle.Position);
                    if (QMissle.Position.IsInRange(Qtarget, (qsize + Q.Width) / 16))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
            }

            if (fQ && Q.Handle.ToggleState == 2 && QMissle != null)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, QMissle.Position);
            }

            if (fQ)
            {
                Orbwalker.DisableAttacking = Q.Handle.ToggleState == 2;
                Orbwalker.DisableMovement = Q.Handle.ToggleState == 2;
            }

            if (useW)
            {
                if (W.Handle.ToggleState != 2 && Wtarget != null && Wtarget.IsValidTarget(W.Range) && !Wtarget.IsValidTarget(W.Range - 250))
                {
                    W.Cast();
                }
            }

            if (useW2)
            {
                if (W.Handle.ToggleState == 2 && (!Wtarget.IsValidTarget(W.Range) || Wtarget.IsValidTarget(W.Range - 250)))
                {
                    W.Cast();
                }
            }

            if (useR && Rtarget != null && Rtarget.IsValidTarget(R.Range))
            {
                foreach (var enemy in from enemy in EntityManager.Heroes.Enemies
                                      let startPos = enemy.ServerPosition
                                      let endPos = Player.Instance.ServerPosition.Extend(startPos, Player.Instance.Distance(enemy) + R.Range)
                                      let rectangle = new Geometry.Polygon.Rectangle((Vector2)startPos, endPos, R.Radius)
                                      where EntityManager.Heroes.Enemies.Count(x => rectangle.IsInside(x)) >= Rhit
                                      select enemy)
                {
                    R.Cast(enemy.Position);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var useW = HarassMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var useW2 = HarassMenu["W2"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var Wtarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (useQ && Qtarget != null && Qtarget.IsValidTarget(Q.Range))
            {
                var qsize = QMissle.StartPosition.Distance(QMissle.Position);
                var pred = Q.GetPrediction(Qtarget);
                if (pred.HitChance >= HitChance.High && Q.Handle.ToggleState == 1)
                {
                    Q.Cast(pred.CastPosition);
                }

                if (QMissle.Position.IsInRange(Qtarget, (qsize + Q.Width) / 16) && Q.Handle.ToggleState == 2)
                {
                    Q.Cast(Game.CursorPos);
                }
            }

            if (useW)
            {
                if (W.Handle.ToggleState != 2 && Wtarget != null && Wtarget.IsValidTarget(W.Range) && !Wtarget.IsValidTarget(W.Range - 250))
                {
                    W.Cast();
                }
            }

            if (useW2)
            {
                if (W.Handle.ToggleState == 2 && (!Wtarget.IsValidTarget(W.Range) || Wtarget.IsValidTarget(W.Range - 250)))
                {
                    W.Cast();
                }
            }
        }

        private static void Clear()
        {
            var useQ = LaneMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var useW = LaneMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady();

            if (useQ)
            {
                // Credits stefsot
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 1500, false);

                var predictResult =
                    Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), Q.Range, Q.Radius, Q.CastDelay, Q.Speed)
                        .OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length)
                        .FirstOrDefault();

                if (predictResult != null && predictResult.CollisionObjects.Length >= 2)
                {
                    Q.Cast(predictResult.CastPosition);
                }
            }

            if (useW)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, W.Range, false);

                if (minions.Count() >= 2)
                {
                    W.Cast();
                }
            }
        }
    }
}