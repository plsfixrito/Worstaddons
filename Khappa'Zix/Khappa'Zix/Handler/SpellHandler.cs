using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Rendering;
using Khappa_Zix.Common;
using SharpDX;
using static Khappa_Zix.Settings.Config.Drawing;

namespace Khappa_Zix.Handler
{
    internal class SpellHandler
    {
        private static AIHeroClient user { get { return Player.Instance; } }

        public static Spell.Targeted Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Active R;

        public static bool IsQEvolved {get { return user.HasBuff("KhazixQEvo"); } }
        public static bool IsWEvolved { get { return user.HasBuff("KhazixWEvo"); } }
        public static bool IsEEvolved { get { return user.HasBuff("KhazixEEvo"); } }
        public static bool IsREvolved { get { return user.HasBuff("KhazixREvo"); } }
        public static bool IsRStealthed { get { return user.HasBuff("khazixrstealth"); } }

        public SpellHandler()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 325, DamageType.Physical);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 80, DamageType.Physical) { AllowedCollisionCount = 0 };
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Circular, 25, 1000, 100, DamageType.Physical) { AllowedCollisionCount = int.MaxValue };
            R = new Spell.Active(SpellSlot.R, int.MaxValue, DamageType.Physical);

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(UseQ)
                Q.DrawRange(Q.IsReady() ? Color.Chartreuse : Color.OrangeRed);
            if (UseW)
                W.DrawRange(W.IsReady() ? Color.Chartreuse : Color.OrangeRed);
            if (UseE)
                E.DrawRange(E.IsReady() ? Color.Chartreuse : Color.OrangeRed);
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (IsQEvolved && Q.Range != 375)
            {
                Q.Range = 375;
            }
            if (IsEEvolved && E.Range != 1000)
            {
                E.Range = 1000;
            }
        }

        public static float ComboDamage(Obj_AI_Base target)
        {
            if (!target.IsKillable())
                return 0;

            var dmg = 0f;
            if (Q.IsReady())
                dmg += QDamage(target);
            if (W.IsReady())
                dmg += WDamage(target);
            if (E.IsReady())
                dmg += EDamage(target);

            return dmg;
        }

        public static float QDamage(Obj_AI_Base target)
        {
            if (!Q.IsLearned)
                return 0;

            var index = Q.Level - 1;
            var qdmg = new float[] { 70f, 95f, 120f, 145f, 170f };
            var qmod = 1.4f;
            if (target.IsIsolated())
            {
                qdmg = new[] { 105f, 142.5f, 180, 217.5f, 255f };
                qmod = 2.1f;
            }

            return user.CalculateDamageOnUnit(target, DamageType.Physical, qdmg[index] + user.FlatPhysicalDamageMod * qmod);
        }

        public static float WDamage(Obj_AI_Base target)
        {
            if (!W.IsLearned)
                return 0;

            var index = W.Level - 1;
            var Wdmg = new [] { 80f, 110f, 140f, 170f, 200f };
            var Wmod = 0.85f;
            if (IsWEvolved)
            {
                Wdmg = new [] { 96f, 132f, 168f, 204f, 240f };
                Wmod = 1.02f;
            }

            return user.CalculateDamageOnUnit(target, DamageType.Physical, Wdmg[index] + user.FlatPhysicalDamageMod * Wmod);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            if (!E.IsLearned)
                return 0;

            var index = E.Level - 1;
            var Edmg = new[] { 65f, 100f, 135f, 170f, 205f };
            var Emod = 0.2f;

            return user.CalculateDamageOnUnit(target, DamageType.Physical, Edmg[index] + user.FlatPhysicalDamageMod * Emod);
        }
    }
}
