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
        Anxious = 5,
        Introverted = 6,
        Ambitious = 7,
        Lazy = 8,
        Empathetic = 9
    }

    [Serializable]
    public enum LifestyleTraitType
    {
        Neat = 0,
        Messy = 1,
        Frugal = 2,
        Extravagant = 3
    }

    [Serializable]
    public enum SurvivalTraitType
    {
        StrongMetabolism = 0,
        Resilient = 1,
        FragileImmuneSystem = 2
    }

    [Serializable]
    public enum NpcArchetypeType
    {
        ShopOwner = 0,
        Bartender = 1,
        Mechanic = 2,
        Nurse = 3,
        Teacher = 4,
        Neighbor = 5,
        Friend = 6,
        Rival = 7,
        RomanticInterest = 8,
        WealthyResident = 9,
        MysteriousOutsider = 10,
        TravelingMerchant = 11
    }

    [Serializable]
    public enum DialogueStyleType
    {
        Casual = 0,
        Formal = 1,
        Warm = 2,
        Blunt = 3,
        Flirty = 4,
        Cryptic = 5
    }

    [Serializable]
    public enum VoiceType
    {
        Deep = 0,
        Soft = 1,
        Raspy = 2,
        Energetic = 3,
        Monotone = 4,
        Cheerful = 5
    }

    [Serializable]
    public enum SpeechStyleType
    {
        Formal = 0,
        Casual = 1,
        Sarcastic = 2,
        Blunt = 3,
        Shy = 4
    }

    [Serializable]
    public enum EmotionalTraitType
    {
        Optimistic = 0,
        Anxious = 1,
        Jealous = 2,
        Loyal = 3,
        Stubborn = 4,
        Forgiving = 5
    }

    [Serializable]
    public enum SocialTraitType
    {
        Flirtatious = 0,
        Introverted = 1,
        Charismatic = 2,
        Awkward = 3,
        Manipulative = 4
    }

    [Serializable]
    public enum EducationLevelType
    {
        NoFormalEducation = 0,
        HighSchool = 1,
        TradeSchool = 2,
        College = 3,
        Graduate = 4
    }

    [Serializable]
    public enum OccupationCategoryType
    {
        Service = 0,
        Labor = 1,
        Professional = 2,
        Creative = 3,
        Criminal = 4
    }

    [Serializable]
    public enum SocialEventType
    {
        Party = 0,
        Festival = 1,
        Market = 2,
        Wedding = 3,
        Funeral = 4,
        Protest = 5,
        Emergency = 6
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
