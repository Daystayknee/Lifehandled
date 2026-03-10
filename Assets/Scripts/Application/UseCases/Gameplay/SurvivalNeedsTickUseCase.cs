using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    /// <summary>
    /// Minimal survival simulation tick for VS01.
    /// Connects weather/wetness/warmth/energy and stress/mood/illness interactions.
    /// </summary>
    public class SurvivalNeedsTickUseCase
    {
        public void Execute(GameSessionContext context, bool isRaining, float minutes)
        {
            if (context == null || context.playerCharacter?.needsStatus == null)
            {
                return;
            }

            var needs = context.playerCharacter.needsStatus;
            var modifiers = context.playerCharacter.geneticModifiers ?? new Domain.Character.GeneticModifierProfile();

            // Base decays
            needs.hunger = NeedsStatus.ClampToRange(needs.hunger + (0.4f * minutes * modifiers.hungerDecayMultiplier));
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst + (0.6f * minutes * modifiers.thirstDecayMultiplier));
            needs.energy = NeedsStatus.ClampToRange(needs.energy - (0.25f * minutes * modifiers.energyDecayMultiplier));
            needs.hygiene = NeedsStatus.ClampToRange(needs.hygiene - (0.15f * minutes));

            // Rain -> wetness
            if (isRaining)
            {
                needs.wetness = NeedsStatus.ClampToRange(needs.wetness + (1.2f * minutes));
            }
            else
            {
                needs.wetness = NeedsStatus.ClampToRange(needs.wetness - (0.6f * minutes));
            }

            // Wetness -> warmth drop
            var warmthDecay = 0.12f + (needs.wetness * 0.01f);
            needs.warmth = NeedsStatus.ClampToRange(needs.warmth - (warmthDecay * minutes));

            // Cold -> additional energy drain
            if (needs.warmth < 35f)
            {
                var coldPenalty = (35f - needs.warmth) * 0.02f;
                needs.energy = NeedsStatus.ClampToRange(needs.energy - (coldPenalty * minutes * modifiers.energyDecayMultiplier));
            }

            // Stress factors
            var stressGain = 0f;
            if (needs.energy < 30f) stressGain += 0.2f;
            if (needs.warmth < 35f) stressGain += 0.2f;
            if (needs.hygiene < 30f) stressGain += 0.15f;
            needs.stress = NeedsStatus.ClampToRange(needs.stress + (stressGain * minutes * modifiers.stressGainMultiplier));

            // Mood responds to stress + poor condition
            var moodDelta = 0f;
            moodDelta -= (needs.stress > 50f ? 0.18f : 0.05f) * minutes * modifiers.moodDropMultiplier;
            if (needs.hygiene < 30f) moodDelta -= 0.08f * minutes;
            if (needs.energy < 30f) moodDelta -= 0.08f * minutes;
            needs.mood = NeedsStatus.ClampToRange(needs.mood + moodDelta);

            // Illness risk from low warmth/hygiene/high stress.
            var illnessDelta = 0f;
            if (needs.warmth < 30f) illnessDelta += 0.1f;
            if (needs.hygiene < 25f) illnessDelta += 0.1f;
            if (needs.stress > 60f) illnessDelta += 0.08f;
            illnessDelta *= modifiers.illnessRiskGainMultiplier;
            illnessDelta -= 0.03f; // slow recovery if conditions improve
            needs.illnessRisk = NeedsStatus.ClampToRange(needs.illnessRisk + (illnessDelta * minutes));
        }

        public void ApplyBadFoodEffect(GameSessionContext context)
        {
            var needs = context?.playerCharacter?.needsStatus;
            if (needs == null)
            {
                return;
            }

            var modifiers = context.playerCharacter.geneticModifiers ?? new Domain.Character.GeneticModifierProfile();

            needs.illnessRisk = NeedsStatus.ClampToRange(needs.illnessRisk + (14f * modifiers.illnessRiskGainMultiplier));
            needs.mood = NeedsStatus.ClampToRange(needs.mood - (8f * modifiers.moodDropMultiplier));
            needs.stress = NeedsStatus.ClampToRange(needs.stress + (6f * modifiers.stressGainMultiplier));
        }
    }
}
