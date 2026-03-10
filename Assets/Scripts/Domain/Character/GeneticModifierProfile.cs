using System;

namespace Lifehandled.Domain.Character
{
    /// <summary>
    /// Narrow gameplay multipliers used in Batch 3 runtime initialization.
    /// Keep values conservative for prototype stability.
    /// </summary>
    [Serializable]
    public class GeneticModifierProfile
    {
        public float hungerDecayMultiplier = 1f;
        public float thirstDecayMultiplier = 1f;
        public float sleepRecoveryMultiplier = 1f;
        public float stressGainMultiplier = 1f;
    }
}
