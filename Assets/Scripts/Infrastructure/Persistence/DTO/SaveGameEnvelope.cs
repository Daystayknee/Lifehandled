using System;
using System.Collections.Generic;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;

namespace Lifehandled.Infrastructure.Persistence.DTO
{
    /// <summary>
    /// Minimal save envelope update for character/household/genetics + VS01 runtime loop state.
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
        public int wallet = 20;
        public string currentLocationId = "home";
        public int talkCountToday;

        public List<Vs01ItemStack> inventory = new();

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
}
