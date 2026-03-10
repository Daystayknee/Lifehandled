using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    /// <summary>
    /// VS01 Batch 5: minimal sleep/end-day action.
    /// Advances day and adjusts key needs in a conservative way.
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

            var needs = context.playerCharacter.needsStatus;
            needs.energy = NeedsStatus.ClampToRange(needs.energy + 35f);
            needs.stress = NeedsStatus.ClampToRange(needs.stress - 15f);
            needs.hunger = NeedsStatus.ClampToRange(needs.hunger + 12f);
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst + 10f);

            message = $"Slept. Day is now {context.currentDay}.";
            return true;
        }
    }
}
