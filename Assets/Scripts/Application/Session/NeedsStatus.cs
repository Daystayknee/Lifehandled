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
                hunger = ClampToRange(60f * modifiers.hungerDecayMultiplier),
                thirst = ClampToRange(60f * modifiers.thirstDecayMultiplier),
                energy = ClampToRange(75f * modifiers.sleepRecoveryMultiplier),
                stress = ClampToRange(25f * modifiers.stressGainMultiplier)
            };

            return status;
        }

        public static float ClampToRange(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
