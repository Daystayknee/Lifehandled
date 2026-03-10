using System.Linq;
using Lifehandled.Application.Content;
using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    public class ConsumeStarterItemUseCase
    {
        private readonly SurvivalNeedsTickUseCase _survivalNeedsTickUseCase = new();

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.playerCharacter == null)
            {
                message = "No active session.";
                return false;
            }

            var candidateItemId = ResolveBestConsumableItem(context);
            if (string.IsNullOrWhiteSpace(candidateItemId))
            {
                message = "No consumable item available.";
                return false;
            }

            if (!context.inventory.TryRemove(candidateItemId, 1))
            {
                message = "No consumable item available.";
                return false;
            }

            var needs = context.playerCharacter.needsStatus;
            if (!PrototypeWorldContentCatalog.TryGetFoodDefinition(candidateItemId, out var foodDef))
            {
                message = "Consumed item, but no food definition exists.";
                return true;
            }

            needs.hunger = NeedsStatus.ClampToRange(needs.hunger - foodDef.hungerRestore);
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst - foodDef.hydrationRestore);
            needs.mood = NeedsStatus.ClampToRange(needs.mood + foodDef.moodDelta);
            needs.energy = NeedsStatus.ClampToRange(needs.energy + foodDef.energyDelta);
            needs.illnessRisk = NeedsStatus.ClampToRange(needs.illnessRisk + foodDef.illnessRiskDelta);

            if (foodDef.illnessRiskDelta > 0f)
            {
                _survivalNeedsTickUseCase.ApplyBadFoodEffect(context);
            }

            message = $"Consumed {candidateItemId}.";
            return true;
        }

        private static string ResolveBestConsumableItem(GameSessionContext context)
        {
            var preferred = new[] { "simple_meal", "water_bottle", "stale_food" };
            foreach (var itemId in preferred)
            {
                if (context.inventory.GetCount(itemId) > 0)
                {
                    return itemId;
                }
            }

            return PrototypeWorldContentCatalog.FoodDefinitions
                .Select(fd => fd.itemId)
                .FirstOrDefault(itemId => context.inventory.GetCount(itemId) > 0);
        }
    }
}
