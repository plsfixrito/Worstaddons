using EloBuddy;
using SharpDX;

namespace KappaUtility.Common.Misc.Entities
{
    internal static class Wards
    {
        public class DetectedWards
        {
            public Obj_AI_Minion Ward;
            public Vector3 Position;
            public WardType Type;
            public int StartTime;
            public int EndTime;

            public DetectedWards(Obj_AI_Minion ward, Vector3 position, WardType type, int starttime, int endtime)
            {
                this.Ward = ward;
                this.Position = position;
                this.StartTime = starttime;
                this.EndTime = endtime;
                this.Type = type;
            }
        }

        public enum WardType
        {
            SightWard,
            VisionWard,
            BlueWard,
            Unknown
        }

        public static WardType Type(this Obj_AI_Base ward)
        {
            switch (ward.BaseSkinName)
            {
                case "VisionWard":
                    return WardType.VisionWard;
                case "YellowTrinket":
                    return WardType.SightWard;
                case "SightWard":
                    return WardType.SightWard;
                case "BlueTrinket":
                    return WardType.BlueWard;
                default:
                    return WardType.Unknown;
            }
        }

        public static Color color(DetectedWards ward)
        {
            switch (ward.Type)
            {
                case WardType.VisionWard:
                    return Color.Purple;
                case WardType.SightWard:
                    return Color.Chartreuse;
                case WardType.BlueWard:
                    return Color.Blue;
                default:
                    return Color.AliceBlue;
            }
        }
    }
}
