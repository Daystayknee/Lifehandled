using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;
using Lifehandled.Application.Session.NPC;

namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Session-level runtime context created from save data.
    /// </summary>
    [Serializable]
    public class GameSessionContext
    {
        public string playerCharacterId = string.Empty;
        public string activeHouseholdId = string.Empty;

        public int currentDay = 1;
        public float hourOfDay = 8f;
        public int wallet = 20;
        public string currentLocationId = "home";
        public int talkCountToday;
        public float socialReputation = 50f;
        public float familyTension = 15f;

        public SeasonType season = SeasonType.Spring;
        public WeatherType weather = WeatherType.Clear;
        public bool isDaytime = true;
        public bool shopOpen = true;
        public float npcOutsideFactor = 0.8f;
        public float foodPriceMultiplier = 1f;

        public EconomyState economy = new();
        public HomeLifeState home = new();
        public InventoryState inventory = new();

        public RuntimeCharacterState playerCharacter = new();
        public List<RuntimeCharacterState> householdMembers = new();
        public List<NpcRuntimeState> npcs = new();
    }
}
