namespace KappaXerath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Program
    {
        public static class LastCastedSpell
        {
            public static string Name;

            public static float Time;
        }

        public const Champion ChampionName = Champion.Xerath;

        private static int lastNotification = 0;

        private static bool hasbought;

        public static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        public static readonly Item Scryb = new Item((int)ItemId.Farsight_Alteration, 3500f);

        public static Spell.Chargeable Q;

        public static Spell.Skillshot W;

        public static Spell.Skillshot E;

        public static Spell.Skillshot R;

        public static Menu Menuini, RMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, MiscMenu, DrawMenu, ColorMenu;

        private static bool AttacksEnabled
        {
            get
            {
                if (IsCastingR)
                {
                    return false;
                }

                if (Q.IsCharging)
                {
                    return false;
                }

                if (ComboMenu["key"].Cast<KeyBind>().CurrentValue)
                {
                    return IsPassiveUp || (!Q.IsReady() && !W.IsReady() && !E.IsReady());
                }

                return true;
            }
        }

        public static bool IsPassiveUp
        {
            get
            {
                return Player.HasBuff("XerathAscended2OnHit");
            }
        }

        public static bool IsCastingR
        {
            get
            {
                return Player.HasBuff("XerathLocusOfPower2")
                       || (LastCastedSpell.Name == "XerathLocusOfPower2" && Core.GameTickCount - LastCastedSpell.Time < 500);
            }
        }

        public static class RCharge
        {
            public static int CastT;

            public static int Index;

            public static Vector3 Position;

            public static bool TapKeyPressed;
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != ChampionName)
            {
                return;
            }

            Q = new Spell.Chargeable(SpellSlot.Q, 750, 1450, 1500, 500, int.MaxValue, 100) { AllowedCollisionCount = int.MaxValue };
            W = new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Circular, 250, int.MaxValue, 100) { AllowedCollisionCount = int.MaxValue };
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1600, 70);
            R = new Spell.Skillshot(SpellSlot.R, 3200, SkillShotType.Circular, 600, int.MaxValue, 125) { AllowedCollisionCount = int.MaxValue };

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("Xerath", "Xerath");
            RMenu = Menuini.AddSubMenu("R Settings");
            ComboMenu = Menuini.AddSubMenu("Combo Settings");
            HarassMenu = Menuini.AddSubMenu("Harass Settings");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
            KillStealMenu = Menuini.AddSubMenu("KillSteal Settings");
            MiscMenu = Menuini.AddSubMenu("Misc Settings");
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            ColorMenu = Menuini.AddSubMenu("Color Picker");

            RMenu.AddGroupLabel("R Settings");
            RMenu.Add("R", new CheckBox("Use R"));
            RMenu.Add(R.Slot + "hit", new ComboBox("R HitChance", 0, "High", "Medium", "Low"));
            RMenu.Add("scrybR", new CheckBox("Use Scrybing Orb while Ulting"));
            RMenu.Add("Rmode", new ComboBox("R Mode", 0, "Auto", "Custom Delays", "On Tap"));
            RMenu.Add("Rtap", new KeyBind("R Tap Key", false, KeyBind.BindTypes.HoldActive, 'S'));
            RMenu.AddGroupLabel("R Custom Delays");
            for (int i = 1; i <= 5; i++)
            {
                RMenu.Add("delay" + i, new Slider("Delay " + i, 0, 0, 1500));
            }
            RMenu.Add("Rblock", new CheckBox("Block Commands While Casting R"));
            RMenu.Add("Rnear", new CheckBox("Focus Targets Near Mouse Only"));
            RMenu.Add("Mradius", new Slider("Mouse Radius", 750, 300, 1500));

            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, 32));
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add(Q.Slot + "hit", new ComboBox("Q HitChance", 0, "High", "Medium", "Low"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add(W.Slot + "hit", new ComboBox("W HitChance", 0, "High", "Medium", "Low"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add(E.Slot + "hit", new ComboBox("E HitChance", 0, "High", "Medium", "Low"));

            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("key", new KeyBind("Harass Key", false, KeyBind.BindTypes.HoldActive, 'C'));
            HarassMenu.Add("toggle", new KeyBind("Auto Harass", false, KeyBind.BindTypes.PressToggle, 'H'));
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add(Q.Slot + "hit", new ComboBox("Q HitChance", 0, "High", "Medium", "Low"));
            HarassMenu.Add("Qmana", new Slider("Use Q if Mana% > [{0}%]"));
            HarassMenu.Add("W", new CheckBox("Use W"));
            HarassMenu.Add(W.Slot + "hit", new ComboBox("W HitChance", 0, "High", "Medium", "Low"));
            HarassMenu.Add("Wmana", new Slider("Use W if Mana% > [{0}%]"));
            HarassMenu.Add("E", new CheckBox("Use E"));
            HarassMenu.Add(E.Slot + "hit", new ComboBox("E HitChance", 0, "High", "Medium", "Low"));
            HarassMenu.Add("Emana", new Slider("Use E if Mana% > [{0}%]"));

            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("key", new KeyBind("LaneClear Key", false, KeyBind.BindTypes.HoldActive, 'V'));
            LaneClearMenu.Add("Q", new CheckBox("Use Q"));
            LaneClearMenu.Add("Qmode", new ComboBox("Q Mode", 0, "LaneClear", "LastHit", "Both"));
            LaneClearMenu.Add("Qmana", new Slider("Use Q if Mana% > [{0}%]"));
            LaneClearMenu.Add("W", new CheckBox("Use W"));
            LaneClearMenu.Add("Wmode", new ComboBox("W Mode", 0, "LaneClear", "LastHit", "Both"));
            LaneClearMenu.Add("Wmana", new Slider("Use W if Mana% > [{0}%]"));
            LaneClearMenu.Add("E", new CheckBox("Use E"));
            LaneClearMenu.Add("Emode", new ComboBox("E Mode", 0, "LaneClear", "LastHit", "Both"));
            LaneClearMenu.Add("Emana", new Slider("Use E if Mana% > [{0}%]"));

            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("key", new KeyBind("JungleClear Key", false, KeyBind.BindTypes.HoldActive, 'V'));
            JungleClearMenu.Add("Q", new CheckBox("Use Q"));
            JungleClearMenu.Add("Qmana", new Slider("Use Q if Mana% > [{0}%]"));
            JungleClearMenu.Add("W", new CheckBox("Use W"));
            JungleClearMenu.Add("Wmana", new Slider("Use W if Mana% > [{0}%]"));
            JungleClearMenu.Add("E", new CheckBox("Use E"));
            JungleClearMenu.Add("Emana", new Slider("Use E if Mana% > [{0}%]"));

            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("Q", new CheckBox("Use Q"));
            KillStealMenu.Add("W", new CheckBox("Use W"));
            KillStealMenu.Add("E", new CheckBox("Use E"));

            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("gap", new CheckBox("E Anti-GapCloser"));
            MiscMenu.Add("int", new CheckBox("E Interrupter"));
            MiscMenu.Add("danger", new ComboBox("Interrupter Danger Level", 1, "High", "Medium", "Low"));
            MiscMenu.Add("flee", new KeyBind("Escape with E", false, KeyBind.BindTypes.HoldActive, 'A'));
            var notifi = MiscMenu.Add("Notifications", new CheckBox("Use Notifications"));
            MiscMenu.Add("autoECC", new CheckBox("Auto E On CC enemy"));
            MiscMenu.Add("scrybebuy", new CheckBox("Auto Scrybing Orb Buy"));
            MiscMenu.Add("scrybebuylevel", new Slider("Buy Orb at level [{0}]", 9, 1, 18));
            MiscMenu.AddGroupLabel("Anti-GapCloser Spells");
            foreach (var spell in
                from spell in Gapcloser.GapCloserList
                from enemy in EntityManager.Heroes.Enemies.Where(enemy => spell.ChampName == enemy.ChampionName)
                select spell)
            {
                MiscMenu.Add(spell.SpellName, new CheckBox(spell.ChampName + " - " + spell.SpellSlot));
            }

            foreach (var spell in SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
            }

            DrawMenu.Add("Rmini", new CheckBox("Draw R Range (MiniMap)", false));

            foreach (var spell in SpellList)
            {
                ColorMenu.Add(spell.Slot + "Color", new ColorPicker(spell.Slot + " Color", Color.Chartreuse));
            }

            if (notifi.CurrentValue)
            {
                Common.ShowNotification("KappaXerath - Loaded", 5000);
            }

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Player.OnIssueOrder += Player_OnIssueOrder;
            GameObject.OnCreate += GameObject_OnCreate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (IsCastingR && args.Slot != SpellSlot.R)
            {
                args.Process = false;
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && MiscMenu["gap"].Cast<CheckBox>().CurrentValue && rengar.IsValidTarget(E.Range))
                {
                    E.Cast(rengar);
                }
            }
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (IsCastingR && RMenu["Rblock"].Cast<CheckBox>().CurrentValue)
            {
                args.Process = false;
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            args.Process = AttacksEnabled;
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || e == null || !E.IsReady() || !MiscMenu[e.SpellName].Cast<CheckBox>().CurrentValue
                || e.End == Vector3.Zero || !MiscMenu["gap"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            if (e.End.IsInRange(Player.Instance, 650) || sender.IsValidTarget(650))
            {
                E.Cast(sender);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                Chat.Print(args.SData.Name);
                LastCastedSpell.Name = args.SData.Name;

                LastCastedSpell.Time = args.Time;

                switch (args.SData.Name)
                {
                    case "XerathLocusOfPower2":
                        RCharge.CastT = 0;
                        RCharge.Index = 0;
                        RCharge.Position = new Vector3();
                        RCharge.TapKeyPressed = false;
                        break;
                    case "XerathLocusPulse":
                        RCharge.CastT = Core.GameTickCount;
                        RCharge.Index++;
                        RCharge.Position = args.End;
                        RCharge.TapKeyPressed = false;
                        break;
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || e == null || !E.IsReady() || e.DangerLevel < Common.danger() || !sender.IsValidTarget(500)
                || !MiscMenu["int"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            E.Cast(sender);
        }

        private static void Combo()
        {
            UseSpells(
                ComboMenu["Q"].Cast<CheckBox>().CurrentValue,
                Common.hitchance(Q, ComboMenu),
                ComboMenu["W"].Cast<CheckBox>().CurrentValue,
                Common.hitchance(W, ComboMenu),
                ComboMenu["E"].Cast<CheckBox>().CurrentValue,
                Common.hitchance(E, ComboMenu));
        }

        private static void Harass()
        {
            UseSpells(
                HarassMenu["Q"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent > HarassMenu["Qmana"].Cast<Slider>().CurrentValue,
                Common.hitchance(Q, HarassMenu),
                HarassMenu["W"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent > HarassMenu["Wmana"].Cast<Slider>().CurrentValue,
                Common.hitchance(W, HarassMenu),
                HarassMenu["E"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent > HarassMenu["Emana"].Cast<Slider>().CurrentValue,
                Common.hitchance(E, HarassMenu));
        }

        private static void UseSpells(bool useQ, HitChance Qhit, bool useW, HitChance Whit, bool useE, HitChance Ehit)
        {
            var qTarget = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range + W.Width * 0.5f, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (eTarget != null && useE && E.IsReady() && (E.GetPrediction(eTarget).HitChance >= Ehit || Common.IsCC(eTarget)))
            {
                if (Player.Instance.Distance(eTarget) < E.Range * 0.4f)
                {
                    E.Cast(eTarget);
                }
                else if (!useW || !W.IsReady())
                {
                    E.Cast(eTarget);
                }
            }

            if (useQ && Q.IsReady() && qTarget != null && (Q.GetPrediction(qTarget).HitChance >= Qhit || Common.IsCC(qTarget)))
            {
                if (Q.IsCharging)
                {
                    Q.Cast(qTarget);
                }
                else if (!useW || !W.IsReady() || Player.Instance.Distance(qTarget) > W.Range)
                {
                    Q.StartCharging();
                }
            }

            if (wTarget != null && useW && W.IsReady() && (W.GetPrediction(wTarget).HitChance >= Whit || Common.IsCC(wTarget)))
            {
                W.Cast(wTarget);
            }
        }

        private static AIHeroClient GetTargetNearMouse(float distance)
        {
            AIHeroClient bestTarget = null;
            var bestRatio = 0f;
            var target = TargetSelector.SelectedTarget;
            if (target.IsValidTarget() && Common.ValidUlt(target)
                && (Game.CursorPos.Distance(target.ServerPosition) < distance && Player.Instance.Distance(target) < R.Range))
            {
                return TargetSelector.SelectedTarget;
            }

            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                if (!hero.IsValidTarget(R.Range) || !Common.ValidUlt(hero) || Game.CursorPos.Distance(hero.ServerPosition) > distance)
                {
                    continue;
                }

                var damage = Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical, 100);
                var ratio = damage / (1 + hero.Health) * TargetSelector.GetPriority(hero);

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    bestTarget = hero;
                }
            }

            return bestTarget;
        }

        private static void WhileCastingR()
        {
            if (!RMenu["R"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            var rMode = RMenu["Rmode"].Cast<ComboBox>().CurrentValue;

            var rTarget = RMenu["Rnear"].Cast<CheckBox>().CurrentValue
                              ? GetTargetNearMouse(RMenu["Mradius"].Cast<Slider>().CurrentValue)
                              : TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (rTarget != null)
            {
                if (rTarget.TotalShieldHealth() - R.GetDamage(rTarget) < 0)
                {
                    if (Core.GameTickCount - RCharge.CastT <= 0)
                    {
                        return;
                    }
                }

                if (RCharge.Index != 0 && rTarget.Distance(RCharge.Position) > 1000)
                {
                    if (Core.GameTickCount - RCharge.CastT <= Math.Min(2500, rTarget.Distance(RCharge.Position) - 1000))
                    {
                        return;
                    }
                }

                if (R.GetPrediction(rTarget).HitChance >= Common.hitchance(R, RMenu) || Common.IsCC(rTarget))
                {
                    switch (rMode)
                    {
                        case 0:
                            R.Cast(rTarget);
                            break;

                        case 1:
                            var delay = RMenu["delay" + (RCharge.Index + 1)].Cast<Slider>().CurrentValue;
                            if (Core.GameTickCount - RCharge.CastT > delay)
                            {
                                R.Cast(rTarget);
                            }
                            break;

                        case 2:
                            if (RCharge.TapKeyPressed)
                            {
                                R.Cast(rTarget);
                            }
                            break;
                    }
                }
            }
        }

        private static void QKS()
        {
            foreach (var hero in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(Q.MaximumRange)))
            {
                if (hero == null)
                {
                    return;
                }

                if (Q.IsReady())
                {
                    if (hero.TotalShieldHealth() < Q.GetDamage(hero))
                    {
                        if (!Q.IsCharging)
                        {
                            Q.StartCharging();
                            return;
                        }
                        if (Q.IsCharging)
                        {
                            Q.Cast(hero);
                        }
                    }
                }
            }
        }

        private static void WKS()
        {
            foreach (var hero in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(W.Range)))
            {
                if (hero == null)
                {
                    return;
                }

                var predictionW = W.GetPrediction(hero);

                if (W.IsReady())
                {
                    if (hero.Health < W.GetDamage(hero))
                    {
                        if (predictionW.HitChance >= HitChance.High)
                        {
                            W.Cast(hero);
                        }
                    }
                }
            }
        }

        private static void EKS()
        {
            foreach (var hero in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(W.Range)))
            {
                if (hero == null)
                {
                    return;
                }

                var predictionE = E.GetPrediction(hero);

                if (E.IsReady())
                {
                    if (hero.Health < E.GetDamage(hero))
                    {
                        if (predictionE.HitChance >= HitChance.High)
                        {
                            E.Cast(hero);
                        }
                    }
                }
            }
        }

        private static void Farm()
        {
            var useQ = LaneClearMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady()
                       && Player.Instance.ManaPercent > LaneClearMenu["Qmana"].Cast<Slider>().CurrentValue;
            var useW = LaneClearMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady()
                       && Player.Instance.ManaPercent > LaneClearMenu["Wmana"].Cast<Slider>().CurrentValue;
            var useE = LaneClearMenu["E"].Cast<CheckBox>().CurrentValue && W.IsReady()
                       && Player.Instance.ManaPercent > LaneClearMenu["Emana"].Cast<Slider>().CurrentValue;
            var allMinionsQ = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(Q.MaximumRange));
            var allMinionsW = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(W.Range));
            var objAiMinionsQ = allMinionsQ as Obj_AI_Minion[] ?? allMinionsQ.ToArray();
            var objAiMinionsW = allMinionsW as Obj_AI_Minion[] ?? allMinionsW.ToArray();

            if (useQ && allMinionsQ != null)
            {
                var Qpos = EntityManager.MinionsAndMonsters.GetLineFarmLocation(objAiMinionsQ.ToArray(), Q.Width, (int)Q.MaximumRange);
                var useQi = LaneClearMenu["Qmode"].Cast<ComboBox>().CurrentValue;

                if (useQi == 0 || useQi == 2)
                {
                    if (Q.IsCharging)
                    {
                        var locQ = Qpos.CastPosition;
                        if (Qpos.HitNumber >= 1)
                        {
                            Q.Cast(locQ);
                        }
                    }
                    else if (Qpos.HitNumber > 0)
                    {
                        Q.StartCharging();
                    }
                }
                if (useQi == 1 || useQi == 2)
                {
                    var minion = objAiMinionsQ.FirstOrDefault(m => Q.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay));
                    if (Q.IsCharging && minion != null)
                    {
                        Q.Cast(minion);
                    }
                    else if (minion != null)
                    {
                        Q.StartCharging();
                    }
                }
            }

            if (useW && allMinionsW != null)
            {
                var Wpos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    allMinionsW.ToArray(),
                    W.Width,
                    (int)W.Range,
                    W.CastDelay,
                    W.Speed);
                var useWi = LaneClearMenu["Wmode"].Cast<ComboBox>().CurrentValue;

                if (useWi == 0 || useWi == 2)
                {
                    var locW = Wpos.CastPosition;
                    if (Wpos.HitNumber >= 1)
                    {
                        W.Cast(locW);
                    }
                }

                if (useWi == 1 || useWi == 2)
                {
                    var minion = objAiMinionsW.FirstOrDefault(m => W.GetDamage(m) >= Prediction.Health.GetPrediction(m, W.CastDelay));
                    if (minion != null)
                    {
                        W.Cast(minion);
                    }
                }
            }

            if (useE)
            {
                var useEi = LaneClearMenu["Emode"].Cast<ComboBox>().CurrentValue;
                foreach (var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(E.Range)))
                {
                    if (minion != null && (useEi == 0 || useEi == 2))
                    {
                        E.Cast(minion);
                    }

                    if (minion != null && (useEi == 1 || useEi == 2))
                    {
                        if (E.GetDamage(minion) >= Prediction.Health.GetPrediction(minion, E.CastDelay))
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }
        }

        private static void JungleFarm()
        {
            var useQ = JungleClearMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady()
                       && Player.Instance.ManaPercent > JungleClearMenu["Qmana"].Cast<Slider>().CurrentValue;
            var useW = JungleClearMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady()
                       && Player.Instance.ManaPercent > JungleClearMenu["Wmana"].Cast<Slider>().CurrentValue;
            var useE = JungleClearMenu["E"].Cast<CheckBox>().CurrentValue && W.IsReady()
                       && Player.Instance.ManaPercent > JungleClearMenu["Emana"].Cast<Slider>().CurrentValue;
            var allMinionsQ = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsValidTarget(Q.MaximumRange));
            var allMinionsW = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsValidTarget(W.Range));
            var objAiMinionsQ = allMinionsQ as Obj_AI_Minion[] ?? allMinionsQ.ToArray();
            var objAiMinionsW = allMinionsW as Obj_AI_Minion[] ?? allMinionsW.ToArray();

            if (useQ && allMinionsQ != null)
            {
                var Qpos = EntityManager.MinionsAndMonsters.GetLineFarmLocation(objAiMinionsQ.ToArray(), Q.Width, (int)Q.MaximumRange);

                if (Q.IsCharging)
                {
                    var locQ = Qpos.CastPosition;
                    if (Qpos.HitNumber >= 1)
                    {
                        Q.Cast(locQ);
                    }
                }
                else if (Qpos.HitNumber > 0)
                {
                    Q.StartCharging();
                }

                var minion = objAiMinionsQ.FirstOrDefault(m => Q.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay));
                if (Q.IsCharging && minion != null)
                {
                    Q.Cast(minion);
                }
                else if (minion != null)
                {
                    Q.StartCharging();
                }
            }

            if (useW && allMinionsW != null)
            {
                var Wpos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    objAiMinionsW.ToArray(),
                    W.Width,
                    (int)W.Range,
                    W.CastDelay,
                    W.Speed);

                var locW = Wpos.CastPosition;
                if (Wpos.HitNumber >= 1)
                {
                    W.Cast(locW);
                }

                var minion = objAiMinionsW.FirstOrDefault(m => W.GetDamage(m) >= Prediction.Health.GetPrediction(m, W.CastDelay));
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }

            if (useE)
            {
                foreach (var minion in EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsValidTarget(E.Range)))
                {
                    if (E.GetDamage(minion) >= Prediction.Health.GetPrediction(minion, E.CastDelay))
                    {
                        E.Cast(minion);
                    }
                    E.Cast(minion);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (MiscMenu["flee"].Cast<KeyBind>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null)
                {
                    E.Cast(target);
                }
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }

            if (KillStealMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                QKS();
            }
            if (KillStealMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                WKS();
            }
            if (KillStealMenu["E"].Cast<CheckBox>().CurrentValue)
            {
                EKS();
            }

            if (MiscMenu["autoECC"].Cast<CheckBox>().CurrentValue)
            {
                foreach (
                    var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range) && Common.IsCC(e)).Where(enemy => enemy != null))
                {
                    E.Cast(enemy);
                }
            }

            ScrybingOrb();

            if (Player.Instance.IsDead)
            {
                return;
            }

            R.Range = (uint)(1925 + R.Level * 1200);

            if (ComboMenu["key"].Cast<KeyBind>().CurrentValue)
            {
                Combo();
            }

            if (HarassMenu["key"].Cast<KeyBind>().CurrentValue || HarassMenu["toggle"].Cast<KeyBind>().CurrentValue)
            {
                Harass();
            }

            if (LaneClearMenu["key"].Cast<KeyBind>().CurrentValue)
            {
                Farm();
            }

            if (JungleClearMenu["key"].Cast<KeyBind>().CurrentValue)
            {
                JungleFarm();
            }

            Orbwalker.DisableAttacking = IsCastingR;
            Orbwalker.DisableMovement = IsCastingR;
            RCharge.TapKeyPressed = RMenu["Rtap"].Cast<KeyBind>().CurrentValue;

            if (IsCastingR)
            {
                WhileCastingR();
                return;
            }

            if (R.IsReady() && MiscMenu["Notifications"].Cast<CheckBox>().CurrentValue && Environment.TickCount - lastNotification > 5000)
            {
                foreach (var enemy in
                    EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget() && R.GetDamage(h) * 3 > h.Health))
                {
                    Common.ShowNotification(enemy.ChampionName + ": is killable R!!!", 4000);
                    lastNotification = Environment.TickCount;
                }
            }
        }

        public static void scrybeorbuse()
        {
            if (!RMenu["scrybR"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (Player.Instance.Distance(R.GetPrediction(target).CastPosition) > Player.Instance.AttackRange)
            {
                if (Scryb.IsOwned(Player.Instance))
                {
                    Scryb.Cast(target);
                }
            }
        }

        public static void ScrybingOrb()
        {
            var level = MiscMenu["scrybebuylevel"].Cast<Slider>().CurrentValue;
            var buy = MiscMenu["scrybebuy"].Cast<CheckBox>().CurrentValue;

            if (!buy)
            {
                return;
            }

            if (hasbought)
            {
                return;
            }

            if (!Scryb.IsOwned(Player.Instance) && Player.Instance.IsInShopRange() && Player.Instance.Level >= level)
            {
                Scryb.Buy();
                hasbought = true;
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Player.Instance.IsDead)
            {
                return;
            }

            var Rcirclemap = DrawMenu["Rmini"].Cast<CheckBox>().CurrentValue;

            if (Rcirclemap && R.IsReady())
            {
                Common.DrawCricleMinimap(Color.White, R.Range, Player.Instance.ServerPosition, 2, 20);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (IsCastingR)
            {
                if (RMenu["Rnear"].Cast<CheckBox>().CurrentValue)
                {
                    Circle.Draw(SharpDX.Color.Red, RMenu["Mradius"].Cast<Slider>().CurrentValue, Game.CursorPos);
                }
            }

            foreach (var spell in SpellList)
            {
                var color = ColorMenu[spell.Slot + "Color"].Cast<ColorPicker>().CurrentValue;
                if (DrawMenu[spell.Slot.ToString()].Cast<CheckBox>().CurrentValue)
                {
                    Circle.Draw(new ColorBGRA(color.R, color.G, color.B, color.A), spell.Range, Player.Instance.ServerPosition);
                }
            }

            if (MiscMenu["Notifications"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage * 5 > t.Health)
                    {
                        Drawing.DrawText(
                            Drawing.Width * 0.1f,
                            Drawing.Height * 0.5f,
                            Color.Red,
                            (int)(t.Health / rDamage) + " x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        Common.drawLine(t.Position, Player.Instance.Position, 10, Color.Yellow);
                    }
                }
            }
        }
    }
}