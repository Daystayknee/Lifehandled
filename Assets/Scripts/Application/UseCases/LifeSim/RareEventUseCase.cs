using Lifehandled.Application.Session;

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
            context.socialReputation = NeedsStatus.ClampToRange(context.socialReputation + 2f);
            context.wallet += 5;
            message = $"Rare event: {eventId}. Reputation and wallet increased.";
            return true;
        }

        private static string ResolveEventId(GameSessionContext context)
        {
            if (context.currentZone == Domain.Common.ZoneType.Forest) return "ancient_cache";
            if (context.currentZone == Domain.Common.ZoneType.Lake) return "golden_catch";
            return "unexpected_opportunity";
        }
    }
}
