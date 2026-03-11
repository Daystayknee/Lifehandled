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

            if (context.currentZone != ZoneType.Store && context.currentZone != ZoneType.GasStation)
            {
                message = "You need to be at the store or gas station to buy items.";
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
            var repNote = ResolveShopReputationTier(context?.socialReputation ?? 50f);
            context.lastEconomyEvent = $"Shop reacts to your reputation ({repNote}).";
            message = $"Bought water bottle for ${price}. {repNote}.";
            return true;
        }

        public static int ResolvePrice(GameSessionContext context)
        {
            var multiplier = EconomySimulationTickUseCase.ResolveFinalItemPriceMultiplier(
                context?.economy,
                WaterBottleRarityMultiplier);

            var negotiationLevel = context?.progression?.GetSkillLevel("negotiation") ?? 1;
            var discountMultiplier = 1f - ((negotiationLevel - 1) * 0.02f);
            if (discountMultiplier < 0.8f)
            {
                discountMultiplier = 0.8f;
            }

            multiplier *= discountMultiplier;

            var socialReputation = context?.socialReputation ?? 50f;
            var reputationMultiplier = ResolveShopReputationPriceMultiplier(socialReputation);
            multiplier *= reputationMultiplier;

            return ResolvePrice(multiplier);
        }


        public static float ResolveShopReputationPriceMultiplier(float socialReputation)
        {
            if (socialReputation >= 80f) return 0.86f;
            if (socialReputation >= 65f) return 0.93f;
            if (socialReputation >= 45f) return 1f;
            if (socialReputation >= 30f) return 1.1f;
            return 1.22f;
        }

        public static string ResolveShopReputationTier(float socialReputation)
        {
            if (socialReputation >= 80f) return "Local favorite discount";
            if (socialReputation >= 65f) return "Trusted regular pricing";
            if (socialReputation >= 45f) return "Neutral pricing";
            if (socialReputation >= 30f) return "Suspicious customer surcharge";
            return "Blacklisted premium surcharge";
        }

        public static int ResolvePrice(float multiplier)
        {
            var scaled = BaseWaterBottlePrice * multiplier;
            if (scaled < 1f) scaled = 1f;
            return (int)System.Math.Ceiling(scaled);
        }
    }
}
