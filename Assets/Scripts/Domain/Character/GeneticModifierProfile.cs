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
        public float metabolismMultiplier = 1f;
        public float hungerDecayMultiplier = 1f;
        public float thirstDecayMultiplier = 1f;
        public float energyDecayMultiplier = 1f;
        public float staminaRecoveryMultiplier = 1f;
        public float sleepRecoveryMultiplier = 1f;
        public float sleepQualityMultiplier = 1f;
        public float stressGainMultiplier = 1f;
        public float moodDropMultiplier = 1f;
        public float illnessRiskGainMultiplier = 1f;
    }
}
