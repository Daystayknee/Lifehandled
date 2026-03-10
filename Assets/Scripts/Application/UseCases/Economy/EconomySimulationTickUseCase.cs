using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;
using System.Linq;

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

            var activeZone = context.zones?.FirstOrDefault(z => z.zoneType == context.currentZone);
            var socialEvent = ResolveTodaySocialEvent(activeZone?.events, context.currentDay);
            if (socialEvent == SocialEventType.Emergency) scarcity += 0.12f;
            if (socialEvent == SocialEventType.Market || socialEvent == SocialEventType.NightMarket) scarcity -= 0.06f;
            if (socialEvent == SocialEventType.HarvestFair) scarcity -= 0.08f;
            if (socialEvent == SocialEventType.BlockParty) scarcity += 0.03f;

            var supplyDemand = 1f;
            if (context.npcOutsideFactor < 0.4f) supplyDemand += 0.08f;
            if (!context.shopOpen) supplyDemand += 0.08f;
            if (socialEvent == SocialEventType.Festival || socialEvent == SocialEventType.Party || socialEvent == SocialEventType.BlockParty) supplyDemand += 0.05f;
            if (socialEvent == SocialEventType.Protest) supplyDemand += 0.07f;
            if (socialEvent == SocialEventType.Concert || socialEvent == SocialEventType.SportsTournament) supplyDemand += 0.06f;
            if (socialEvent == SocialEventType.BookFair || socialEvent == SocialEventType.ArtShow) supplyDemand += 0.03f;
            if (socialEvent == SocialEventType.CharityDrive) supplyDemand += 0.02f;

            if (activeZone?.resources != null)
            {
                if (activeZone.resources.Contains("atm")) supplyDemand += 0.02f;
                if (activeZone.resources.Contains("vending_machine")) supplyDemand += 0.03f;
                if (activeZone.resources.Contains("shops")) supplyDemand -= 0.02f;
            }

            var economy = context.economy;
            economy.scarcityMultiplier = Clamp(scarcity, 0.85f, 1.35f);
            economy.supplyDemandMultiplier = Clamp(supplyDemand, 0.9f, 1.35f);

            economy.reputationMultiplier = Clamp(1f + ((context.socialReputation - 50f) * 0.002f), 0.85f, 1.25f);

            context.foodPriceMultiplier = ResolveBaseFoodPriceMultiplier(economy);
        }

        private static SocialEventType? ResolveTodaySocialEvent(System.Collections.Generic.List<string> events, int day)
        {
            if (events == null || events.Count == 0)
            {
                return null;
            }

            var prefix = $"social_event_day:{day}:";
            var raw = events.FirstOrDefault(e => e.StartsWith(prefix));
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            var tag = raw.Substring(prefix.Length);
            return tag switch
            {
                "party" => SocialEventType.Party,
                "festival" => SocialEventType.Festival,
                "market" => SocialEventType.Market,
                "wedding" => SocialEventType.Wedding,
                "funeral" => SocialEventType.Funeral,
                "protest" => SocialEventType.Protest,
                "emergency" => SocialEventType.Emergency,
                "concert" => SocialEventType.Concert,
                "sports_tournament" => SocialEventType.SportsTournament,
                "book_fair" => SocialEventType.BookFair,
                "art_show" => SocialEventType.ArtShow,
                "harvest_fair" => SocialEventType.HarvestFair,
                "science_expo" => SocialEventType.ScienceExpo,
                "charity_drive" => SocialEventType.CharityDrive,
                "block_party" => SocialEventType.BlockParty,
                "talent_show" => SocialEventType.TalentShow,
                "night_market" => SocialEventType.NightMarket,
                _ => null
            };
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
