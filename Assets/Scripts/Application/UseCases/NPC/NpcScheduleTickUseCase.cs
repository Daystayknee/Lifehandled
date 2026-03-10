using System.Linq;
using Lifehandled.Application.Session;
using Lifehandled.Application.Session.NPC;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.NPC
{
    public class NpcScheduleTickUseCase
    {
        public void Execute(GameSessionContext context)
        {
            if (context?.npcs == null)
            {
                return;
            }

            var activeZone = context.zones?.FirstOrDefault(z => z.zoneType == context.currentZone);
            var todayEvent = ResolveTodaySocialEvent(activeZone?.events, context.currentDay);

            foreach (NpcRuntimeState npc in context.npcs)
            {
                var hour = context.hourOfDay;
                var schedule = npc.profile.schedule;
                var occupationPreset = npc.profile.occupation?.schedulePreset ?? "day_shift";

                if (hour < 6f || hour >= 22f)
                {
                    schedule.currentBlock = NpcScheduleBlock.Sleep;
                    schedule.isAvailableForTalk = false;
                }
                else if (occupationPreset == "night_shift" && (hour >= 18f || hour < 2f))
                {
                    schedule.currentBlock = NpcScheduleBlock.Work;
                    schedule.isAvailableForTalk = false;
                }
                else if (hour < 9f)
                {
                    schedule.currentBlock = NpcScheduleBlock.Commute;
                    schedule.isAvailableForTalk = true;
                }
                else if (hour < 17f)
                {
                    schedule.currentBlock = NpcScheduleBlock.Work;
                    schedule.isAvailableForTalk = context.npcOutsideFactor > 0.3f;
                }
                else if (hour < 20f)
                {
                    schedule.currentBlock = NpcScheduleBlock.Errands;
                    schedule.isAvailableForTalk = true;
                }
                else
                {
                    schedule.currentBlock = NpcScheduleBlock.Home;
                    schedule.isAvailableForTalk = true;
                }

                ApplySocialEventScheduleModifiers(schedule, todayEvent, hour);

                if (npc.profile.socialTraits.Contains(Domain.Common.SocialTraitType.Introverted) && schedule.currentBlock == NpcScheduleBlock.Social)
                {
                    schedule.isAvailableForTalk = false;
                }

                TickNpcNeedsAndMood(npc, context.weather);
                ApplyHobbyTick(npc);
            }
        }

        private static void TickNpcNeedsAndMood(NpcRuntimeState npc, WeatherType weather)
        {
            var needs = npc.profile.needs;
            needs.hunger = Clamp(needs.hunger + 0.3f);
            needs.energy = Clamp(needs.energy - 0.2f);
            needs.social = Clamp(needs.social - 0.1f);

            var moodDelta = 0f;
            if (needs.energy < 30f) moodDelta -= 0.2f;
            if (needs.hunger > 70f) moodDelta -= 0.2f;
            if (weather == WeatherType.Storm) moodDelta -= 0.1f;

            if (npc.profile.personalityTraits.Contains(PersonalityTraitType.Irritable)) moodDelta -= 0.1f;
            if (npc.profile.personalityTraits.Contains(PersonalityTraitType.Kind)) moodDelta += 0.05f;
            if (npc.profile.emotionalTraits.Contains(Domain.Common.EmotionalTraitType.Optimistic)) moodDelta += 0.05f;
            if (npc.profile.emotionalTraits.Contains(Domain.Common.EmotionalTraitType.Anxious)) moodDelta -= 0.08f;

            npc.profile.mood = Clamp(npc.profile.mood + moodDelta);
        }

        private static float Clamp(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }

        private static void ApplyHobbyTick(NpcRuntimeState npc)
        {
            var hobbies = npc.profile.preferences?.hobbyIds;
            if (hobbies == null || hobbies.Count == 0)
            {
                return;
            }

            if (hobbies.Contains("working_out"))
            {
                npc.profile.needs.energy = Clamp(npc.profile.needs.energy - 0.05f);
                npc.profile.mood = Clamp(npc.profile.mood + 0.08f);
            }

            if (hobbies.Contains("reading") || hobbies.Contains("painting") || hobbies.Contains("gaming"))
            {
                npc.profile.mood = Clamp(npc.profile.mood + 0.05f);
            }

            if (hobbies.Contains("fishing") || hobbies.Contains("gardening") || hobbies.Contains("cooking"))
            {
                npc.profile.needs.social = Clamp(npc.profile.needs.social + 0.04f);
            }
        }

        private static SocialEventType? ResolveTodaySocialEvent(System.Collections.Generic.List<string> zoneEvents, int currentDay)
        {
            if (zoneEvents == null || zoneEvents.Count == 0)
            {
                return null;
            }

            var prefix = $"social_event_day:{currentDay}:";
            var raw = zoneEvents.FirstOrDefault(e => e.StartsWith(prefix));
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            var value = raw.Substring(prefix.Length);
            return value switch
            {
                "party" => SocialEventType.Party,
                "festival" => SocialEventType.Festival,
                "market" => SocialEventType.Market,
                "wedding" => SocialEventType.Wedding,
                "funeral" => SocialEventType.Funeral,
                "protest" => SocialEventType.Protest,
                "emergency" => SocialEventType.Emergency,
                _ => null
            };
        }

        private static void ApplySocialEventScheduleModifiers(NpcSchedule schedule, SocialEventType? socialEvent, float hour)
        {
            if (!socialEvent.HasValue)
            {
                return;
            }

            switch (socialEvent.Value)
            {
                case SocialEventType.Party:
                case SocialEventType.Festival:
                case SocialEventType.Wedding:
                    if (hour >= 18f && hour < 22f)
                    {
                        schedule.currentBlock = NpcScheduleBlock.Social;
                        schedule.isAvailableForTalk = true;
                    }
                    break;

                case SocialEventType.Market:
                    if (hour >= 10f && hour < 16f)
                    {
                        schedule.currentBlock = NpcScheduleBlock.Errands;
                        schedule.isAvailableForTalk = true;
                    }
                    break;

                case SocialEventType.Protest:
                    if (hour >= 12f && hour < 18f)
                    {
                        schedule.currentBlock = NpcScheduleBlock.Social;
                        schedule.isAvailableForTalk = false;
                    }
                    break;

                case SocialEventType.Emergency:
                    schedule.currentBlock = NpcScheduleBlock.Home;
                    schedule.isAvailableForTalk = false;
                    break;

                case SocialEventType.Funeral:
                    if (hour >= 9f && hour < 14f)
                    {
                        schedule.currentBlock = NpcScheduleBlock.Social;
                        schedule.isAvailableForTalk = false;
                    }
                    break;
            }
        }
    }
}
