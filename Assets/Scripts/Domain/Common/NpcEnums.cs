using System;

namespace Lifehandled.Domain.Common
{
    [Serializable]
    public enum PersonalityTraitType
    {
        Kind = 0,
        Irritable = 1,
        Reserved = 2,
        Outgoing = 3,
        Practical = 4,
        Anxious = 5
    }

    [Serializable]
    public enum NpcScheduleBlock
    {
        Home = 0,
        Work = 1,
        Commute = 2,
        Errands = 3,
        Social = 4,
        Sleep = 5
    }

    [Serializable]
    public enum DramaEventType
    {
        HelpedNpc = 0,
        InsultedNpc = 1,
        StoleFromShop = 2,
        Gossiped = 3
    }
}
