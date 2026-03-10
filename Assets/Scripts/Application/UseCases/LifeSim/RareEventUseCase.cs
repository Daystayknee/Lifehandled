using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class RareEventUseCase
    {
        public bool TryTrigger(GameSessionContext context, out string message)
        {
            message = string.Empty;
            if (context == null)
            {
                message = "No active session.";
                return false;
            }

            context.rareEventsSeen ??= new System.Collections.Generic.List<string>();
            var eventId = ResolveEventId(context);
            if (context.rareEventsSeen.Contains(eventId))
            {
                message = "No rare event right now.";
                return false;
            }

            context.rareEventsSeen.Add(eventId);
            ApplyRareEventRewards(context, eventId);
            message = $"Rare event: {eventId}. Rewards applied.";
            return true;
        }

        private static string ResolveEventId(GameSessionContext context)
        {
            var activeZone = context.zones?.Find(z => z.zoneType == context.currentZone);
            var dayEvent = activeZone?.events?.Find(e => e.StartsWith($"social_event_day:{context.currentDay}:"));
            if (!string.IsNullOrWhiteSpace(dayEvent) && dayEvent.EndsWith("emergency"))
            {
                return "heroic_response";
            }

            if (context.currentZone == Domain.Common.ZoneType.Forest) return "ancient_cache";
            if (context.currentZone == Domain.Common.ZoneType.Lake) return "golden_catch";
            if (context.currentZone == ZoneType.TownCenter) return "festival_windfall";
            return "unexpected_opportunity";
        }

        private static void ApplyRareEventRewards(GameSessionContext context, string eventId)
        {
            context.socialReputation = NeedsStatus.ClampToRange(context.socialReputation + 2f);
            context.wallet += 5;

            var progression = context.progression;
            if (progression == null)
            {
                return;
            }

            switch (eventId)
            {
                case "ancient_cache":
                    context.wallet += 4;
                    progression.GainSkillXp("survival", 8f);
                    break;
                case "golden_catch":
                    context.inventory.TryAddWithCapacity("cooked_fish", 1, context.home?.GetStorageCapacity() ?? 6);
                    progression.GainSkillXp("fishing", 10f);
                    break;
                case "festival_windfall":
                    context.socialReputation = NeedsStatus.ClampToRange(context.socialReputation + 3f);
                    progression.GainSkillXp("charisma", 9f);
                    break;
                case "heroic_response":
                    context.socialReputation = NeedsStatus.ClampToRange(context.socialReputation + 6f);
                    progression.GainSkillXp("medicine", 12f);
                    break;
                default:
                    progression.GainSkillXp("negotiation", 4f);
                    break;
            }
        }
    }
}
