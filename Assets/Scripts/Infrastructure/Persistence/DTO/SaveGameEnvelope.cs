using System;
using System.Collections.Generic;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;

namespace Lifehandled.Infrastructure.Persistence.DTO
{
    /// <summary>
    /// Save envelope for character/household/genetics and VS01 runtime loop state.
    /// </summary>
    [Serializable]
    public class SaveGameEnvelope
    {
        public int saveVersion = 1;

        public string playerCharacterId = string.Empty;
        public int geneticSchemaVersion = 1;

        public List<CharacterData> characters = new();
        public List<HouseholdData> households = new();
        public List<RelationshipLink> relationshipLinks = new();

        public Vs01RuntimeState vs01State = new();
    }

    [Serializable]
    public class Vs01RuntimeState
    {
        public int currentDay = 1;
        public float hourOfDay = 8f;
        public int wallet = 20;
        public string currentLocationId = "home";
        public ZoneType currentZone = ZoneType.Home;
        public int talkCountToday;
        public float socialReputation = 50f;
        public float familyTension = 15f;
        public int generationIndex;
        public string legacyFamilyName = "Founders";
        public float legacyReputation = 50f;

        public SeasonType season = SeasonType.Spring;
        public WeatherType weather = WeatherType.Clear;
        public bool isDaytime = true;
        public bool shopOpen = true;
        public float npcOutsideFactor = 0.8f;
        public float foodPriceMultiplier = 1f;

        public JobType currentJob = JobType.PartTimeShopHelper;
        public int dailyIncome = 12;
        public HousingTier housingTier = HousingTier.Basic;
        public int weeklyRentCost = 18;
        public float scarcityMultiplier = 1f;
        public float regionWealthMultiplier = 1f;
        public float reputationMultiplier = 1f;
        public float supplyDemandMultiplier = 1f;
        public int lastIncomePaidDay;
        public int lastRentPaidDay;

        public bool isHomeOwned = true;
        public int furnitureCount = 1;
        public float homeComfort = 55f;
        public float homeCleanliness = 65f;
        public int storageCapacityBase = 6;
        public int storageCapacityBonus;
        public float neighborhoodReputation = 50f;

        public List<Vs01ItemStack> inventory = new();
        public List<Vs01NpcState> npcs = new();
        public List<Vs01ZoneState> zones = new();
        public List<Vs01SkillState> skills = new();
        public List<string> unlockedPerkIds = new();
        public List<string> collectibles = new();
        public List<string> rareEventsSeen = new();
        public List<string> generationCharacterIds = new();

        public float hunger = -1f;
        public float thirst = -1f;
        public float energy = -1f;
        public float warmth = -1f;
        public float hygiene = -1f;
        public float stress = -1f;
        public float mood = -1f;
        public float illnessRisk = -1f;
        public float wetness = -1f;
    }

    [Serializable]
    public class Vs01ItemStack
    {
        public string itemId = string.Empty;
        public int count;
    }

    [Serializable]
    public class Vs01NpcState
    {
        public string npcId = string.Empty;
        public string displayName = "NPC";
        public NpcArchetypeType archetype = NpcArchetypeType.Neighbor;
        public DialogueStyleType dialogueStyle = DialogueStyleType.Casual;
        public VoiceType voiceType = VoiceType.Soft;
        public SpeechStyleType speechStyle = SpeechStyleType.Casual;
        public string clothingStyleId = "casual_basic";
        public OccupationCategoryType occupationCategory = OccupationCategoryType.Service;
        public string occupationJobId = "barista";
        public string occupationSchedulePreset = "day_shift";
        public int occupationDailyIncome = 10;
        public float occupationReputationImpact = 1f;
        public float occupationSocialCircleRadius = 1f;
        public string upbringing = "ordinary_town";
        public EducationLevelType educationLevel = EducationLevelType.HighSchool;
        public string hometown = "founders_town";
        public List<string> pastRelationshipNotes = new();
        public List<string> previousJobs = new();
        public List<string> traumaTags = new();
        public List<string> favoriteFoodItemIds = new();
        public List<string> favoriteActivityIds = new();
        public string preferredClothingStyleId = "casual_basic";
        public string musicTasteId = "pop";
        public List<string> hobbyIds = new();
        public List<PersonalityTraitType> personalityTraits = new();
        public List<EmotionalTraitType> emotionalTraits = new();
        public List<SocialTraitType> socialTraits = new();
        public List<LifestyleTraitType> lifestyleTraits = new();
        public List<SurvivalTraitType> survivalTraits = new();

        public float hunger = 40f;
        public float energy = 70f;
        public float social = 50f;
        public float mood = 60f;

        public NpcScheduleBlock currentScheduleBlock = NpcScheduleBlock.Home;
        public bool isAvailableForTalk = true;

        public float friendship = 30f;
        public float trust = 30f;
        public float attraction;
        public float respect = 30f;
        public float fear;
        public float resentment;

        public int knownSecretsCount;
        public float gossipHeat;
        public float rumorBelief;
        public float rivalryWithPlayer;
        public float romanceInterest;
        public float familyTensionWithPlayer;
        public float socialReputationOfPlayer = 50f;

        public List<Vs01NpcMemoryEntry> memory = new();
    }

    [Serializable]
    public class Vs01NpcMemoryEntry
    {
        public int day;
        public float hour;
        public string interactionType = string.Empty;
        public string outcome = string.Empty;
    }

    [Serializable]
    public class Vs01ZoneState
    {
        public ZoneType zoneType;
        public string zoneId = string.Empty;
        public string displayName = string.Empty;
        public List<string> npcPool = new();
        public List<string> resources = new();
        public List<string> events = new();
        public float dangerLevel;
    }

    [Serializable]
    public class Vs01SkillState
    {
        public string skillId = string.Empty;
        public int level = 1;
        public float xp;
    }
}
