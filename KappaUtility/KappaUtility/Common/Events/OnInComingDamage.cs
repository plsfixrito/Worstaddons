using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using KappaUtility.Common.KappaEvade;
using KappaUtility.Common.Misc;

namespace KappaUtility.Common.Events
{
    internal class OnInComingDamage
    {
        /// <summary>
        /// Fires when There is In Coming Damage to an ally
        /// </summary>
        public static event OninComingDamage OnIncomingDamage;

        /// <summary>
        ///     A handler for the InComingDamage event
        /// </summary>
        /// <param name="args">The arguments the event provides</param>
        public delegate void OninComingDamage(InComingDamageEventArgs args);

        public class InComingDamageEventArgs
        {
            public Obj_AI_Base Sender;
            public AIHeroClient Target;
            public float InComingDamage;
            public Type DamageType;
            public bool CanWindWall;

            public enum Type
            {
                TurretAttack,
                HeroAttack,
                MinionAttack,
                SkillShot,
                TargetedSpell
            }

            public InComingDamageEventArgs(Obj_AI_Base sender, AIHeroClient target, float Damage, Type type, bool windwall = false)
            {
                this.Sender = sender;
                this.Target = target;
                this.InComingDamage = Damage;
                this.DamageType = type;
                this.CanWindWall = windwall;
            }
        }

        public static void Init()
        {
            try
            {
                Game.OnUpdate += delegate
                    {
                        // Used to Invoke the Incoming Damage Event When there is SkillShot Incoming
                        foreach (var ally in EntityManager.Heroes.Allies.Where(a => a.IsValidTarget()))
                        {
                            foreach (var spell in Collision.NewSpells)
                            {
                                if (ally.IsInDanger(spell))
                                    InvokeOnIncomingDamage(new InComingDamageEventArgs(spell.Caster, ally, spell.Caster.GetSpellDamage(ally, spell.spell.slot), InComingDamageEventArgs.Type.SkillShot, true));
                            }
                            foreach (var b in DamageBuffs)
                            {
                                var dmgbuff = ally.Buffs.FirstOrDefault(buff => buff.SourceName.Equals(b.Champion) && buff.Name.Equals(b.BuffName) && buff.IsActive && buff.EndTime - Game.Time < 0.25f);
                                var caster = dmgbuff?.Caster as AIHeroClient;
                                if (caster != null)
                                    InvokeOnIncomingDamage(new InComingDamageEventArgs(caster, ally, caster.GetSpellDamage(ally, b.Slot), InComingDamageEventArgs.Type.TargetedSpell));
                            }
                        }
                    };

                SpellsDetector.OnTargetedSpellDetected += delegate(AIHeroClient sender, AIHeroClient target, GameObjectProcessSpellCastEventArgs args, Database.TargetedSpells.TSpell spell)
                    {
                        if (!Brain.Utility.Load.menu.SubMenus.FirstOrDefault(m => m.DisplayName.Equals("DamageHandler")).CheckBoxValue(args.Slot + sender.ChampionName))
                            return;

                        // Used to Invoke the Incoming Damage Event When there is a TargetedSpell Incoming
                        if (target.IsAlly)
                            InvokeOnIncomingDamage(new InComingDamageEventArgs(sender, target, sender.GetSpellDamage(target, spell.slot), InComingDamageEventArgs.Type.TargetedSpell, spell.CanWindWall));
                    };

                Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                    {
                        // Used to Invoke the Incoming Damage Event When there is an AutoAttack Incoming
                        if(args.Target == null || sender == null)
                            return;

                        var target = args.Target as AIHeroClient;
                        var hero = sender as AIHeroClient;
                        var turret = sender as Obj_AI_Turret;
                        var minion = sender as Obj_AI_Minion;

                        if (target == null || !target.IsAlly || !sender.IsEnemy)
                            return;

                        if (hero != null)
                            InvokeOnIncomingDamage(new InComingDamageEventArgs(hero, target, hero.GetAutoAttackDamage(target, true), InComingDamageEventArgs.Type.HeroAttack, true));
                        if (turret != null)
                            InvokeOnIncomingDamage(new InComingDamageEventArgs(turret, target, turret.GetAutoAttackDamage(target, true), InComingDamageEventArgs.Type.TurretAttack));
                        if (minion != null)
                            InvokeOnIncomingDamage(new InComingDamageEventArgs(minion, target, minion.GetAutoAttackDamage(target, true), InComingDamageEventArgs.Type.MinionAttack, true));
                    };
                Obj_AI_Base.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                    {
                        if (!Brain.Utility.Load.menu.SubMenus.FirstOrDefault(m => m.DisplayName.Equals("DamageHandler")).CheckBoxValue("unlisted"))
                            return;

                        var caster = sender as AIHeroClient;
                        var target = args.Target as AIHeroClient;
                        if (caster == null || target == null || !caster.IsEnemy || !target.IsAlly || args.IsAutoAttack())
                            return;
                        if (!Database.TargetedSpells.TargetedSpellsList.Any(s => s.hero == caster.Hero && s.slot == args.Slot))
                        {
                            InvokeOnIncomingDamage(new InComingDamageEventArgs(caster, target, caster.GetSpellDamage(target, args.Slot), InComingDamageEventArgs.Type.TargetedSpell));
                        }
                    };
            }
            catch (Exception ex)
            {
                Logger.Send("ERROR AT KappaUtility.Common.Events.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void InvokeOnIncomingDamage(InComingDamageEventArgs args)
        {
            if (args?.InComingDamage < 1 || args == null)
                return;
            if (args.DamageType == InComingDamageEventArgs.Type.HeroAttack && !Brain.Utility.Load.menu.SubMenus.FirstOrDefault(m => m.DisplayName.Equals("DamageHandler")).CheckBoxValue("Heros"))
                return;
            if (args.DamageType == InComingDamageEventArgs.Type.MinionAttack && !Brain.Utility.Load.menu.SubMenus.FirstOrDefault(m => m.DisplayName.Equals("DamageHandler")).CheckBoxValue("Minions"))
                return;
            if (args.DamageType == InComingDamageEventArgs.Type.TurretAttack && !Brain.Utility.Load.menu.SubMenus.FirstOrDefault(m => m.DisplayName.Equals("DamageHandler")).CheckBoxValue("Turrets"))
                return;
            
            OnIncomingDamage?.Invoke(
                new InComingDamageEventArgs(
                    args.Sender,
                    args.Target,
                    args.InComingDamage * (Brain.Utility.Load.menu.SubMenus.FirstOrDefault(m => m.DisplayName.Equals("DamageHandler")).SliderValue("Mod") * 0.01f),
                    args.DamageType,
                    args.CanWindWall));
        }

        private static List<DamageBuff> DamageBuffs = new List<DamageBuff>
            {
                new DamageBuff("Karthus", "karthusfallenonetarget", SpellSlot.R),
                new DamageBuff("Tristana", "tristanaechargesound", SpellSlot.E),
                new DamageBuff("Zilean", "ZileanQEnemyBomb", SpellSlot.Q),
            };

        internal class DamageBuff
        {
            public string Champion;
            public string BuffName;
            public SpellSlot Slot;
            public DamageBuff(string Caster, string buffname, SpellSlot slot)
            {
                this.Champion = Caster;
                this.BuffName = buffname;
                this.Slot = slot;
            }
        }
    }
}
