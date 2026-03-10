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
        public int talkCountToday;

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

        public List<Vs01ItemStack> inventory = new();
        public List<Vs01NpcState> npcs = new();

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
        public List<PersonalityTraitType> personalityTraits = new();

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
}
