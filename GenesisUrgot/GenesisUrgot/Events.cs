using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using GenesisUrgot.KappaEvade;

namespace GenesisUrgot
{
    class Events
    {
        /// <summary>
        ///     Fires when There is In Coming Damage to an ally
        /// </summary>
        public static event OnInComingDamage OnIncomingDamage;

        /// <summary>
        ///     A handler for the InComingDamage event
        /// </summary>
        /// <param name="args">The arguments the event provides</param>
        public delegate void OnInComingDamage(InComingDamageEventArgs args);

        public class InComingDamageEventArgs
        {
            public Obj_AI_Base Sender;
            public AIHeroClient Target;
            public float InComingDamage;
            public Type DamageType;

            public enum Type
            {
                TurretAttack,
                HeroAttack,
                MinionAttack,
                SkillShot,
                TargetedSpell
            }

            public InComingDamageEventArgs(Obj_AI_Base sender, AIHeroClient target, float Damage, Type type)
            {
                this.Sender = sender;
                this.Target = target;
                this.InComingDamage = Damage;
                this.DamageType = type;
            }
        }

        public static void Init()
        {
            Game.OnUpdate += delegate
                {
                    // Used to Invoke the Incoming Damage Event When there is SkillShot Incoming
                    foreach (var spell in Collision.NewSpells)
                    {
                        foreach (var ally in EntityManager.Heroes.Allies.Where(a => !a.IsDead && a.IsValidTarget() && a.IsInDanger(spell)))
                        {
                            OnIncomingDamage?.Invoke(new InComingDamageEventArgs(spell.Caster, ally, spell.Caster.GetSpellDamage(ally, spell.spell.slot), InComingDamageEventArgs.Type.SkillShot));
                        }
                    }
                };
            SpellsDetector.OnTargetedSpellDetected += delegate(AIHeroClient sender, AIHeroClient target, GameObjectProcessSpellCastEventArgs args, Database.TargetedSpells.TSpell spell)
                {
                    // Used to Invoke the Incoming Damage Event When there is a TargetedSpell Incoming
                    if (target.IsAlly)
                        OnIncomingDamage?.Invoke(new InComingDamageEventArgs(sender, target, sender.GetSpellDamage(target, spell.slot), InComingDamageEventArgs.Type.TargetedSpell));
                };
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    // Used to Invoke the Incoming Damage Event When there is an AutoAttack Incoming
                    var target = args.Target as AIHeroClient;
                    var hero = sender as AIHeroClient;
                    var turret = sender as Obj_AI_Turret;
                    var minion = sender as Obj_AI_Minion;

                    if (target == null || !target.IsAlly)
                        return;

                    if (hero != null)
                        OnIncomingDamage?.Invoke(new InComingDamageEventArgs(hero, target, hero.GetAutoAttackDamage(target), InComingDamageEventArgs.Type.HeroAttack));
                    if (turret != null)
                        OnIncomingDamage?.Invoke(new InComingDamageEventArgs(turret, target, turret.GetAutoAttackDamage(target), InComingDamageEventArgs.Type.TurretAttack));
                    if (minion != null)
                        OnIncomingDamage?.Invoke(new InComingDamageEventArgs(minion, target, minion.GetAutoAttackDamage(target), InComingDamageEventArgs.Type.MinionAttack));
                };
            Obj_AI_Base.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    var caster = sender as AIHeroClient;
                    var target = args.Target as AIHeroClient;
                    if (caster == null || target == null || !caster.IsEnemy || !target.IsAlly || args.IsAutoAttack()) return;
                    if (!Database.TargetedSpells.TargetedSpellsList.Any(s => s.hero == caster.Hero && s.slot == args.Slot))
                    {
                        OnIncomingDamage?.Invoke(new InComingDamageEventArgs(caster, target, caster.GetSpellDamage(target, args.Slot), InComingDamageEventArgs.Type.TargetedSpell));
                    }
                };
        }
    }
}
