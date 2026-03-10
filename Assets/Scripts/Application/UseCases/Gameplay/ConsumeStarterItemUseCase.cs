using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    public class ConsumeStarterItemUseCase
    {
        public const string WaterBottleId = "water_bottle";

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.playerCharacter == null)
            {
                message = "No active session.";
                return false;
            }

            if (!context.inventory.TryRemove(WaterBottleId, 1))
            {
                message = "No water bottle to consume.";
                return false;
            }

            var needs = context.playerCharacter.needsStatus;
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst - 20f);
            needs.hunger = NeedsStatus.ClampToRange(needs.hunger - 4f);
            message = "Consumed water bottle.";
            return true;
        }
    }
}
