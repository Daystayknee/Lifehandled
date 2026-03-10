using System.Linq;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.World
{
    public class TravelToZoneUseCase
    {
        public bool Execute(GameSessionContext context, ZoneType zoneType, out string message)
        {
            message = string.Empty;
            if (context == null)
            {
                message = "No active session.";
                return false;
            }

            context.zones ??= BuildZoneCatalogUseCase.CreateDefault();
            var zone = context.zones.FirstOrDefault(z => z.zoneType == zoneType);
            if (zone == null)
            {
                message = "Zone not found.";
                return false;
            }

            context.currentLocationId = zone.zoneId;
            context.currentZone = zoneType;

            // Danger is lightweight but meaningful.
            var needs = context.playerCharacter?.needsStatus;
            if (needs != null)
            {
                var stressShift = zone.dangerLevel * 4f;
                needs.stress = NeedsStatus.ClampToRange(needs.stress + stressShift);

                if (zone.dangerLevel > 0.5f)
                {
                    needs.energy = NeedsStatus.ClampToRange(needs.energy - (zone.dangerLevel * 3f));
                }
            }

            message = $"Traveled to {zone.displayName}. Danger {zone.dangerLevel:0.00}.";
            return true;
        }
    }
}
