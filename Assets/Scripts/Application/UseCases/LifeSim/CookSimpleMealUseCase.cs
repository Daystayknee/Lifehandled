using System.Linq;
using Lifehandled.Application.Content;
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

            var capacity = context.home.GetStorageCapacity();
            var recipe = ResolveBestAvailableRecipe(context);
            if (recipe != null)
            {
                if (!TryCookRecipe(context, recipe, capacity, out message))
                {
                    return false;
                }

                return true;
            }

            if (!context.inventory.TryRemove(RawFoodItemId, 1))
            {
                message = "No raw food to cook.";
                return false;
            }

            if (!context.inventory.TryAddWithCapacity(SimpleMealItemId, 1, capacity))
            {
                context.inventory.Add(RawFoodItemId, 1);
                message = "Storage is full.";
                return false;
            }

            if (TryBurnMeal(context, SimpleMealItemId, capacity, out message))
            {
                return true;
            }

            ApplyCookingSideEffects(context, cleanlinessCost: 1.5f, hygieneCost: 2f, moodBonus: 0.8f);
            context.lastHouseholdEvent = "Cooked a simple meal at home.";
            message = "Cooked a simple meal.";
            return true;
        }

        private static bool TryCookRecipe(GameSessionContext context, RecipeDefinition recipe, int capacity, out string message)
        {
            foreach (var ingredient in recipe.ingredientItemIds)
            {
                if (!context.inventory.TryRemove(ingredient, 1))
                {
                    message = "Missing recipe ingredients.";
                    return false;
                }
            }

            if (!context.inventory.TryAddWithCapacity(recipe.outputItemId, 1, capacity))
            {
                foreach (var ingredient in recipe.ingredientItemIds)
                {
                    context.inventory.Add(ingredient, 1);
                }

                message = "Storage is full.";
                return false;
            }

            if (TryBurnMeal(context, recipe.outputItemId, capacity, out message))
            {
                return true;
            }

            ApplyCookingSideEffects(context, cleanlinessCost: 2.2f, hygieneCost: 2.4f, moodBonus: 1.6f);
            context.lastHouseholdEvent = $"Cooked recipe: {recipe.outputItemId}.";
            message = $"Cooked {recipe.outputItemId} from recipe.";
            return true;
        }

        private static RecipeDefinition ResolveBestAvailableRecipe(GameSessionContext context)
        {
            return PrototypeWorldContentCatalog.RecipeDefinitions
                .Where(r => r.ingredientItemIds != null && r.ingredientItemIds.Count > 0)
                .Where(r => r.ingredientItemIds.All(i => context.inventory.GetCount(i) > 0))
                .OrderByDescending(r => r.ingredientItemIds.Count)
                .ThenBy(r => r.recipeId)
                .FirstOrDefault();
        }

        private static bool TryBurnMeal(GameSessionContext context, string cookedItemId, int capacity, out string message)
        {
            var cookingLevel = context.progression?.GetSkillLevel("cooking") ?? 1;
            var burnChance = 0.2f - ((cookingLevel - 1) * 0.03f);
            if (burnChance < 0.04f)
            {
                burnChance = 0.04f;
            }

            if (new System.Random((context.currentDay * 997) + (int)(context.hourOfDay * 100f)).NextDouble() >= burnChance)
            {
                message = string.Empty;
                return false;
            }

            context.inventory.TryRemove(cookedItemId, 1);
            context.inventory.TryAddWithCapacity("stale_food", 1, capacity);
            context.lastHouseholdEvent = "Meal overcooked; kitchen tension rises.";
            message = "Meal overcooked. You recovered stale food instead.";
            return true;
        }

        private static void ApplyCookingSideEffects(GameSessionContext context, float cleanlinessCost, float hygieneCost, float moodBonus)
        {
            var needs = context.playerCharacter.needsStatus;
            needs.hygiene = NeedsStatus.ClampToRange(needs.hygiene - hygieneCost);
            needs.mood = NeedsStatus.ClampToRange(needs.mood + moodBonus);
            context.home.cleanliness = NeedsStatus.ClampToRange(context.home.cleanliness - cleanlinessCost);
        }
    }
}
