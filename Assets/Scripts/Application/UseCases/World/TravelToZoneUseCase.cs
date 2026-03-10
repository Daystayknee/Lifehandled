using System.Linq;
using System.Text;
using Lifehandled.Application.Content;
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

            var detail = ApplyZoneAssetAndWildlifeEffects(context, zone);

            message = $"Traveled to {zone.displayName}. Danger {zone.dangerLevel:0.00}. {detail}";
            return true;
        }

        private static string ApplyZoneAssetAndWildlifeEffects(GameSessionContext context, ZoneState zone)
        {
            var effects = new StringBuilder();
            var resources = zone.resources ?? new System.Collections.Generic.List<string>();
            var storageCapacity = context.home?.GetStorageCapacity() ?? 6;
            var needs = context.playerCharacter?.needsStatus;
            foreach (var resourceId in resources)
            {
                if (!PrototypeWorldContentCatalog.TryGetInteractableDefinition(resourceId, out var interactable))
                {
                    continue;
                }

                if (interactable.walletDelta < 0 && context.wallet < -interactable.walletDelta)
                {
                    continue;
                }

                context.wallet += interactable.walletDelta;
                if (needs != null)
                {
                    needs.stress = NeedsStatus.ClampToRange(needs.stress + interactable.stressDelta);
                    needs.energy = NeedsStatus.ClampToRange(needs.energy + interactable.energyDelta);
                }

                if (!string.IsNullOrWhiteSpace(interactable.inventoryItemId))
                {
                    context.inventory.TryAddWithCapacity(interactable.inventoryItemId, 1, storageCapacity);
                }

                effects.Append($"Used {interactable.interactableId}. ");
            }

            var fishingSkill = context.progression?.GetSkillLevel("fishing") ?? 1;
            var survivalSkill = context.progression?.GetSkillLevel("survival") ?? 1;
            foreach (var def in PrototypeWorldContentCatalog.AnimalDefinitions.Where(a => resources.Contains(a.animalId)))
            {
                var encounterScore = def.encounterWeight + (survivalSkill * 0.03f) + (def.animalId == "fish" ? fishingSkill * 0.04f : 0f);
                if (encounterScore >= 0.75f)
                {
                    context.inventory.TryAddWithCapacity(def.outputItemId, 1, storageCapacity);
                    context.progression?.GainSkillXp(def.animalId == "fish" ? "fishing" : "survival", 3f);
                    effects.Append($"Encountered {def.animalId} and gained {def.outputItemId}. ");
                    break;
                }
            }

            if (effects.Length == 0)
            {
                return "No special encounters.";
            }

            return effects.ToString().Trim();
        }
    }
}
