using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaUtility.Common.KappaEvade
{
    internal static class KappaEvade
    {
        public static void Init()
        {
            try
            {
                Logger.Send("KappaEvade Loaded !");
                SpellsDetector.Init();
                Collision.Init();
                SpellsDetector.OnSkillShotDetected += SpellsDetector_OnSkillShotDetected;
                //Drawing.OnDraw += Drawing_OnDraw;
                GameObject.OnDelete += GameObject_OnDelete;
                Game.OnTick += Game_OnTick;
                //GameObject.OnCreate += GameObject_OnCreate;
                //debug.Init();
            }
            catch (Exception)
            {
                // nothing
            }
        }

        public static bool IsInDanger(this Obj_AI_Base target, ActiveSpells spell)
        {
            var traveltime = target.Distance(spell.Start) / spell.spell.Speed * 1000 + spell.spell.CastDelay;
            return spell.ToPolygon().IsInside(target.PrediectPosition((int)traveltime));
        }

        public static Geometry.Polygon.Sector CreateCone(Vector3 Center, Vector3 Direction, float Angle, float Range)
        {
            return new Geometry.Polygon.Sector(Center, Direction, (float)(Angle * Math.PI / 180), Range);
        }

        public static Geometry.Polygon ToPolygon(this ActiveSpells spell)
        {
            if (spell.spell.type == Database.SkillShotSpells.Type.LineMissile)
            {
                return new Geometry.Polygon.Rectangle(spell.StartPosition, spell.EndPosition, spell.Width);
            }
            if (spell.spell.type == Database.SkillShotSpells.Type.CircleMissile)
            {
                return new Geometry.Polygon.Circle(spell.EndPosition, spell.Width);
            }
            if (spell.spell.type == Database.SkillShotSpells.Type.Cone)
            {
                return CreateCone(spell.StartPosition, spell.End, spell.spell.Angle, spell.Range);
            }
            return null;
        }

        public struct ActiveSpells
        {
            public AIHeroClient Caster;
            public Vector3 Start;
            public Vector3 End;
            public float Range;
            public float Width;
            public Database.SkillShotSpells.SSpell spell;
            public float EndTime;
            public MissileClient Missile;
            public float StartTime;
            public float ArriveTime;

            public Vector3 StartPosition
            {
                get
                {
                    if (this.spell.SticksToCaster && this.Caster != null)
                    {
                        return this.Caster.ServerPosition;
                    }
                    return Missile?.Position ?? Start;
                }
            }

            public Vector3 EndPosition
            {
                get
                {
                    if (spell.SpellName.Equals("SionR"))
                    {
                        var direction = this.Caster.Direction() != Vector2.Zero ? this.Caster.Direction().To3D() : StartPosition - (this.Start - this.End);
                        return StartPosition.Extend(direction, this.Range).To3D();
                    }
                    if (this.spell.SticksToMissile || this.spell.SpellName.Equals("SionR"))
                    {
                        return StartPosition.Extend(StartPosition - (this.Start - this.End), this.Range).To3D();
                    }
                    if (this.spell.EndSticksToCaster && this.Caster != null)
                    {
                        return this.Caster.ServerPosition;
                    }
                    return Missile?.StartPosition.Extend(End, Range).To3D() ?? End;
                }
            }
        }

        public static List<ActiveSpells> DetectedSpells = new List<ActiveSpells>();

        public static List<ActiveSpells> DetectedMissiles = new List<ActiveSpells>();
        /*
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null || missile == null || !caster.IsMe || !missile.IsValid)
            {
                return;
            }

            if (Database.SkillShotSpells.SkillShotsSpellsList.Any(s => caster != null && (s.hero == caster.Hero && s.MissileName.ToLower() == missile.SData.Name.ToLower())))
            {
                var spell = Database.SkillShotSpells.SkillShotsSpellsList.FirstOrDefault(s => caster != null && (s.hero == caster.Hero && s.MissileName.ToLower() == missile.SData.Name.ToLower()));

                if (DetectedSpells.All(s => !spell.MissileName.Equals(s.spell.MissileName, StringComparison.CurrentCultureIgnoreCase) && caster.NetworkId != s.Caster.NetworkId))
                {
                    var endpos = missile.StartPosition.Extend(missile.EndPosition, spell.Range).To3D();

                    if (spell.type == Database.SkillShotSpells.Type.LineMissile || spell.type == Database.SkillShotSpells.Type.CircleMissile)
                    {
                        var rect = new Geometry.Polygon.Rectangle(missile.StartPosition, endpos, spell.Width);
                        var collide =
                            ObjectManager.Get<Obj_AI_Base>()
                                .OrderBy(m => m.Distance(sender))
                                .FirstOrDefault(
                                    s =>
                                    s.Team != sender.Team
                                    && (((s.IsMinion || s.IsMonster || s is Obj_AI_Minion) && !s.IsWard() && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Minions))
                                        || s is AIHeroClient && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Heros)) && rect.IsInside(s) && s.IsValidTarget());

                        if (collide != null)
                        {
                            endpos = collide.ServerPosition;
                        }
                    }

                    if (spell.type == Database.SkillShotSpells.Type.Cone)
                    {
                        var sector = new Geometry.Polygon.Sector(missile.StartPosition, endpos, spell.Angle, spell.Range);
                        var collide =
                            ObjectManager.Get<Obj_AI_Base>()
                                .OrderBy(m => m.Distance(sender))
                                .FirstOrDefault(
                                    s =>
                                    s.Team != sender.Team
                                    && (((s.IsMinion || s.IsMonster || s is Obj_AI_Minion) && !s.IsWard() && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Minions))
                                        || s is AIHeroClient && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Heros)) && sector.IsInside(s) && s.IsValidTarget());

                        if (collide != null)
                        {
                            endpos = collide.ServerPosition;
                        }
                    }
                    //Chat.Print("OnCreate " + missile.SData.Name);
                    DetectedSpells.Add(
                        new ActiveSpells
                        {
                            spell = spell,
                            Start = missile.StartPosition,
                            End = missile.EndPosition,
                            Width = spell.Width,
                            EndTime = (endpos.Distance(missile.StartPosition) / spell.Speed) + (spell.CastDelay / 1000) + Game.Time,
                            Missile = missile,
                            Caster = caster,
                            ArriveTime = (missile.StartPosition.Distance(Player.Instance) / spell.Speed) - (spell.CastDelay / 1000)
                        });
                }
            }
        }
        */

        private static void Game_OnTick(EventArgs args)
        {
            DetectedSpells.RemoveAll(s => Game.Time - s.EndTime >= 0);
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (missile == null || caster == null || missile.IsAutoAttack() || !missile.IsValid)
                return;
            //Chat.Print("OnDelete Detected " + missile.SData.Name);
            if (DetectedSpells.Any(s => s.Caster != null && s.spell.MissileName.Equals(missile.SData.Name, StringComparison.CurrentCultureIgnoreCase) && caster.NetworkId == s.Caster.NetworkId))
            {
                //Chat.Print("OnDelete Remove " + missile.SData.Name);
                DetectedSpells.RemoveAll(
                    s => s.Caster != null && s.spell.MissileName.Equals(missile.SData.Name, StringComparison.CurrentCultureIgnoreCase) && caster.NetworkId.Equals(s.Caster.NetworkId));
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in Collision.NewSpells /*.Where(s => debug.SkillShots.checkbox(s.Caster.ID() + s.spell.slot + "draw"))*/)
            {
                if (spell.Missile != null)
                {
                    Circle.Draw(SharpDX.Color.GreenYellow, spell.Width, spell.Missile);
                }

                var c = Player.Instance.IsInDanger(spell) ? Color.Red : Color.AliceBlue;
                spell.ToPolygon().Draw(c, 2);
            }
        }

        private static void SpellsDetector_OnSkillShotDetected(
            AIHeroClient sender,
            GameObjectProcessSpellCastEventArgs args,
            Database.SkillShotSpells.SSpell spell,
            Vector3 Start,
            Vector3 End,
            float Range,
            float Width,
            MissileClient missile)
        {
            var spellrange = Range;
            var endpos = spell.IsFixedRange ? Start.Extend(End, spellrange).To3D() : End;

            if (spell.type == Database.SkillShotSpells.Type.CircleMissile && End.Distance(Start) < Range)
            {
                endpos = End;
            }

            var newactive = new ActiveSpells
                {
                    Start = Start, End = endpos, Range = spellrange, Width = Width, spell = spell, EndTime = (endpos.Distance(Start) / spell.Speed) + (spell.CastDelay / 1000) + Game.Time,
                    Caster = sender, Missile = missile, ArriveTime = (Start.Distance(Player.Instance) / spell.Speed) - (spell.CastDelay / 1000)
                };

            if (!DetectedSpells.Contains(newactive))
            {
                DetectedSpells.Add(newactive);
            }
        }
    }
}
