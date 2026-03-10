using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class CleanHomeUseCase
    {
        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.home == null || context.playerCharacter?.needsStatus == null)
            {
                message = "No active session.";
                return false;
            }

            var needs = context.playerCharacter.needsStatus;
            if (needs.energy < 8f)
            {
                message = "Too tired to clean.";
                return false;
            }

            needs.energy = NeedsStatus.ClampToRange(needs.energy - 8f);
            needs.stress = NeedsStatus.ClampToRange(needs.stress - 4f);
            needs.mood = NeedsStatus.ClampToRange(needs.mood + 4f);

            context.home.cleanliness = NeedsStatus.ClampToRange(context.home.cleanliness + 20f);
            context.home.neighborhoodReputation = NeedsStatus.ClampToRange(context.home.neighborhoodReputation + 1f);

            message = "Home cleaned. Cleanliness and mood improved.";
            return true;
        }
    }
}
