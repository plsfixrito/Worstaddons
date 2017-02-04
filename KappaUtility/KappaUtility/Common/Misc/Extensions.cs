using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Texture;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaUtility.Common.Misc
{
    internal static class Extensions
    {
        /// <summary>
        ///     Returns true if target Is CC'D.
        /// </summary>
        public static bool IsCC(this Obj_AI_Base target)
        {
            return (!target.CanMove && !target.IsMe) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup)
                   || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression)
                   || target.HasBuffOfType(BuffType.Taunt);
        }

        /// <summary>
        ///     Returns true if target Is CC'D.
        /// </summary>
        public static bool IsCC(this AIHeroClient target)
        {
            return ((!target.CanMove || target.IsRecalling()) && !target.IsMe) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback)
                   || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun)
                   || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt);
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target, float range)
        {
            return target != null && !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention")
                   && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity)
                   && target.IsValidTarget(range);
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target)
        {
            return target != null && !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention")
                   && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity)
                   && target.IsValidTarget();
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target (AIHeroClient).
        /// </summary>
        public static bool IsKillable(this AIHeroClient target)
        {
            return target != null && !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention")
                   && !target.IsZombie && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.HasUndyingBuff() && !target.IsInvulnerable && !target.IsZombie
                   && !target.HasBuff("bansheesveil") && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        /// <summary>
        ///     Returns CCBuffs list.
        /// </summary>
        public static List<BuffType> CCbuffs = new List<BuffType>
            {
                BuffType.Blind, BuffType.Charm, BuffType.Fear, BuffType.Knockback, BuffType.Knockup, BuffType.NearSight, BuffType.Poison, BuffType.Polymorph, BuffType.Shred, BuffType.Silence,
                BuffType.Slow, BuffType.Snare, BuffType.Stun, BuffType.Suppression, BuffType.Taunt
            };

        /// <summary>
        ///     Casts spell with selected hitchancepercent.
        /// </summary>
        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, float hitchancepercent)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChancePercent >= hitchancepercent || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static List<string> NoManaHeros = new List<string>
        {
            "Akali", "DrMundo", "Garen", "Gnar", "Katarina", "Kennen", "Kled", "LeeSin", "Mordekaiser",
            "RekSai", "Renekton", "Rengar", "Riven", "Rumble", "Shen", "Shyvana", "Tryndamere", "Vladimir", "Yasuo"
        };

        public static bool IsNoManaHero(this AIHeroClient target)
        {
            return NoManaHeros.Contains(target.ChampionName.Trim());
        }

        /// <summary>
        ///     Returns true The spell slot isn't Unknown
        /// </summary>
        public static bool IsVaild(this Spell.SpellBase spell)
        {
            return spell.Slot != SpellSlot.Unknown;
        }
        
        public static bool IsVisionWard(this Obj_AI_Base target)
        {
            return target.BaseSkinName.ToLower().Contains("vision");
        }

        /// <summary>
        ///     Returns a recreated name of the target.
        /// </summary>
        public static string Name(this Obj_AI_Base target)
        {
            if (target.IsMe)
            {
                return "MyHero";
            }

            var hero = target as AIHeroClient;
            if (hero != null)
            {
                return hero.Name();
            }

            if (ObjectManager.Get<Obj_AI_Base>().Count(o => o.BaseSkinName.Equals(target.BaseSkinName)) > 1)
            {
                return target.BaseSkinName + "(" + target.Name + ")";
            }
            return target.BaseSkinName;
        }

        /// <summary>
        ///     Returns a recreated name of the target.
        /// </summary>
        public static string Name(this AIHeroClient target)
        {
            if (target.IsMe)
            {
                return "MyHero";
            }

            if (EntityManager.Heroes.AllHeroes.Count(h => h.BaseSkinName.Equals(target.BaseSkinName)) > 1)
            {
                return target.BaseSkinName + "(" + target.Name + ")";
            }
            return target.BaseSkinName;
        }

        /// <summary>
        ///     Returns true if the Item IsReady.
        /// </summary>
        public static bool ItemReady(this Item item, Menu menu)
        {
            return item != null && item.IsOwned(Player.Instance) && item.IsReady() && menu.CheckBoxValue(item.ItemInfo.Name);
        }

        /// <summary>
        ///     Returns Predicted Position.
        /// </summary>
        public static Vector3 PrediectPosition(this Obj_AI_Base target, int Time = 250)
        {
            return Prediction.Position.PredictUnitPosition(target, Time).To3D();
        }

        /// <summary>
        ///     Creates a checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(this Menu m, string id, string name, bool defaultvalue = true)
        {
            return m.Add(id, new CheckBox(name, defaultvalue));
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
        ///     Returns ComboBox Value.
        /// </summary>
        public static int ComboBoxValue(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        /// <summary>
        ///     Returns KeyBind Value.
        /// </summary>
        public static bool KeyBindValue(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Returns CheckBox Value.
        /// </summary>
        public static bool CheckBoxValue(this Menu m, string id)
        {
            try
            {
                return m[id].Cast<CheckBox>().CurrentValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine("wrong id: " + id + " Menu: " + m.DisplayName);
                return false;
            }
        }

        /// <summary>
        ///     Returns Slider Value.
        /// </summary>
        public static int SliderValue(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        /// <summary>
        ///     List of the Game Ping Types.
        /// </summary>
        public static string[] Pingtypes = { "Danger", "Fallback", "OnMyWay", "AssistMe", "EnemyMissing", "Normal" };

        public static void DrawLine(Vector3 from, Vector3 to, int width, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(from);
            var wts2 = Drawing.WorldToScreen(to);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], width, color);
        }
        
        public static float DeathTimer(this AIHeroClient hero)
        {
            var spawntime = 0f;

            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                return Math.Max(10, hero.Level * 2 + 4);
            }

            var currentminutes = Game.Time / 60;

            float BRW = hero.Level * 2.5f + 7.5f;

            if (currentminutes > 15 && currentminutes < 30)
            {
                spawntime = BRW + ((BRW / 100) * (currentminutes - 15) * 2 * 0.425f);
            }

            if (currentminutes > 30 && currentminutes < 45)
            {
                spawntime = BRW + ((BRW / 100) * (currentminutes - 15) * 2 * 0.425f) + ((BRW / 100) * (currentminutes - 30) * 2 * 0.30f);
            }

            if (currentminutes > 45f && currentminutes < 53.5f)
            {
                spawntime = BRW + ((BRW / 100) * (currentminutes - 15) * 2 * 0.425f) + ((BRW / 100) * (currentminutes - 30) * 2 * 0.30f) * ((BRW / 100) * (currentminutes - 45) * 2 * 1.45f);
            }

            if (currentminutes > 53.5f)
            {
                spawntime = (BRW + ((BRW / 100) * (currentminutes - 15) * 2 * 0.425f) + ((BRW / 100) * (currentminutes - 30) * 2 * 0.30f) + ((BRW / 100) * (currentminutes - 45) * 2 * 1.45f)) * 1.5f;
            }

            if (spawntime.Equals(0))
            {
                spawntime = BRW;
            }

            return spawntime + 1;
        }

        public static float XPToCurrentLevel(this AIHeroClient hero)
        {
            var Totalxp = new[] { 280, 660, 1140, 1720, 2400, 3180, 4060, 5040, 6120, 7300, 8580, 9960, 11440, 13020, 14700, 16480, 18360 };
            var aram = 0;
            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                if (hero.Level <= 3)
                    return 0;

                aram = -2;
            }
            if (Game.MapId == GameMapId.CrystalScar)
            {
                var csTotalXPSum = new[] { 790, 1245, 1785, 2410, 3110, 3900, 4770, 5645, 6575, 7560, 8590, 9660, 10935, 12290, 13715 };
                return hero.Level <= 3 ? 0 : csTotalXPSum[hero.Level - 4];
            }
            return hero.Level == 1 ? 0 : Totalxp[hero.Level - 2] + aram;
        }

        public static float XPNeededToNextLevel(this AIHeroClient hero)
        {
            if (hero.Level >= 18)
            {
                return 0;
            }
            if (Game.MapId == GameMapId.CrystalScar)
            {
                var csTotalXP = new[] { 790, 455, 540, 625, 700, 790, 870, 875, 930, 985, 1030, 1070, 1275, 1355, 1425 };
                return csTotalXP[hero.Level <= 3 ? 0 : hero.Level - 3];
            }
            var aramstart = Game.MapId == GameMapId.HowlingAbyss && hero.Level <= 3;
            var baseexpneeded = 180 + hero.Level * 100;

            return aramstart ? 1138 : baseexpneeded;
        }

        public static float CurrentXP(this AIHeroClient hero)
        {
            if (hero.Level >= 18)
            {
                return 0;
            }
            var TotalXp = TotalXP(hero);
            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                if (hero.Level <= 3)
                    return TotalXp;
            }
            var currnet = TotalXp - XPToCurrentLevel(hero);
            return hero.Level == 1 ? TotalXp : currnet;
        }

        public static float CurrentXPPercent(this AIHeroClient hero)
        {
            return Math.Max(0, CurrentXP(hero) / XPNeededToNextLevel(hero) * 100);
        }

        public static float TotalXP(this AIHeroClient hero)
        {
            return hero.Experience.XP;
        }

        public static SpellDataInst SlotToSpell(this AIHeroClient hero, SpellSlot slot)
        {
            return hero.Spellbook.GetSpell(slot);
        }

        public static string CoolDown(this SpellDataInst spell)
        {
            var t = (spell.CooldownExpires - Game.Time) + 1;
            var ts = TimeSpan.FromSeconds(t);
            var s = t > 60 ? string.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds) : string.Format("{0:0}", t);
            return s;
        }

        public static float CurrentCD(this SpellDataInst spell)
        {
            var t = spell.CooldownExpires - Game.Time;
            return t;
        }

        public static float CurrentCD(this AIHeroClient hero, SpellSlot slot)
        {
            var spell = SlotToSpell(hero, slot);
            return spell.CurrentCD();
        }

        public static bool IsOnCoolDown(this AIHeroClient hero, SpellSlot slot)
        {
            return CurrentCD(hero, slot) > 0;
        }

        public static Sprite SpellSprite(this AIHeroClient hero, SpriteSlot spr)
        {
            var Notready = hero.IsOnCoolDown(spr.Slot) || !hero.Spellbook.GetSpell(spr.Slot).IsLearned;
            return Notready ? spr.NotReadySprite : spr.ReadySprite;
        }

        public static ChampionTexture HeroIcons(this AIHeroClient hero)
        {
            return LoadTexture.LoadedTexture.FirstOrDefault(t => t.Hero.IdEquals(hero));
        }

        public static Sprite HeroIcon(this AIHeroClient hero)
        {
            return (hero.IsDead || !hero.IsHPBarRendered) ? HeroIcons(hero)?.HeroIconDead : HeroIcons(hero)?.HeroIcon;
        }
    }
}
