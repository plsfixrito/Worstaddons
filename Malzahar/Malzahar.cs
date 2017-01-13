namespace Malzahar
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

    internal static class Malzahar
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Skillshot W { get; private set; }

        public static Spell.Targeted E { get; private set; }

        public static Spell.Targeted R { get; private set; }

        private static Menu menuIni;

        public static Menu Combo { get; private set; }

        public static Menu Harass { get; private set; }

        public static Menu LaneClear { get; private set; }

        public static Menu JungleClear { get; private set; }

        public static Menu KillSteal { get; private set; }

        public static Menu Misc { get; private set; }

        public static Menu DrawMenu { get; private set; }

        public static bool IsKillable(this Obj_AI_Base target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                   && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        public static bool IsCC(this Obj_AI_Base target)
        {
            return target.IsStunned || target.IsRooted || target.IsTaunted || target.IsCharmed || target.Spellbook.IsChanneling
                   || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup)
                   || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression)
                   || target.HasBuffOfType(BuffType.Taunt);
        }

        public static float GetDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            float dmg = 0f;
            var AP = Player.Instance.FlatMagicDamageMod;
            var AD = Player.Instance.FlatPhysicalDamageMod;
            var slotLevel = Player.GetSpell(spell.Slot).Level - 1;

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    {
                        dmg += new float[] { 70, 110, 150, 190, 230 }[slotLevel] + 0.70f * AP;
                    }
                    break;
                case SpellSlot.W:
                    {
                        dmg += (new float[] { 30, 33, 35, 37, 40 }[slotLevel] + 0.40f * AD)
                               + (new float[] { 10, 15, 20, 25, 30 }[slotLevel] + 0.10f * AP);
                    }
                    break;
                case SpellSlot.E:
                    {
                        dmg += new float[] { 80, 115, 150, 185, 220 }[slotLevel] + 0.70f * AP;
                    }
                    break;
                case SpellSlot.R:
                    {
                        dmg += new float[] { target.MaxHealth * 0.25f, target.MaxHealth * 0.35f, target.MaxHealth * 0.45f }[slotLevel]
                               + (0.07f * (AP / 100));
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, dmg - 10);
        }

        public static float GetDamage(Obj_AI_Base target)
        {
            float dmg = 0f;
            var AP = Player.Instance.FlatMagicDamageMod;
            var AD = Player.Instance.FlatPhysicalDamageMod;

            if (Q.IsReady())
            {
                dmg += new float[] { 70, 110, 150, 190, 230 }[Player.GetSpell(SpellSlot.Q).Level - 1] + 0.70f * AP;
            }
            if (W.IsReady())
            {
                dmg += (new float[] { 30, 33, 35, 37, 40 }[Player.GetSpell(SpellSlot.W).Level - 1] + 0.40f * AD)
                       + (new float[] { 10, 15, 20, 25, 30 }[Player.GetSpell(SpellSlot.W).Level - 1] + 0.10f * AP);
            }
            if (E.IsReady())
            {
                dmg += new float[] { 80, 115, 150, 185, 220 }[Player.GetSpell(SpellSlot.E).Level - 1] + 0.7f * AP;
            }
            if (R.IsReady())
            {
                dmg +=
                    new float[] { target.MaxHealth * 0.25f, target.MaxHealth * 0.35f, target.MaxHealth * 0.45f }[
                        Player.GetSpell(SpellSlot.R).Level - 1] + (0.07f * (AP / 100));
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, dmg - 25);
        }

        private static Color colorselector(Spell.SpellBase slot)
        {
            switch (DrawMenu[slot.Name].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    {
                        return Color.Aqua;
                    }
                case 1:
                    {
                        return Color.BlueViolet;
                    }
                case 2:
                    {
                        return Color.Chartreuse;
                    }
                case 3:
                    {
                        return Color.Purple;
                    }
                case 4:
                    {
                        return Color.White;
                    }
                case 5:
                    {
                        return Color.Orange;
                    }
                case 6:
                    {
                        return Color.Green;
                    }
            }
            return Color.White;
        }

        private static DangerLevel danger()
        {
            switch (Misc["danger"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    {
                        return DangerLevel.High;
                    }
                case 1:
                    {
                        return DangerLevel.Medium;
                    }
                case 2:
                    {
                        return DangerLevel.Low;
                    }
            }
            return DangerLevel.High;
        }

        private static bool IsCastingR;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Malzahar)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Circular, 250, 500, 90);
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 250, int.MaxValue, 80);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Targeted(SpellSlot.R, 700);

            menuIni = MainMenu.AddMenu("Malzahar", "Malzahar");
            Combo = menuIni.AddSubMenu("Combo Settings");
            Harass = menuIni.AddSubMenu("Harass Settings");
            LaneClear = menuIni.AddSubMenu("LaneClear Settings");
            JungleClear = menuIni.AddSubMenu("JungleClear Settings");
            KillSteal = menuIni.AddSubMenu("KillSteal Settings");
            Misc = menuIni.AddSubMenu("Misc Settings");
            DrawMenu = menuIni.AddSubMenu("Drawings Settings");

            Combo.AddGroupLabel("Combo");
            Combo.Add("Q", new CheckBox("Use Q"));
            Combo.Add("W", new CheckBox("Use W"));
            Combo.Add("E", new CheckBox("Use E"));
            Combo.Add("RCombo", new CheckBox("Use R Combo"));
            Combo.Add("RFinisher", new CheckBox("Use R Finisher"));
            Combo.Add("RTurret", new CheckBox("Use R if enemy Under Ally Turret"));
            Combo.AddSeparator(0);
            Combo.AddGroupLabel("Don't Use Ult On:");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                CheckBox cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                Combo.Add("DontUlt" + enemy.BaseSkinName, cb);
            }

            Harass.AddGroupLabel("Harass");
            Harass.Add("Q", new CheckBox("Use Q"));
            Harass.Add("W", new CheckBox("Use W"));
            Harass.Add("E", new CheckBox("Use E"));
            Harass.Add("mana", new Slider("Use if Mana% is more than [{0}%]", 65));

            LaneClear.AddGroupLabel("LaneClear");
            LaneClear.Add("Q", new CheckBox("Use Q"));
            LaneClear.Add("W", new CheckBox("Use W"));
            LaneClear.Add("E", new CheckBox("Use E"));
            LaneClear.Add("mana", new Slider("Use if Mana% is more than [{0}%]", 65));

            JungleClear.AddGroupLabel("JungleClear");
            JungleClear.Add("Q", new CheckBox("Use Q"));
            JungleClear.Add("W", new CheckBox("Use W"));
            JungleClear.Add("E", new CheckBox("Use E"));
            JungleClear.Add("mana", new Slider("Use if Mana% is more than [{0}%]", 65));

            KillSteal.AddGroupLabel("KillSteal");
            KillSteal.Add("Q", new CheckBox("Use Q"));
            KillSteal.Add("W", new CheckBox("Use W"));
            KillSteal.Add("E", new CheckBox("Use E"));
            KillSteal.Add("R", new CheckBox("Use R"));
            KillSteal.AddGroupLabel("Don't Use Ult On:");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                CheckBox cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                KillSteal.Add("DontUlt" + enemy.BaseSkinName, cb);
            }

            Misc.AddGroupLabel("Misc");
            Misc.Add("RSave", new CheckBox("Block All Commands When Casting R"));
            Misc.Add("Qgap", new CheckBox("Q on GapCloser"));
            Misc.Add("Rgap", new CheckBox("R on GapCloser"));
            Misc.Add("Qint", new CheckBox("Q interrupt DangerSpells"));
            Misc.Add("Rint", new CheckBox("R interrupt DangerSpells"));
            Misc.Add("RTurret", new CheckBox("R Enemy Under Ally Tower"));
            Misc.Add("blockR", new CheckBox("Block R under Enemy Turret", false));
            Misc.Add("danger", new ComboBox("Spells DangerLevel to interrupt", 2, "High", "Medium", "Low"));

            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");
            DrawMenu.AddSeparator(0);
            DrawMenu.Add("Q", new CheckBox("Draw Q Range"));
            DrawMenu.Add(Q.Name, new ComboBox("Q Color", 0, "Aqua", "BlueViolet", "Chartreuse", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(0);
            DrawMenu.Add("W", new CheckBox("Draw W Range"));
            DrawMenu.Add(W.Name, new ComboBox("W Color", 1, "Aqua", "BlueViolet", "Chartreuse", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(0);
            DrawMenu.Add("E", new CheckBox("Draw E Range"));
            DrawMenu.Add(E.Name, new ComboBox("E Color", 2, "Aqua", "BlueViolet", "Chartreuse", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(0);
            DrawMenu.Add("R", new CheckBox("Draw R Range"));
            DrawMenu.Add(R.Name, new ComboBox("R Color", 3, "Aqua", "BlueViolet", "Chartreuse", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(0);

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Player.OnIssueOrder += Player_OnIssueOrder;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var Eready = LaneClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady();
                if (Eready && E.GetDamage(target) >= args.RemainingHealth)
                {
                    E.Cast(target);
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || IsCastingR)
            {
                return;
            }

            if (Misc["Qgap"].Cast<CheckBox>().CurrentValue && (e.End.IsInRange(Player.Instance, Q.Range) || sender.IsValidTarget(Q.Range)))
            {
                Q.Cast(sender);
            }

            if (Misc["blockR"].Cast<CheckBox>().CurrentValue && Player.Instance.IsUnderEnemyturret())
            {
                return;
            }

            if (Misc["Rgap"].Cast<CheckBox>().CurrentValue && (e.End.IsInRange(Player.Instance, R.Range) || sender.IsValidTarget(R.Range)))
            {
                R.Cast(sender);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && Misc["Rgap"].Cast<CheckBox>().CurrentValue && rengar.IsValidTarget(R.Range))
                {
                    R.Cast(rengar);
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || IsCastingR)
            {
                return;
            }

            if (danger() >= e.DangerLevel)
            {
                if (Misc["Qint"].Cast<CheckBox>().CurrentValue && sender.IsValidTarget(Q.Range))
                {
                    Q.Cast(sender);
                }

                if (Misc["blockR"].Cast<CheckBox>().CurrentValue && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                if (Misc["Rint"].Cast<CheckBox>().CurrentValue && sender.IsValidTarget(R.Range))
                {
                    R.Cast(sender);
                }
            }
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Misc["Rsave"].Cast<CheckBox>().CurrentValue && IsCastingR)
            {
                args.Process = false;
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
            {
                return;
            }

            if (Misc["Rsave"].Cast<CheckBox>().CurrentValue && IsCastingR)
            {
                args.Process = false;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            IsCastingR = Player.Instance.Buffs.FirstOrDefault(b => b.Name.ToLower().Contains("malzaharrsound")) != null;
            Orbwalker.DisableAttacking = IsCastingR;
            Orbwalker.DisableMovement = IsCastingR;

            if (IsCastingR)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ComboLogic();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                && Player.Instance.ManaPercent >= Harass["mana"].Cast<Slider>().CurrentValue)
            {
                HarassLogic();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                && Player.Instance.ManaPercent >= LaneClear["mana"].Cast<Slider>().CurrentValue)
            {
                LaneClearLogic();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                && Player.Instance.ManaPercent >= JungleClear["mana"].Cast<Slider>().CurrentValue)
            {
                JungleClearLogic();
            }
            Rlogic();
            KillStealLogic();
        }

        private static void Rlogic()
        {
            if (Misc["RTurret"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                if (Misc["blockR"].Cast<CheckBox>().CurrentValue && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                var targets =
                    EntityManager.Heroes.Enemies.Where(
                        e => e.IsUnderTurret() && !e.IsUnderHisturret() && !e.IsUnderEnemyturret() && e.IsValidTarget(R.Range));
                if (targets != null)
                {
                    foreach (var target in targets.Where(target => target != null))
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void ComboLogic()
        {
            if (IsCastingR)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range + 50, DamageType.Mixed);

            if (target == null || !target.IsKillable())
            {
                return;
            }

            var Qready = Combo["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range);
            var Wready = Combo["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range)
                         && W.GetPrediction(target).HitChance >= HitChance.High;
            var Eready = Combo["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range);
            var Rfinready = Combo["RFinisher"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range);
            var Rcomready = Combo["RCombo"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range);
            var RTurret = Combo["RTurret"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range) && target.IsUnderTurret()
                          && !target.IsUnderHisturret() && !target.IsUnderEnemyturret();

            if (Wready)
            {
                W.Cast(target);
            }

            if (Qready && (Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()))
            {
                Q.Cast(target);
            }

            if (Eready)
            {
                E.Cast(target);
            }

            if (!Combo["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                if (Misc["blockR"].Cast<CheckBox>().CurrentValue && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                if (Rcomready && GetDamage(target) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                {
                    R.Cast(target);
                }

                if (Rfinready && R.GetDamage(target) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                {
                    R.Cast(target);
                }

                if (RTurret)
                {
                    R.Cast(target);
                }
            }
        }

        private static void HarassLogic()
        {
            if (IsCastingR)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range + 50, DamageType.Mixed);

            if (target == null || !target.IsKillable())
            {
                return;
            }

            var Qready = Harass["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range);
            var Wready = Harass["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range)
                         && W.GetPrediction(target).HitChance >= HitChance.High;
            var Eready = Harass["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range);

            if (Wready)
            {
                W.Cast(target);
            }

            if (Qready && (Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()))
            {
                Q.Cast(target);
            }

            if (Eready)
            {
                E.Cast(target);
            }
        }

        private static void LaneClearLogic()
        {
            if (IsCastingR)
            {
                return;
            }

            var Qready = LaneClear["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var Wready = LaneClear["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var Eready = LaneClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady();

            if (Wready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsValidTarget(W.Range));

                if (minions != null)
                {
                    var location =
                        Prediction.Position.PredictCircularMissileAoe(
                            minions.Cast<Obj_AI_Base>().ToArray(),
                            W.Range,
                            W.Radius + 100,
                            W.CastDelay,
                            W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                    if (location != null && location.CollisionObjects.Length >= 2)
                    {
                        W.Cast(location.CastPosition);
                    }
                }
            }

            if (Qready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsValidTarget(Q.Range + 50));

                if (minions != null)
                {
                    var location =
                        Prediction.Position.PredictCircularMissileAoe(
                            minions.Cast<Obj_AI_Base>().ToArray(),
                            Q.Range,
                            Q.Radius + 50,
                            Q.CastDelay,
                            Q.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                    if (location != null && location.CollisionObjects.Length >= 2)
                    {
                        Q.Cast(location.CastPosition);
                    }
                }
            }

            if (Eready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsValidTarget(E.Range));

                if (minions != null)
                {
                    foreach (var minion in minions.Where(minion => E.GetDamage(minion) >= minion?.TotalShield()))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        private static void JungleClearLogic()
        {
            if (IsCastingR)
            {
                return;
            }

            var Qready = JungleClear["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var Wready = JungleClear["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var Eready = JungleClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady();

            if (Wready)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(e => e.MaxHealth)
                        .FirstOrDefault(m => m.IsKillable() && m.IsValidTarget(W.Range));
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }

            if (Qready)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(e => e.MaxHealth)
                        .FirstOrDefault(m => m.IsKillable() && m.IsValidTarget(Q.Range));
                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }

            if (Eready)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsKillable() && m.IsValidTarget(E.Range));

                if (minions != null)
                {
                    foreach (var minion in minions.Where(minion => E.GetDamage(minion) >= minion?.TotalShield()))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        private static void KillStealLogic()
        {
            if (IsCastingR)
            {
                return;
            }

            var Qready = KillSteal["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var Wready = KillSteal["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var Eready = KillSteal["E"].Cast<CheckBox>().CurrentValue && E.IsReady();
            var Rready = KillSteal["R"].Cast<CheckBox>().CurrentValue && R.IsReady();

            var Qksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(Q.Range));
            var Wksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(W.Range));
            var Eksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(E.Range));

            if (Qready)
            {
                if (Qksenemy != null)
                {
                    foreach (
                        var enemy in Qksenemy.Where(enemy => Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Q.CastDelay))
                        )
                    {
                        Q.Cast(enemy);
                    }
                }
            }

            if (Wready)
            {
                if (Wksenemy != null)
                {
                    foreach (
                        var enemy in Wksenemy.Where(enemy => W.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, W.CastDelay))
                        )
                    {
                        W.Cast(enemy);
                    }
                }
            }

            if (Eready)
            {
                if (Eksenemy != null)
                {
                    foreach (
                        var enemy in Eksenemy.Where(enemy => E.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, E.CastDelay))
                        )
                    {
                        E.Cast(enemy);
                    }
                }
            }

            if (Rready)
            {
                if (Misc["blockR"].Cast<CheckBox>().CurrentValue && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                var ksenemy =
                    EntityManager.Heroes.Enemies.Where(
                        e => e.IsKillable() && e.IsValidTarget(R.Range) && !KillSteal["DontUlt" + e.BaseSkinName].Cast<CheckBox>().CurrentValue);
                if (ksenemy != null)
                {
                    foreach (
                        var enemy in ksenemy.Where(enemy => R.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, R.CastDelay)))
                    {
                        if (Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Q.CastDelay)
                            || W.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, W.CastDelay)
                            || E.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, E.CastDelay))
                        {
                            return;
                        }

                        R.Cast(enemy);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Q.IsReady() ? colorselector(Q) : Color.Red, Q.Range, Player.Instance.Position);
            }

            if (DrawMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(W.IsReady() ? colorselector(W) : Color.Red, W.Range, Player.Instance.Position);
            }

            if (DrawMenu["E"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(E.IsReady() ? colorselector(E) : Color.Red, E.Range, Player.Instance.Position);
            }

            if (DrawMenu["R"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(R.IsReady() ? colorselector(R) : Color.Red, R.Range, Player.Instance.Position);
            }

            if (DrawMenu["damage"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsHPBarRendered))
                {
                    if (enemy != null)
                    {
                        var hpx = enemy.HPBarPosition.X;
                        var hpy = enemy.HPBarPosition.Y;
                        var damage = (int)GetDamage(enemy) + "/" + (int)enemy.TotalShieldHealth();
                        var c = System.Drawing.Color.GreenYellow;

                        if (GetDamage(enemy) >= enemy.TotalShieldHealth() / 2)
                        {
                            damage = "Harass for Kill: " + (int)GetDamage(enemy) + "/" + (int)enemy.TotalShieldHealth();
                            c = System.Drawing.Color.Orange;
                        }

                        if (GetDamage(enemy) >= enemy.TotalShieldHealth())
                        {
                            damage = "Killable: " + (int)GetDamage(enemy) + "/" + (int)enemy.TotalShieldHealth();
                            c = System.Drawing.Color.Red;
                        }

                        Drawing.DrawText(hpx + 145, hpy, c, damage, 3);
                    }
                }
            }
        }
    }
}