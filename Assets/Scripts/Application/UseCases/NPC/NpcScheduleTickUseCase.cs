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
                ApplyRoutineVariationByCalendarAndWeather(context, npc, schedule, hour);
                ApplyPreferenceDrivenActions(npc, schedule, hour);

                if (npc.profile.socialTraits.Contains(SocialTraitType.Introverted) && schedule.currentBlock == NpcScheduleBlock.Social)
                {
                    schedule.isAvailableForTalk = false;
                }

                TickNpcNeedsAndMood(npc, context.weather, todayEvent, context.isWeekend, context.activeHolidayId);
                ApplyHobbyTick(npc);
            }
        }

        private static void TickNpcNeedsAndMood(NpcRuntimeState npc, WeatherType weather, SocialEventType? todayEvent, bool isWeekend, string activeHolidayId)
        {
            var needs = npc.profile.needs;
            needs.hunger = Clamp(needs.hunger + 0.3f);
            needs.energy = Clamp(needs.energy - 0.2f);
            needs.social = Clamp(needs.social - 0.1f);

            var moodDelta = 0f;
            if (needs.energy < 30f) moodDelta -= 0.2f;
            if (needs.hunger > 70f) moodDelta -= 0.2f;
            if (weather == WeatherType.Storm) moodDelta -= 0.1f;
            if (isWeekend) moodDelta += 0.06f;
            if (!string.IsNullOrWhiteSpace(activeHolidayId) && activeHolidayId != "none") moodDelta += 0.1f;

            if (todayEvent == SocialEventType.Protest || todayEvent == SocialEventType.Emergency)
            {
                moodDelta -= 0.12f;
            }
            else if (todayEvent == SocialEventType.Party || todayEvent == SocialEventType.Festival || todayEvent == SocialEventType.BlockParty)
            {
                moodDelta += 0.12f;
            }

            if (npc.profile.personalityTraits.Contains(PersonalityTraitType.Irritable)) moodDelta -= 0.1f;
            if (npc.profile.personalityTraits.Contains(PersonalityTraitType.Kind)) moodDelta += 0.05f;
            if (npc.profile.emotionalTraits.Contains(EmotionalTraitType.Optimistic)) moodDelta += 0.05f;
            if (npc.profile.emotionalTraits.Contains(EmotionalTraitType.Anxious)) moodDelta -= 0.08f;

            npc.profile.mood = Clamp(npc.profile.mood + moodDelta);
        }

        private static void ApplyRoutineVariationByCalendarAndWeather(GameSessionContext context, NpcRuntimeState npc, NpcSchedule schedule, float hour)
        {
            if (context.weather == WeatherType.Storm)
            {
                // Severe weather keeps most NPCs indoors.
                if (schedule.currentBlock == NpcScheduleBlock.Errands || schedule.currentBlock == NpcScheduleBlock.Social)
                {
                    schedule.currentBlock = NpcScheduleBlock.Home;
                    schedule.isAvailableForTalk = false;
                }
            }

            if (context.isWeekend && hour >= 10f && hour < 21f && schedule.currentBlock == NpcScheduleBlock.Work)
            {
                // Weekend variation for non-essential workers.
                if (npc.profile.occupation?.category != OccupationCategoryType.Service)
                {
                    schedule.currentBlock = NpcScheduleBlock.Social;
                    schedule.isAvailableForTalk = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(context.activeHolidayId) && context.activeHolidayId != "none" && hour >= 11f && hour < 22f)
            {
                if (schedule.currentBlock != NpcScheduleBlock.Sleep)
                {
                    schedule.currentBlock = NpcScheduleBlock.Social;
                    schedule.isAvailableForTalk = true;
                }
            }
        }

        private static void ApplyPreferenceDrivenActions(NpcRuntimeState npc, NpcSchedule schedule, float hour)
        {
            var prefs = npc.profile.preferences;
            if (prefs == null)
            {
                return;
            }

            if (schedule.currentBlock == NpcScheduleBlock.Home && hour >= 18f && hour < 21f)
            {
                if (prefs.favoriteActivityIds.Contains("cooking") || prefs.hobbyIds.Contains("baking"))
                {
                    npc.profile.needs.hunger = Clamp(npc.profile.needs.hunger - 0.2f);
                    npc.profile.mood = Clamp(npc.profile.mood + 0.08f);
                }
            }

            if (schedule.currentBlock == NpcScheduleBlock.Social && prefs.hobbyIds.Contains("gaming"))
            {
                npc.profile.needs.social = Clamp(npc.profile.needs.social + 0.12f);
            }

            if (schedule.currentBlock == NpcScheduleBlock.Errands && prefs.favoriteActivityIds.Contains("market"))
            {
                schedule.isAvailableForTalk = true;
                npc.profile.mood = Clamp(npc.profile.mood + 0.06f);
            }

            if (prefs.hobbyIds.Contains("fishing") && hour >= 5f && hour < 9f)
            {
                // Early morning preference-driven routine variation.
                schedule.currentBlock = NpcScheduleBlock.Errands;
                schedule.isAvailableForTalk = true;
            }
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
                case SocialEventType.BlockParty:
                case SocialEventType.TalentShow:
                case SocialEventType.Concert:
                    if (hour >= 18f && hour < 22f)
                    {
                        schedule.currentBlock = NpcScheduleBlock.Social;
                        schedule.isAvailableForTalk = true;
                    }
                    break;

                case SocialEventType.Market:
                case SocialEventType.NightMarket:
                case SocialEventType.BookFair:
                case SocialEventType.ArtShow:
                    if (hour >= 10f && hour < 20f)
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
