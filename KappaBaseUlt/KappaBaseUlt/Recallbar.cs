using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaBaseUlt
{
    internal static class Recallbar
    {
        public static float X2;
        public static float Y2;

        public static float X = Drawing.Width * 0.425f;
        public static float Y = Drawing.Height * 0.80f;

        private static int Width = (int)(Drawing.Width - 2 * X);
        private static int Height = 6;

        private static readonly float Scale = (float)Width / 8000;
        private static Vector2 startpoint;

        public static void RecallBarDraw(this Program.EnemyInfo enemy)
        {
            Rect(X + X2, Y + Y2, Width, Height, 3, Color.White);
            var c = Color.White;
            if (enemy.CountDown() >= enemy.Enemy.traveltime())
            {
                c = Color.White;
                if (enemy.Enemy.Killable())
                {
                    c = Color.Red;
                    Drawing.DrawLine(startpoint.X, startpoint.Y - Scale * enemy.Enemy.traveltime() - 3, startpoint.X - 20, startpoint.Y - Scale * enemy.Enemy.traveltime() - 3, 3, c);
                }
            }
            var text = "(" + (int)enemy.Enemy.GetDamage() + "|" + (int)enemy.Enemy.Health + ")" + enemy.Enemy.BaseSkinName;
            var textlength = text.Length * 9;
            Drawing.DrawText(startpoint.X - textlength, startpoint.Y - Scale * enemy.CountDown() - 10, c, text);

            var linex = startpoint.X - 20;
            var liney = startpoint.Y - Scale * enemy.CountDown() - 3;
            Drawing.DrawLine(linex + 20, liney, linex, liney, 3, c);
        }

        public static void Rect(float x, float y, int width, int height, float bold, Color color)
        {
            var x2 = x + 450;
            var angle = (float)(90 * Math.PI / 180);
            for (var i = 0; i < height; i++)
            {
                startpoint = new Vector2(x2 + width, y + i).RotateAroundPoint(new Vector2(x2, y + i), angle);
                Drawing.DrawLine(new Vector2(x2, y + i), startpoint, bold, color);
            }
        }
    }
}
