using System;
using System.Collections.Generic;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;

namespace Lifehandled.Infrastructure.Persistence.DTO
{
    /// <summary>
    /// Minimal save envelope update for Batch 1 character/household/genetics support.
    /// </summary>
    [Serializable]
    public class SaveGameEnvelope
    {
        public int saveVersion = 1;

        // Existing systems can continue to use their current sections while this batch
        // adds only the character/household/genetics foundations.
        public string playerCharacterId = string.Empty;
        public int geneticSchemaVersion = 1;

        public List<CharacterData> characters = new();
        public List<HouseholdData> households = new();
        public List<RelationshipLink> relationshipLinks = new();
    }
}
