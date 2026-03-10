using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;

namespace Lifehandled.Domain.Character
{
    /// <summary>
    /// Core character save/runtime data for Batch 1 foundation.
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        public string characterId = string.Empty;
        public bool isPlayerControlled;
        public CharacterRole role = CharacterRole.Npc;

        public string displayName = "New Character";
        public int ageYears = 18;
        public LifeAgeStage ageStage = LifeAgeStage.YoungAdult;
        public LifeAgeSubStage ageSubStage = LifeAgeSubStage.A;

        public AppearanceProfile appearance = new();
        public GeneticProfile genetics = new();

        public string householdId = string.Empty;
        public List<string> relationshipLinkIds = new();

        public void RecalculateAgeClassification()
        {
            AgeStageClassifier.Resolve(ageYears, out ageStage, out ageSubStage);
        }
    }
}
