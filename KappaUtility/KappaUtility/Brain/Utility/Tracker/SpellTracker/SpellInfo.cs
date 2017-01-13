namespace KappaUtility.Brain.Utility.Tracker.SpellTracker
{
    internal class SpellInfo
    {
        public string ObjectName { get; private set; }
        public SpellType SpellType { get; private set; }
        public string SpellName { get; private set; }
        public string ChampionName { get; private set; }
        public float SpellTime { get; private set; }

        public SpellInfo(string spellName, string championName, float spellTime, SpellType spellType , string objectName)
        {
            ObjectName = objectName;
            SpellType = spellType;
            SpellName = spellName;
            ChampionName = championName;
            SpellTime = spellTime;
        }
    }

    public enum SpellType
    {
        GameObject,
        Buff,
        Minion
    }
}