using System;
using Lifehandled.Domain.Character;

namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Minimal runtime needs/status container for VS01 Batch 1 scene startup.
    /// </summary>
    [Serializable]
    public class NeedsStatus
    {
        public float hunger = 60f;
        public float thirst = 60f;
        public float energy = 75f;
        public float stress = 25f;

        public static NeedsStatus CreateDefault(GeneticModifierProfile modifiers)
        {
            modifiers ??= new GeneticModifierProfile();

            // Keep startup values conservative; just seed slight personality differences.
            var status = new NeedsStatus
            {
                hunger = Clamp01To100(60f * modifiers.hungerDecayMultiplier),
                thirst = Clamp01To100(60f * modifiers.thirstDecayMultiplier),
                energy = Clamp01To100(75f * modifiers.sleepRecoveryMultiplier),
                stress = Clamp01To100(25f * modifiers.stressGainMultiplier)
            };

            return status;
        }

        private static float Clamp01To100(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
