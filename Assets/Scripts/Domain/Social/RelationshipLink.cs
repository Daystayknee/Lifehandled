using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;

namespace Lifehandled.Domain.Social
{
    /// <summary>
    /// Pairwise relationship record referenced by character IDs.
    /// </summary>
    [Serializable]
    public class RelationshipLink
    {
        public string relationshipLinkId = string.Empty;
        public string characterAId = string.Empty;
        public string characterBId = string.Empty;

        public RelationshipType linkType = RelationshipType.Friend;
        public RelationshipStats stats = new();

        public int knownSinceDay;
        public bool isHouseholdBond;
        public List<string> memoryTagIds = new();
    }

    [Serializable]
    public class RelationshipStats
    {
        public float friendship = 30f;
        public float trust = 30f;
        public float attraction = 0f;
        public float respect = 30f;
        public float fear = 0f;
        public float resentment = 0f;
    }
}
