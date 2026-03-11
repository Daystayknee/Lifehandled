using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class BuyFurnitureUseCase
    {
        public const int FurniturePrice = 25;

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null || context.home == null)
            {
                message = "No active session.";
                return false;
            }

            if (context.wallet < FurniturePrice)
            {
                message = "Not enough money for furniture.";
                return false;
            }

            context.wallet -= FurniturePrice;
            context.home.furnitureCount += 1;
            context.home.homeComfort = NeedsStatus.ClampToRange(context.home.homeComfort + 8f);
            context.home.storageCapacityBonus += 1;
            context.home.neighborhoodReputation = NeedsStatus.ClampToRange(context.home.neighborhoodReputation + 2f);

            message = "Bought furniture. Comfort and storage increased.";
            return true;
        }
    }
}
