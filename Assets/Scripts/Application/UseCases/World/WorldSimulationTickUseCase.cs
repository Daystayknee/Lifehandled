using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.World
{
    /// <summary>
    /// Small, expandable world simulation tick:
    /// day/night, season, weather, NPC-outside factor, and shop hours.
    /// </summary>
    public class WorldSimulationTickUseCase
    {
        public void Execute(GameSessionContext context, float minutes)
        {
            if (context == null)
            {
                return;
            }

            AdvanceTime(context, minutes);
            context.season = ResolveSeason(context.currentDay);
            context.weather = ResolveWeather(context.currentDay, context.hourOfDay);

            context.isDaytime = context.hourOfDay >= 6f && context.hourOfDay < 19f;
            context.npcOutsideFactor = ResolveNpcOutsideFactor(context.hourOfDay, context.weather);
            context.shopOpen = ResolveShopOpen(context.hourOfDay, context.weather);
        }

        private static void AdvanceTime(GameSessionContext context, float minutes)
        {
            context.hourOfDay += minutes / 60f;

            while (context.hourOfDay >= 24f)
            {
                context.hourOfDay -= 24f;
                context.currentDay += 1;
                context.talkCountToday = 0;
            }
        }

        private static SeasonType ResolveSeason(int day)
        {
            // 30-day seasons on loop for prototype.
            var seasonIndex = ((day - 1) / 30) % 4;
            if (seasonIndex == 0) return SeasonType.Spring;
            if (seasonIndex == 1) return SeasonType.Summer;
            if (seasonIndex == 2) return SeasonType.Autumn;
            return SeasonType.Winter;
        }

        private static WeatherType ResolveWeather(int day, float hour)
        {
            // Deterministic lightweight weather pattern by day+time block.
            var block = (int)(hour / 6f);
            var pattern = (day + block) % 8;

            if (pattern == 6) return WeatherType.Rain;
            if (pattern == 7) return WeatherType.Storm;
            if (pattern == 3 || pattern == 4) return WeatherType.Cloudy;
            return WeatherType.Clear;
        }

        private static float ResolveNpcOutsideFactor(float hour, WeatherType weather)
        {
            float baseFactor;

            if (hour < 6f || hour >= 22f)
            {
                baseFactor = 0.2f;
            }
            else if (hour < 10f)
            {
                baseFactor = 0.6f;
            }
            else if (hour < 18f)
            {
                baseFactor = 0.9f;
            }
            else
            {
                baseFactor = 0.5f;
            }

            if (weather == WeatherType.Rain) baseFactor -= 0.2f;
            if (weather == WeatherType.Storm) baseFactor -= 0.5f;

            return Clamp01(baseFactor);
        }

        private static bool ResolveShopOpen(float hour, WeatherType weather)
        {
            var normalHours = hour >= 8f && hour < 20f;
            if (!normalHours)
            {
                return false;
            }

            // Severe weather can close shop for prototype world reaction.
            return weather != WeatherType.Storm;
        }

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }
    }
}
