using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Khappa_Zix.Common
{
    public static class Extensions
    {
        /// <summary>
        ///     Returns true if the Item IsReady.
        /// </summary>
        public static bool ItemReady(this Item item, Menu menu)
        {
            return item != null && item.IsOwned(Player.Instance) && item.IsReady() && menu.CheckBoxValue(item.Id.ToString());
        }

        /// <summary>
        ///     Returns if no orbwalker modes are active
        /// </summary>
        public static bool NoModesActive
        {
            get
            {
                return !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                       && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                       && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
            }
        }
        
        public static bool IsIsolated(this Obj_AI_Minion target)
        {
            if (EntityManager.MinionsAndMonsters.GetJungleMonsters().Contains(target))
            {
                return EntityManager.MinionsAndMonsters.GetJungleMonsters().Count(m => m.IsValidTarget() && m.IsInRange(target, 500)) <= 1;
            }
            return target.CountEnemyHeroesInRangeWithPrediction(500) <= 1 && target.CountEnemyMinionsInRangeWithPrediction(500) == 0 && !target.IsUnderEnemyturret();
        }
        public static bool IsIsolated(this Obj_AI_Base target)
        {
            return target.CountEnemyHeroesInRangeWithPrediction(500) <= 1 && target.CountEnemyMinionsInRangeWithPrediction(500) == 0 && !target.IsUnderEnemyturret();
        }

        public static bool UnderEnemyTurret(this Vector3 pos)
        {
            return EntityManager.Turrets.Enemies.Any(t => t.IsValidTarget() && pos.IsInRange(t, t.GetAutoAttackRange()));
        }

        /// <summary>
        ///     Supported Jungle Mobs.
        /// </summary>
        public static string[] ACSJunglemobs = { "AscXerath" };

        /// <summary>
        ///     Supported Jungle Mobs.
        /// </summary>
        public static string[] SRJunglemobs = {
            "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water",
            "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak",
            "Sru_Crab", "SRU_Murkwolf", "SRU_Blue", "SRU_Red", "SRU_RiftHerald"
        };

        /// <summary>
        ///     Supported Jungle Mobs.
        /// </summary>
        public static string[] TTJungleMob = { "TT_NWraith", "TT_NWolf", "TT_NGolem", "TT_Spiderboss" };

        /// <summary>
        ///     Returns Supported Jungle Mobs.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> BigJungleMobs
        {
            get
            {
                var names = new string[] { };
                if (Game.MapId == GameMapId.SummonersRift)
                    names = SRJunglemobs;
                if (Game.MapId == GameMapId.TwistedTreeline)
                    names = TTJungleMob;
                if (Game.MapId == GameMapId.CrystalScar)
                    names = ACSJunglemobs;

                return EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => names.Any(n => n.Equals(m.BaseSkinName)));
            }
        }

        /// <summary>
        ///     Returns Lane Minions In Spell Range.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Enemies(this Spell.SpellBase spell)
        {
            return EntityManager.Enemies.Where(m => m.IsKillable() && m.IsInRange(spell.RangeCheckSource ?? Player.Instance.ServerPosition, spell.Range));
        }

        /// <summary>
        ///     Returns Lane Minions In Spell Range.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> LaneMinions(this Spell.SpellBase spell)
        {
            return EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsInRange(spell.RangeCheckSource ?? Player.Instance.ServerPosition, spell.Range));
        }

        /// <summary>
        ///     Returns Lane Minions In Spell Range.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleMinions(this Spell.SpellBase spell)
        {
            return EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsKillable() && m.IsInRange(spell.RangeCheckSource ?? Player.Instance.ServerPosition, spell.Range));
        }
        /// <summary>
        ///     Returns Lane Minions In Spell Range.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> BigJungleMinions(this Spell.SpellBase spell)
        {
            return BigJungleMobs.Where(m => m.IsKillable() && m.IsInRange(spell.RangeCheckSource ?? Player.Instance.ServerPosition, spell.Range));
        }

        /// <summary>
        ///     Returns true if target Is CC'D.
        /// </summary>
        public static bool IsCC(this Obj_AI_Base target)
        {
            return (!target.CanMove && !target.IsMe) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Fear)
                   || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt)
                   || target.HasBuffOfType(BuffType.Sleep);
        }

        /// <summary>
        ///     Returns true if target Is CC'D.
        /// </summary>
        public static bool IsCC(this AIHeroClient target)
        {
            return ((!target.CanMove || target.IsRecalling()) && !target.IsMe) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Fear)
                   || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt)
                   || target.HasBuffOfType(BuffType.Sleep);
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target, float range)
        {
            return target != null && !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie
                   && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead
                   && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range + target.BoundingRadius / 2f);
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target)
        {
            return target != null && !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie
                   && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead
                   && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target (AIHeroClient).
        /// </summary>
        public static bool IsKillable(this AIHeroClient target)
        {
            return target != null && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie
                   && !target.HasUndyingBuff(true) && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead
                   && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target (AIHeroClient).
        /// </summary>
        public static bool IsKillable(this AIHeroClient target, float range)
        {
            return target != null && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.IsZombie
                   && !target.HasUndyingBuff(true) && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead
                   && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range + target.BoundingRadius / 2f);
        }

        /// <summary>
        ///     Casts spell with selected hitchance.
        /// </summary>
        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, HitChance hitChance)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChance >= hitChance || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        ///     Casts spell with selected hitchance.
        /// </summary>
        public static void Cast(this Spell.Skillshot spell, AIHeroClient target, HitChance hitChance)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChance >= hitChance || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        ///     Casts spell with selected hitchancepercent.
        /// </summary>
        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, float hitchancepercent)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range) && !target.WillDie(spell))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChancePercent >= hitchancepercent || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        ///     Casts spell with selected hitchancepercent.
        /// </summary>
        public static void Cast(this Spell.Skillshot spell, AIHeroClient target, float hitchancepercent)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range) && !target.WillDie(spell))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChancePercent >= hitchancepercent || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static void DrawLine(Vector3 from, Vector3 to, int width, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(from);
            var wts2 = Drawing.WorldToScreen(to);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], width, color);
        }

        /// <summary>
        ///     Attemtps To Cast the spell AoE.
        /// </summary>
        public static bool CastAOE(this Spell.Skillshot spell, IEnumerable<Obj_AI_Base> targetEnumerable, int hitcount, float CustomRange = -1)
        {
            var range = CustomRange.Equals(-1) ? spell.Range : CustomRange;
            var targets = targetEnumerable as Obj_AI_Base[] ?? targetEnumerable.ToArray();
            var predtype = Prediction.Position.PredictionData.PredictionType.Circular;
            if (spell.Type.Equals(SkillShotType.Circular))
            {
                if (Prediction.Manager.PredictionSelected.Equals("ICPrediction"))
                {
                    foreach (var enemy in targets.Where(e => e.IsKillable(range)))
                    {
                        var pred = spell.GetPrediction(enemy);
                        var circle = new Geometry.Polygon.Circle(pred.CastPosition, spell.Width);
                        foreach (var point in circle.Points)
                        {
                            circle = new Geometry.Polygon.Circle(point, spell.Width);
                            foreach (var p in circle.Points.OrderBy(a => a.Distance(pred.CastPosition)))
                            {
                                if (targets.Count(t => spell.GetPrediction(t).CastPosition.IsInRange(p, spell.Width) && t.IsKillable()) >= hitcount)
                                {
                                    Player.CastSpell(spell.Slot, p.To3D());
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var predi = Prediction.Position.GetPredictionAoe(
                        targets,
                        new Prediction.Position.PredictionData(
                            predtype,
                            (int)range,
                            spell.Width,
                            spell.ConeAngleDegrees,
                            spell.CastDelay,
                            spell.Speed,
                            spell.AllowedCollisionCount,
                            spell.SourcePosition));

                    foreach (var pre in predi)
                    {
                        if (pre.CollisionObjects.Count(e => e.IsKillable(spell.Range)) >= hitcount && pre.HitChance >= HitChance.Low)
                        {
                            spell.Cast(pre.CastPosition);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static float Mana(this Spell.SpellBase spell)
        {
            if (!spell.IsLearned)
                return 0;
            return spell.ManaCost;
        }

        /// <summary>
        ///     Returns a recreated name of the target.
        /// </summary>
        public static string Name(this Obj_AI_Base target)
        {
            if (ObjectManager.Get<Obj_AI_Base>().Count(o => o.BaseSkinName.Equals(target.BaseSkinName)) > 1)
            {
                return target.BaseSkinName + "(" + target.Name + ")";
            }
            return target.BaseSkinName;
        }

        /// <summary>
        ///     Creates a checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(this Menu m, string id, string name, bool defaultvalue = true)
        {
            return m.Add(id, new CheckBox(name, defaultvalue));
        }

        /// <summary>
        ///     Creates a checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(this Menu m, SpellSlot slot, string name, bool defaultvalue = true)
        {
            return m.Add(slot.ToString(), new CheckBox(name, defaultvalue));
        }

        /// <summary>
        ///     Creates a slider.
        /// </summary>
        public static Slider CreateSlider(this Menu m, string id, string name, int defaultvalue = 0, int MinValue = 0, int MaxValue = 100)
        {
            return m.Add(id, new Slider(name, defaultvalue, MinValue, MaxValue));
        }

        /// <summary>
        ///     Creates a KeyBind.
        /// </summary>
        public static KeyBind CreateKeyBind(this Menu m, string id, string name, bool defaultvalue, KeyBind.BindTypes BindType, uint key1 = 27U, uint key2 = 27U)
        {
            return m.Add(id, new KeyBind(name, defaultvalue, BindType, key1, key2));
        }

        /// <summary>
        ///     Returns KeyBind Value.
        /// </summary>
        public static bool KeyBindValue(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Returns ComboBox Value.
        /// </summary>
        public static int ComboBoxValue(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        /// <summary>
        ///     Returns CheckBox Value.
        /// </summary>
        public static bool CheckBoxValue(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        ///     Returns CheckBox Value.
        /// </summary>
        public static bool CheckBoxValue(this Menu m, SpellSlot slot)
        {
            return m[slot.ToString()].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        ///     Returns Slider Value.
        /// </summary>
        public static int SliderValue(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        /// <summary>
        ///     Returns true if the value is >= the slider.
        /// </summary>
        public static bool CompareSlider(this Menu m, string id, float value)
        {
            return value >= m[id].Cast<Slider>().CurrentValue;
        }

        /// <summary>
        ///     Returns true if the target will die before the spell finish him.
        /// </summary>
        public static bool WillDie(this Obj_AI_Base target, Spell.SpellBase spell)
        {
            return spell.GetHealthPrediction(target) <= 0;
        }

        /// <summary>
        ///     Returns true if the spell will kill the target.
        /// </summary>
        public static bool WillKill(this Spell.SpellBase spell, Obj_AI_Base target, float MultiplyDmgBy = 1, float ExtraDamage = 0, DamageType ExtraDamageType = DamageType.True)
        {
            return Player.Instance.GetSpellDamage(target, spell.Slot) * MultiplyDmgBy + Player.Instance.CalculateDamageOnUnit(target, ExtraDamageType, ExtraDamage) >= spell.GetHealthPrediction(target) && !target.WillDie(spell);
        }

        public static bool CanKill(this AIHeroClient hero, AIHeroClient target)
        {
            return hero.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth() && target.IsKillable(hero.GetAutoAttackRange(target));
        }

        public static IEnumerable<AIHeroClient> GetKillStealTargets(this Spell.SpellBase spell)
        {
            return EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).Where(o => spell.WillKill(o) && o.IsKillable());
        }

        public static AIHeroClient GetKillStealTarget(this Spell.SpellBase spell)
        {
            return spell.GetKillStealTargets().FirstOrDefault(o => o.IsKillable(spell.Range));
        }

        public static float AlliesAADamage(this Obj_AI_Base target)
        {
            return EntityManager.Allies.Where(a => a.IsValidTarget() && a.IsInRange(target, a.GetAutoAttackRange(target))).Sum(d => d.GetAutoAttackDamage(target, true));
        }

        /// <summary>
        ///     Returns true if the target is big minion (Siege / Super Minion).
        /// </summary>
        public static bool IsBigMinion(this Obj_AI_Base target)
        {
            return target.BaseSkinName.ToLower().Contains("siege") || target.BaseSkinName.ToLower().Contains("super");
        }

        /// <summary>
        ///     Returns true if the target is big minion (Siege / Super Minion).
        /// </summary>
        public static Vector3 PrediectPosition(this Obj_AI_Base target, int Time = 250)
        {
            if (Time == 250)
                Time += Game.Ping;
            return Prediction.Position.PredictUnitPosition(target, Time).To3D();
        }

        public static float PredictHealth(this Obj_AI_Base target, int Time = 250)
        {
            if (Time == 250)
                Time += Game.Ping;
            return Prediction.Health.GetPrediction(target, Time);
        }

        public static bool WillDie(this Obj_AI_Base target, int Time = 250)
        {
            return target.PredictHealth() <= 0;
        }
    }
}
