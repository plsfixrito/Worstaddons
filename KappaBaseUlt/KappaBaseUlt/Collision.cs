using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace KappaBaseUlt
{
    public static class Collision
    {
        public static bool Check(AIHeroClient start, Vector3 end, Baseult spell)
        {
            if (spell.AllowedCollisionCount >= int.MaxValue)
            {
                return true;
            }
            
            if (spell.AllowedCollisionCount == -1 || spell.AllowedCollisionCount == 1)
            {
                if (start.GetYasuoWallCollision(end, start.IsAlly) != Vector3.Zero)
                {
                    return false;
                }
            }

            var polygon = new Geometry.Polygon.Rectangle(start.ServerPosition, end, spell.Width);
            return !EntityManager.Heroes.AllHeroes.Any(h => h.IsValid && !h.IsDead && h.Team != start.Team && polygon.IsInside(h));
        }
    }
}