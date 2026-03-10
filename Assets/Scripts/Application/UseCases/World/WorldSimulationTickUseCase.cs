using System;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;
using System.Linq;

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

            ApplyZoneContext(context);
            ApplySocialEventContext(context);
        }

        private static void ApplyZoneContext(GameSessionContext context)
        {
            if (context.zones == null || context.zones.Count == 0)
            {
                return;
            }

            var activeZone = context.zones.FirstOrDefault(z => z.zoneType == context.currentZone);
            if (activeZone == null)
            {
                return;
            }

            // Weather raises danger outdoors.
            var weatherDanger = context.weather == WeatherType.Storm ? 0.2f : (context.weather == WeatherType.Rain ? 0.1f : 0f);
            activeZone.dangerLevel = Clamp01(activeZone.dangerLevel + weatherDanger);
        }

        private static void ApplySocialEventContext(GameSessionContext context)
        {
            if (context.zones == null || context.zones.Count == 0)
            {
                return;
            }

            var activeZone = context.zones.FirstOrDefault(z => z.zoneType == context.currentZone);
            if (activeZone == null)
            {
                return;
            }

            activeZone.events ??= new System.Collections.Generic.List<string>();
            var todayEventTagPrefix = $"social_event_day:{context.currentDay}:";
            var existingTodayEvent = activeZone.events.FirstOrDefault(e => e.StartsWith(todayEventTagPrefix));
            if (!string.IsNullOrWhiteSpace(existingTodayEvent))
            {
                return;
            }

            activeZone.events.RemoveAll(e => e.StartsWith("social_event_day:"));

            var socialEvent = ResolveSocialEvent(context.currentDay);
            var npcMoodDelta = 0f;
            if (socialEvent == SocialEventType.Emergency)
            {
                context.npcOutsideFactor = Clamp01(context.npcOutsideFactor - 0.25f);
                activeZone.dangerLevel = Clamp01(activeZone.dangerLevel + 0.15f);
                npcMoodDelta = -2f;
            }
            else
            {
                context.npcOutsideFactor = Clamp01(context.npcOutsideFactor + 0.12f);
                context.socialReputation = Clamp100(context.socialReputation + 0.35f);
                npcMoodDelta = socialEvent == SocialEventType.Funeral ? -0.8f : 1.2f;
            }

            if (context.npcs != null && context.npcs.Count > 0)
            {
                foreach (var npc in context.npcs.Where(n => activeZone.npcPool == null || activeZone.npcPool.Count == 0 || activeZone.npcPool.Contains(n.profile.npcId)))
                {
                    npc.profile.mood = Clamp100(npc.profile.mood + npcMoodDelta);
                }
            }

            EnsureDailySocialEventAcrossZones(context, socialEvent);
        }

        private static void EnsureDailySocialEventAcrossZones(GameSessionContext context, SocialEventType socialEvent)
        {
            if (context.zones == null || context.zones.Count == 0)
            {
                return;
            }

            var eventTag = $"social_event_day:{context.currentDay}:{socialEvent.ToString().ToLowerInvariant()}";
            foreach (var zone in context.zones)
            {
                zone.events ??= new System.Collections.Generic.List<string>();
                zone.events.RemoveAll(e => e.StartsWith("social_event_day:"));
                zone.events.Add(eventTag);
            }
        }

        private static SocialEventType ResolveSocialEvent(int day)
        {
            var idx = (day - 1) % 7;
            return idx switch
            {
                0 => SocialEventType.Party,
                1 => SocialEventType.Festival,
                2 => SocialEventType.Market,
                3 => SocialEventType.Wedding,
                4 => SocialEventType.Funeral,
                5 => SocialEventType.Protest,
                _ => SocialEventType.Emergency
            };
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

        private static float Clamp100(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
