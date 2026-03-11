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
            var hour = context.hourOfDay;
            var month = context.monthOfYear;
            foreach (var def in PrototypeWorldContentCatalog.AnimalDefinitions.Where(a => resources.Contains(a.animalId) || resources.Contains($"animal:{a.animalId}")))
            {
                var encounterScore = def.encounterWeight + (survivalSkill * 0.03f) + (def.animalId == "fish" ? fishingSkill * 0.04f : 0f);

                // Batch 3: time/weather/seasonal migration-style modifiers.
                if (def.animalId == "fish")
                {
                    if (context.weather == WeatherType.Rain) encounterScore += 0.12f;
                    if (context.weather == WeatherType.Storm) encounterScore -= 0.1f;
                    if (hour < 6f || hour >= 20f) encounterScore -= 0.08f;
                    if (month is >= 4 and <= 8) encounterScore += 0.06f;
                }
                else if (def.animalId == "deer" || def.animalId == "rabbit")
                {
                    if (hour < 7f || hour >= 18f) encounterScore += 0.08f;
                    if (context.weather == WeatherType.Storm) encounterScore -= 0.15f;
                    if (month is >= 9 and <= 11) encounterScore += 0.07f; // migration season proxy
                }
                else if (def.animalId == "bird")
                {
                    if (hour >= 5f && hour <= 11f) encounterScore += 0.1f;
                    if (context.weather == WeatherType.Storm) encounterScore -= 0.18f;
                }

                if (encounterScore >= 0.75f)
                {
                    context.inventory.TryAddWithCapacity(def.outputItemId, 1, storageCapacity);
                    context.progression?.GainSkillXp(def.animalId == "fish" ? "fishing" : "survival", 3f);
                    effects.Append($"Encountered {def.animalId} (score {encounterScore:0.00}) and gained {def.outputItemId}. ");
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
