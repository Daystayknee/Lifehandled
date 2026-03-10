using System;
using Lifehandled.Domain.Character;

namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Runtime survival state for VS01.
    /// Values are 0..100 where higher can mean "worse" for some needs (e.g. hunger/stress/illnessRisk).
    /// </summary>
    [Serializable]
    public class NeedsStatus
    {
        // Primary needs/states
        public float hunger = 60f;
        public float thirst = 60f;
        public float energy = 75f;
        public float warmth = 70f;
        public float hygiene = 70f;

        // Secondary states
        public float stress = 25f;
        public float mood = 70f;
        public float illnessRisk = 5f;

        // Environmental helper for rain/cold interaction chain.
        public float wetness = 0f;

        public static NeedsStatus CreateDefault(GeneticModifierProfile modifiers)
        {
            modifiers ??= new GeneticModifierProfile();

            return new NeedsStatus
            {
                hunger = ClampToRange(60f * modifiers.hungerDecayMultiplier),
                thirst = ClampToRange(60f * modifiers.thirstDecayMultiplier),
                energy = ClampToRange(75f * modifiers.sleepRecoveryMultiplier),
                warmth = 70f,
                hygiene = 70f,
                stress = ClampToRange(25f * modifiers.stressGainMultiplier),
                mood = 70f,
                illnessRisk = 5f,
                wetness = 0f
            };
        }

        public static float ClampToRange(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
