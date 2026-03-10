using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Economy;

namespace Lifehandled.Application.UseCases.Gameplay
{
    /// <summary>
    /// Minimal end-day action with basic sleep quality effects.
    /// Poor sleep increases stress and reduces mood.
    /// </summary>
    public class SleepEndDayUseCase
    {
        private readonly DailyEconomySettlementUseCase _dailyEconomySettlementUseCase = new();

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.playerCharacter?.needsStatus == null)
            {
                message = "No active session.";
                return false;
            }

            context.currentDay += 1;
            context.hourOfDay = 7f;
            var needs = context.playerCharacter.needsStatus;
            var modifiers = context.playerCharacter.geneticModifiers ?? new Domain.Character.GeneticModifierProfile();

            var sleepQuality = ComputeSleepQuality(needs, context.home, modifiers);

            // Energy recovery depends on sleep quality.
            needs.energy = NeedsStatus.ClampToRange(needs.energy + ((20f + (20f * sleepQuality)) * modifiers.staminaRecoveryMultiplier * modifiers.sleepRecoveryMultiplier));

            // Overnight baseline changes.
            needs.hunger = NeedsStatus.ClampToRange(needs.hunger + 10f);
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst + 10f);
            needs.wetness = NeedsStatus.ClampToRange(needs.wetness - 20f);

            if (sleepQuality < 0.4f)
            {
                needs.stress = NeedsStatus.ClampToRange(needs.stress + 8f);
                needs.mood = NeedsStatus.ClampToRange(needs.mood - 10f);
                var economyResult = _dailyEconomySettlementUseCase.Execute(context);
                message = $"Poor sleep. Day is now {context.currentDay}. {economyResult}";
            }
            else
            {
                needs.stress = NeedsStatus.ClampToRange(needs.stress - 8f);
                needs.mood = NeedsStatus.ClampToRange(needs.mood + 5f);
                var economyResult = _dailyEconomySettlementUseCase.Execute(context);
                message = $"Slept well. Day is now {context.currentDay}. {economyResult}";
            }

            return true;
        }

        private static float ComputeSleepQuality(NeedsStatus needs, HomeLifeState home, Domain.Character.GeneticModifierProfile modifiers)
        {
            // 0..1 composite from warmth, hygiene, home comfort, cleanliness, and inverse stress.
            var warmthScore = needs.warmth / 100f;
            var hygieneScore = needs.hygiene / 100f;
            var stressScore = 1f - (needs.stress / 100f);
            var comfortScore = (home?.homeComfort ?? 50f) / 100f;
            var cleanlinessScore = (home?.cleanliness ?? 50f) / 100f;

            var raw = (warmthScore * 0.25f)
                      + (hygieneScore * 0.2f)
                      + (stressScore * 0.25f)
                      + (comfortScore * 0.2f)
                      + (cleanlinessScore * 0.1f);
            raw *= modifiers?.sleepQualityMultiplier ?? 1f;
            if (raw < 0f) return 0f;
            if (raw > 1f) return 1f;
            return raw;
        }
    }
}
