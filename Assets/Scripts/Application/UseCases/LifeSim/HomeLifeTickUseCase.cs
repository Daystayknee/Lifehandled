using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    /// <summary>
    /// Small life-sim tick for home cleanliness, mood pressure, and neighborhood drift.
    /// </summary>
    public class HomeLifeTickUseCase
    {
        public void Execute(GameSessionContext context, float inGameMinutes)
        {
            if (context == null || context.home == null || context.playerCharacter?.needsStatus == null)
            {
                return;
            }

            var home = context.home;
            var needs = context.playerCharacter.needsStatus;

            var hours = inGameMinutes / 60f;
            home.cleanliness = NeedsStatus.ClampToRange(home.cleanliness - (0.4f * hours));

            if (home.cleanliness < 35f)
            {
                needs.mood = NeedsStatus.ClampToRange(needs.mood - (0.8f * hours));
            }

            // Better-kept homes slightly improve neighborhood reputation over time.
            if (home.cleanliness > 70f)
            {
                home.neighborhoodReputation = NeedsStatus.ClampToRange(home.neighborhoodReputation + (0.15f * hours));
            }
            else if (home.cleanliness < 30f)
            {
                home.neighborhoodReputation = NeedsStatus.ClampToRange(home.neighborhoodReputation - (0.2f * hours));
            }
        }
    }
}
