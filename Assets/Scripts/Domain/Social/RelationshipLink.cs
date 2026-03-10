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
        public float familiarity;
        public float trust;
        public float affection;
        public float tension;
        public float respect;
    }
}
