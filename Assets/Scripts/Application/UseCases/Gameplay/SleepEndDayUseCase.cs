using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    /// <summary>
    /// Minimal end-day action with basic sleep quality effects.
    /// Poor sleep increases stress and reduces mood.
    /// </summary>
    public class SleepEndDayUseCase
    {
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

            var sleepQuality = ComputeSleepQuality(needs);

            // Energy recovery depends on sleep quality.
            needs.energy = NeedsStatus.ClampToRange(needs.energy + (20f + (20f * sleepQuality)));

            // Overnight baseline changes.
            needs.hunger = NeedsStatus.ClampToRange(needs.hunger + 10f);
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst + 10f);
            needs.wetness = NeedsStatus.ClampToRange(needs.wetness - 20f);

            if (sleepQuality < 0.4f)
            {
                needs.stress = NeedsStatus.ClampToRange(needs.stress + 8f);
                needs.mood = NeedsStatus.ClampToRange(needs.mood - 10f);
                message = $"Poor sleep. Day is now {context.currentDay}.";
            }
            else
            {
                needs.stress = NeedsStatus.ClampToRange(needs.stress - 8f);
                needs.mood = NeedsStatus.ClampToRange(needs.mood + 5f);
                message = $"Slept well. Day is now {context.currentDay}.";
            }

            return true;
        }

        private static float ComputeSleepQuality(NeedsStatus needs)
        {
            // 0..1 composite from warmth, hygiene and inverse stress.
            var warmthScore = needs.warmth / 100f;
            var hygieneScore = needs.hygiene / 100f;
            var stressScore = 1f - (needs.stress / 100f);

            var raw = (warmthScore * 0.4f) + (hygieneScore * 0.3f) + (stressScore * 0.3f);
            if (raw < 0f) return 0f;
            if (raw > 1f) return 1f;
            return raw;
        }
    }
}
