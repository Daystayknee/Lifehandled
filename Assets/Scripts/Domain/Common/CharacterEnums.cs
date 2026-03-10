using System;

namespace Lifehandled.Domain.Common
{
    /// <summary>
    /// High-level role classification for character records.
    /// </summary>
    [Serializable]
    public enum CharacterRole
    {
        PlayerMain = 0,
        HouseholdMember = 1,
        Npc = 2
    }

    [Serializable]
    public enum RelationshipType
    {
        Family = 0,
        Friend = 1,
        Roommate = 2,
        Romantic = 3,
        Rival = 4
    }

    [Serializable]
    public enum HouseholdType
    {
        Solo = 0,
        Roommates = 1,
        Family = 2,
        Mixed = 3
    }

    [Serializable]
    public enum GeneSourceType
    {
        Randomized = 0,
        Template = 1,
        Inherited = 2
    }
}
