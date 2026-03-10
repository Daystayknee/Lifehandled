using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class CookSimpleMealUseCase
    {
        public const string RawFoodItemId = "stale_food";
        public const string SimpleMealItemId = "simple_meal";

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.home == null || context.playerCharacter?.needsStatus == null)
            {
                message = "No active session.";
                return false;
            }

            if (!context.inventory.TryRemove(RawFoodItemId, 1))
            {
                message = "No raw food to cook.";
                return false;
            }

            var capacity = context.home.GetStorageCapacity();
            if (!context.inventory.TryAddWithCapacity(SimpleMealItemId, 1, capacity))
            {
                // Revert if storage is full.
                context.inventory.Add(RawFoodItemId, 1);
                message = "Storage is full.";
                return false;
            }

            var needs = context.playerCharacter.needsStatus;
            needs.hygiene = NeedsStatus.ClampToRange(needs.hygiene - 2f);
            context.home.cleanliness = NeedsStatus.ClampToRange(context.home.cleanliness - 1.5f);

            message = "Cooked a simple meal.";
            return true;
        }
    }
}
