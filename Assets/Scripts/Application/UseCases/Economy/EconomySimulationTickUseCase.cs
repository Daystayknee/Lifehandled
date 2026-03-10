using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.Economy
{
    /// <summary>
    /// Lightweight economy tick: derives scarcity/supply-demand modifiers from world state.
    /// </summary>
    public class EconomySimulationTickUseCase
    {
        public void Execute(GameSessionContext context)
        {
            if (context == null)
            {
                return;
            }

            context.economy ??= new EconomyState();

            var scarcity = 1f;
            if (context.weather == WeatherType.Rain) scarcity += 0.08f;
            if (context.weather == WeatherType.Storm) scarcity += 0.18f;
            if (context.npcOutsideFactor < 0.5f) scarcity += 0.06f;

            var supplyDemand = 1f;
            if (context.npcOutsideFactor < 0.4f) supplyDemand += 0.08f;
            if (!context.shopOpen) supplyDemand += 0.08f;

            var economy = context.economy;
            economy.scarcityMultiplier = Clamp(scarcity, 0.85f, 1.35f);
            economy.supplyDemandMultiplier = Clamp(supplyDemand, 0.9f, 1.35f);

            context.foodPriceMultiplier = ResolveBaseFoodPriceMultiplier(economy);
        }

        public static float ResolveBaseFoodPriceMultiplier(EconomyState economy)
        {
            if (economy == null)
            {
                return 1f;
            }

            var result = economy.scarcityMultiplier
                         * economy.regionWealthMultiplier
                         * economy.reputationMultiplier
                         * economy.supplyDemandMultiplier;

            return Clamp(result, 0.75f, 2.25f);
        }

        public static float ResolveFinalItemPriceMultiplier(EconomyState economy, float itemRarityMultiplier)
        {
            var baseMultiplier = ResolveBaseFoodPriceMultiplier(economy);
            if (itemRarityMultiplier <= 0f) itemRarityMultiplier = 1f;
            return Clamp(baseMultiplier * itemRarityMultiplier, 0.75f, 3f);
        }

        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
