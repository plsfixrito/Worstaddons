using System;
using EloBuddy;
using EloBuddy.SDK;
using KappaUtility.Common.TeleportsHandler;
using SharpDX;

namespace KappaUtility.Brain.Utility.Tracker.TeleportTracker
{
    internal class Recallbar
    {
        public static float X2;
        public static float Y2;

        public static float X = Drawing.Width * 0.425f;
        public static float Y = Drawing.Height * 0.80f;

        private static int Width = (int)(Drawing.Width - 2 * X);
        private static int Height = 6;

        private static readonly float Scale = (float)Width / 8000;
        private static Vector2 startpoint;

        public static void RecallBarDraw(Obj_AI_Base sender, TeleportInfo tp)
        {
            Rect(X + X2, Y + Y2, Width, Height, 3, Color.White);
            if(tp == null)
                return;

            var c = Color.GreenYellow.ToSystem();
            if (tp.Sender.IsEnemy)
                c = Color.Red.ToSystem();
            var text = sender.BaseSkinName;
            var textlength = text.Length * 12;
            Drawing.DrawText(startpoint.X - textlength, startpoint.Y - Scale * tp.TimeLeft - 10, c, text);

            var linex = startpoint.X - 20;
            var liney = startpoint.Y - Scale * tp.TimeLeft - 3;
            Drawing.DrawLine(linex + 20, liney, linex, liney, 3, c);
        }

        public static void Rect(float x, float y, int width, int height, float bold, Color color)
        {
            var x2 = x + 450;
            var angle = (float)(90 * Math.PI / 180);
            for (var i = 0; i < height; i++)
            {
                startpoint = new Vector2(x2 + width, y + i).RotateAroundPoint(new Vector2(x2, y + i), angle);
                Drawing.DrawLine(new Vector2(x2, y + i), startpoint, bold, color.ToSystem());
            }
        }
    }
}
