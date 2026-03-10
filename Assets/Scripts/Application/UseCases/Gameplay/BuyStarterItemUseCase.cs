using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.Gameplay
{
    public class BuyStarterItemUseCase
    {
        public const string WaterBottleId = "water_bottle";
        public const int WaterBottlePrice = 5;

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null)
            {
                message = "No active session.";
                return false;
            }

            if (context.wallet < WaterBottlePrice)
            {
                message = "Not enough money.";
                return false;
            }

            context.wallet -= WaterBottlePrice;
            context.inventory.Add(WaterBottleId, 1);
            message = "Bought water bottle.";
            return true;
        }
    }
}
