using EloBuddy;
using SharpDX;

namespace KappaUtility.Brain.Utility.Tracker.SpellTracker
{
    internal sealed class DetectedSpellInfo : SpellInfo
    {
        public float EndTime { get; private set; }
        public int NetworkId { get; private set; }
        public Vector3 Position { get; private set; }
        public Obj_AI_Base Sender { get; private set; }
        public GameObject Object { get; private set; }

        public DetectedSpellInfo(string spellName, string championName, float spellTime, SpellType spellType, string objectName,
            float endTime, int networkId, Vector3 positon, Obj_AI_Base sender, GameObject obj) : base(spellName, championName, spellTime, spellType, objectName)
        {
            EndTime = endTime;
            NetworkId = networkId;
            Position = positon;
            Sender = sender;
            Object = obj;
        }
    }
}