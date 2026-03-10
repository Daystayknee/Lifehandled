using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Economy;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.Gameplay
{
    public class BuyStarterItemUseCase
    {
        public const string WaterBottleId = "water_bottle";
        public const int BaseWaterBottlePrice = 5;
        public const float WaterBottleRarityMultiplier = 1f;

        public bool Execute(GameSessionContext context, out string message)
        {
            if (context == null)
            {
                message = "No active session.";
                return false;
            }

            if (!context.shopOpen)
            {
                message = "Shop is closed right now.";
                return false;
            }

            if (context.currentZone != ZoneType.Store)
            {
                message = "You need to be at the store to buy items.";
                return false;
            }

            var capacity = context.home?.GetStorageCapacity() ?? 6;
            if (context.inventory.GetTotalItemCount() >= capacity)
            {
                message = "Storage full at home. Expand storage first.";
                return false;
            }

            var price = ResolvePrice(context);
            if (context.wallet < price)
            {
                message = "Not enough money.";
                return false;
            }

            context.wallet -= price;
            context.inventory.TryAddWithCapacity(WaterBottleId, 1, capacity);
            message = $"Bought water bottle for ${price}.";
            return true;
        }

        public static int ResolvePrice(GameSessionContext context)
        {
            var multiplier = EconomySimulationTickUseCase.ResolveFinalItemPriceMultiplier(
                context?.economy,
                WaterBottleRarityMultiplier);

            return ResolvePrice(multiplier);
        }

        public static int ResolvePrice(float multiplier)
        {
            var scaled = BaseWaterBottlePrice * multiplier;
            if (scaled < 1f) scaled = 1f;
            return (int)System.Math.Ceiling(scaled);
        }
    }
}
