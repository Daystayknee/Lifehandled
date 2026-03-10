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

                if (npc.profile.socialTraits.Contains(Domain.Common.SocialTraitType.Introverted) && schedule.currentBlock == NpcScheduleBlock.Social)
                {
                    schedule.isAvailableForTalk = false;
                }

                TickNpcNeedsAndMood(npc, context.weather);
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
    }
}
