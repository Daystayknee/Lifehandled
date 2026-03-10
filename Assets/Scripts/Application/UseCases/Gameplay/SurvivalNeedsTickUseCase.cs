using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.Gameplay
{
    /// <summary>
    /// Minimal survival progression tick for VS01.
    /// Minutes are in real-time game loop units.
    /// </summary>
    public class SurvivalNeedsTickUseCase
    {
        public void Execute(GameSessionContext context, float minutes)
        {
            if (context == null || context.playerCharacter?.needsStatus == null)
            {
                return;
            }

            var needs = context.playerCharacter.needsStatus;
            var modifiers = context.playerCharacter.geneticModifiers ?? new GeneticModifierProfile();

            // Core decays
            needs.hunger = NeedsStatus.ClampToRange(needs.hunger + (0.14f * minutes * modifiers.hungerDecayMultiplier));
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst + (0.18f * minutes * modifiers.thirstDecayMultiplier));
            needs.energy = NeedsStatus.ClampToRange(needs.energy - (0.11f * minutes * modifiers.energyDecayMultiplier));
            needs.hygiene = NeedsStatus.ClampToRange(needs.hygiene - (0.09f * minutes));

            // Weather/wetness interactions
            if (context.weather == WeatherType.Rain || context.weather == WeatherType.Storm)
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
            needs.stress = NeedsStatus.ClampToRange(needs.stress + (stressGain * minutes * modifiers.stressGainMultiplier * modifiers.stressToleranceMultiplier));

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
            illnessDelta -= (0.03f * modifiers.immuneStrengthMultiplier); // slow recovery if conditions improve
            needs.illnessRisk = NeedsStatus.ClampToRange(needs.illnessRisk + (illnessDelta * minutes));

            TickBodyAndSkinConditions(context, needs, modifiers, minutes);
        }

        public void ApplyBadFoodEffect(GameSessionContext context)
        {
            var needs = context?.playerCharacter?.needsStatus;
            if (needs == null)
            {
                return;
            }

            var modifiers = context.playerCharacter.geneticModifiers ?? new GeneticModifierProfile();

            needs.illnessRisk = NeedsStatus.ClampToRange(needs.illnessRisk + (14f * modifiers.illnessRiskGainMultiplier));
            needs.mood = NeedsStatus.ClampToRange(needs.mood - (8f * modifiers.moodDropMultiplier));
            needs.stress = NeedsStatus.ClampToRange(needs.stress + (6f * modifiers.stressGainMultiplier));

            var appearance = context.playerCharacter.data?.appearance;
            if (appearance != null)
            {
                appearance.illness01 = Clamp01(appearance.illness01 + 0.12f);
                appearance.acne01 = Clamp01(appearance.acne01 + 0.04f);
                appearance.dryness01 = Clamp01(appearance.dryness01 + 0.02f);
            }
        }

        private static void TickBodyAndSkinConditions(GameSessionContext context, NeedsStatus needs, GeneticModifierProfile modifiers, float minutes)
        {
            var appearance = context.playerCharacter?.data?.appearance;
            if (appearance == null)
            {
                return;
            }

            var t = minutes * 0.01f;

            // Body condition states
            appearance.fatigue01 = Clamp01(appearance.fatigue01 + (t * (needs.energy < 35f ? 2.2f : -0.7f)));
            appearance.dehydration01 = Clamp01(appearance.dehydration01 + (t * (needs.thirst > 65f ? 2.4f : -0.9f)));
            appearance.illness01 = Clamp01(appearance.illness01 + (t * ((needs.illnessRisk / 100f) * modifiers.illnessRiskGainMultiplier - 0.04f * modifiers.immuneStrengthMultiplier)));
            appearance.injury01 = Clamp01(appearance.injury01 + (context.currentZone == ZoneType.Forest ? t * 0.1f : -t * 0.05f));
            appearance.weightShift01 = Clamp01(appearance.weightShift01 + (t * ((needs.hunger > 70f ? -0.2f : 0.06f) + (needs.energy < 30f ? -0.05f : 0.02f))));
            appearance.muscleGrowth01 = Clamp01(appearance.muscleGrowth01 + (t * (needs.energy > 60f ? 0.07f : -0.04f)));
            appearance.scarVisibility01 = Clamp01(appearance.scarVisibility01 + (appearance.injury01 > 0.4f ? t * 0.05f : 0f));

            // Skin condition reactions to weather/hygiene/diet proxies
            var harshWeather = context.weather == WeatherType.Storm || context.weather == WeatherType.Rain;
            appearance.rashes01 = Clamp01(appearance.rashes01 + (harshWeather && needs.hygiene < 40f ? t * 0.18f : -t * 0.06f));
            appearance.dryness01 = Clamp01(appearance.dryness01 + (needs.thirst > 70f ? t * 0.25f : -t * 0.08f));
            appearance.acne01 = Clamp01(appearance.acne01 + (needs.hygiene < 35f ? t * 0.2f : -t * 0.07f));
            appearance.sunburn01 = Clamp01(appearance.sunburn01 + (context.weather == WeatherType.Clear && context.isDaytime && context.currentZone != ZoneType.Home ? t * 0.12f : -t * 0.1f));
            appearance.sunDamage01 = Clamp01(appearance.sunDamage01 + (appearance.sunburn01 > 0.35f ? t * 0.03f : -t * 0.01f));

            // Hair and aging drift
            appearance.hairDensity01 = Clamp01(appearance.hairDensity01 - (t * 0.01f * modifiers.agingRateMultiplier));
            appearance.baldingPattern01 = Clamp01(appearance.baldingPattern01 + (t * 0.01f * modifiers.agingRateMultiplier));
            appearance.grayingRate01 = Clamp01(appearance.grayingRate01 + (t * 0.015f * modifiers.agingRateMultiplier));
        }

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }
    }
}
