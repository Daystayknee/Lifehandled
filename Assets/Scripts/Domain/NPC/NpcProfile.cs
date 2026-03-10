using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.Social;

namespace Lifehandled.Domain.NPC
{
    [Serializable]
    public class NpcProfile
    {
        public string npcId = string.Empty;
        public string displayName = "NPC";

        public NpcArchetypeType archetype = NpcArchetypeType.Neighbor;
        public DialogueStyleType dialogueStyle = DialogueStyleType.Casual;
        public VoiceType voiceType = VoiceType.Soft;
        public SpeechStyleType speechStyle = SpeechStyleType.Casual;
        public string clothingStyleId = "casual_basic";
        public OccupationProfile occupation = new();
        public LifeHistoryProfile lifeHistory = new();

        public List<PersonalityTraitType> personalityTraits = new();
        public List<EmotionalTraitType> emotionalTraits = new();
        public List<SocialTraitType> socialTraits = new();
        public List<LifestyleTraitType> lifestyleTraits = new();
        public List<SurvivalTraitType> survivalTraits = new();
        public NpcNeeds needs = new();
        public float mood = 60f;

        public NpcSchedule schedule = new();
        public RelationshipStats relationshipToPlayer = new();
        public List<NpcMemoryEntry> memory = new();
        public NpcDramaState drama = new();
    }

    [Serializable]
    public class NpcNeeds
    {
        public float hunger = 40f;
        public float energy = 70f;
        public float social = 50f;
    }

    [Serializable]
    public class NpcSchedule
    {
        public NpcScheduleBlock currentBlock = NpcScheduleBlock.Home;
        public bool isAvailableForTalk = true;
    }

    [Serializable]
    public class NpcMemoryEntry
    {
        public int day;
        public float hour;
        public string interactionType = string.Empty;
        public string outcome = string.Empty;
    }

    [Serializable]
    public class NpcDramaState
    {
        public int knownSecretsCount;
        public float gossipHeat;
        public float rumorBelief;
        public float rivalryWithPlayer;
        public float romanceInterest;
        public float familyTensionWithPlayer;
        public float socialReputationOfPlayer = 50f;
    }

    [Serializable]
    public class LifeHistoryProfile
    {
        public string upbringing = "ordinary_town";
        public EducationLevelType educationLevel = EducationLevelType.HighSchool;
        public string hometown = "founders_town";
        public List<string> pastRelationshipNotes = new();
        public List<string> previousJobs = new();
        public List<string> traumaTags = new();
    }

    [Serializable]
    public class OccupationProfile
    {
        public OccupationCategoryType category = OccupationCategoryType.Service;
        public string jobId = "barista";
        public string schedulePreset = "day_shift";
        public int dailyIncome = 10;
        public float reputationImpact = 1f;
        public float socialCircleRadius = 1f;
    }
}
