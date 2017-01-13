namespace KappaXerath
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Notifications;

    using SharpDX;

    internal abstract class Common
    {
        public static DangerLevel danger()
        {
            switch (Program.MiscMenu["danger"].Cast<ComboBox>().CurrentValue)
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
            return DangerLevel.Low;
        }

        public static HitChance hitchance(Spell.SpellBase spell, Menu m)
        {
            switch (m[spell.Slot + "hit"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    {
                        return HitChance.High;
                    }
                case 1:
                    {
                        return HitChance.Medium;
                    }
                case 2:
                    {
                        return HitChance.Low;
                    }
            }
            return HitChance.Unknown;
        }

        public static void ShowNotification(string message, int duration = -1)
        {
            Notifications.Show(new SimpleNotification(message, message), duration);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        public static void DrawCricleMinimap(System.Drawing.Color color, float radius, Vector3 center, int thickness = 5, int quality = 30)
        {
            var pointList = new List<Vector3>();
            for (var i = 0; i < quality; i++)
            {
                var angle = i * Math.PI * 2 / quality;
                pointList.Add(new Vector3(center.X + radius * (float)Math.Cos(angle), center.Y + radius * (float)Math.Sin(angle), center.Z));
            }

            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);

                Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, thickness, color);
            }
        }

        public static bool IsCC(Obj_AI_Base target)
        {
            return target.IsStunned || target.IsRooted || target.IsTaunted || target.IsCharmed || target.Spellbook.IsChanneling
                   || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup)
                   || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression)
                   || target.HasBuffOfType(BuffType.Taunt);
        }

        public static bool ValidUlt(AIHeroClient target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                   && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }
    }
}