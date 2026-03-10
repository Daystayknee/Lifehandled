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

        public List<PersonalityTraitType> personalityTraits = new();
        public NpcNeeds needs = new();
        public float mood = 60f;

        public NpcSchedule schedule = new();
        public RelationshipStats relationshipToPlayer = new();
        public List<NpcMemoryEntry> memory = new();
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
}
