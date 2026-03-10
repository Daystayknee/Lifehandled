using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    public class ConsumeStarterItemUseCase
    {
        public const string WaterBottleId = "water_bottle";
        public const string BadFoodId = "stale_food";
        public const string SimpleMealId = "simple_meal";

        private readonly SurvivalNeedsTickUseCase _survivalNeedsTickUseCase = new();

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.playerCharacter == null)
            {
                message = "No active session.";
                return false;
            }

            // Prefer cooked meal first.
            if (context.inventory.TryRemove(SimpleMealId, 1))
            {
                var needs = context.playerCharacter.needsStatus;
                needs.hunger = NeedsStatus.ClampToRange(needs.hunger - 28f);
                needs.mood = NeedsStatus.ClampToRange(needs.mood + 4f);
                needs.illnessRisk = NeedsStatus.ClampToRange(needs.illnessRisk - 4f);
                message = "Ate a simple meal.";
                return true;
            }

            // Prefer safe item next.
            if (context.inventory.TryRemove(WaterBottleId, 1))
            {
                var needs = context.playerCharacter.needsStatus;
                needs.thirst = NeedsStatus.ClampToRange(needs.thirst - 20f);
                needs.hunger = NeedsStatus.ClampToRange(needs.hunger - 4f);
                message = "Consumed water bottle.";
                return true;
            }

            // Fallback bad food demonstrates illness risk interaction.
            if (context.inventory.TryRemove(BadFoodId, 1))
            {
                var needs = context.playerCharacter.needsStatus;
                needs.hunger = NeedsStatus.ClampToRange(needs.hunger - 15f);
                _survivalNeedsTickUseCase.ApplyBadFoodEffect(context);
                message = "Ate stale food. Illness risk increased.";
                return true;
            }

            message = "No consumable item available.";
            return false;
        }
    }
}
