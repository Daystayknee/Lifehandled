using System;

namespace Lifehandled.Domain.Character
{
    /// <summary>
    /// Narrow gameplay multipliers used in runtime initialization.
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
        public float staminaCapacityMultiplier = 1f;
        public float sleepRecoveryMultiplier = 1f;
        public float sleepQualityMultiplier = 1f;
        public float stressGainMultiplier = 1f;
        public float stressToleranceMultiplier = 1f;
        public float moodDropMultiplier = 1f;
        public float immuneStrengthMultiplier = 1f;
        public float illnessRiskGainMultiplier = 1f;
        public float painToleranceMultiplier = 1f;
        public float agingRateMultiplier = 1f;
        public float hairGrowthSpeedMultiplier = 1f;
    }
}
