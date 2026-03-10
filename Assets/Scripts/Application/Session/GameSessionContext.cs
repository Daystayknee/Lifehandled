using System;
using System.Collections.Generic;

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
        public int wallet = 20;
        public InventoryState inventory = new();

        public RuntimeCharacterState playerCharacter = new();
        public List<RuntimeCharacterState> householdMembers = new();
    }
}
