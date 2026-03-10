using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;

namespace Lifehandled.Domain.Character
{
    /// <summary>
    /// Minimal genetics payload for V1. Values are normalized (0..1) scalar genes.
    /// </summary>
    [Serializable]
    public class GeneticProfile
    {
        public int genomeVersion = 1;
        public LineageProfile lineage = new();
        public List<GeneValue> geneValues = new();
        public List<string> geneticTraitIds = new();
        public List<string> healthPredispositionIds = new();
        public InheritanceMetadata inheritance = new();
    }

    [Serializable]
    public class GeneValue
    {
        public string geneId = string.Empty;
        public float value;
    }

    [Serializable]
    public class LineageProfile
    {
        public string parentACharacterId = string.Empty;
        public string parentBCharacterId = string.Empty;
        public List<string> ancestorCharacterIds = new();
    }

    [Serializable]
    public class InheritanceMetadata
    {
        public bool isFounder = true;
        public int generationIndex;
        public GeneSourceType geneSource = GeneSourceType.Randomized;
    }
}
