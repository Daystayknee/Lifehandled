using System;
using System.Collections.Generic;
using System.Linq;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;

namespace Lifehandled.Infrastructure.Persistence.DTO
{
    /// <summary>
    /// Batch 2 compatibility helpers for defaulting and integrity checks.
    /// Keeps old/partial save payloads playable with minimal fallbacks.
    /// </summary>
    public static class SaveGameEnvelopeCompatibility
    {
        public static CompatibilityResult ApplyDefaultsAndValidate(SaveGameEnvelope envelope)
        {
            var warnings = new List<string>();

            // Null-safe container initialization for old/partial payloads.
            envelope.characters ??= new List<CharacterData>();
            envelope.households ??= new List<HouseholdData>();
            envelope.relationshipLinks ??= new List<RelationshipLink>();
            envelope.vs01State ??= new Vs01RuntimeState();
            envelope.vs01State.inventory ??= new List<Vs01ItemStack>();
            envelope.vs01State.npcs ??= new List<Vs01NpcState>();
            envelope.vs01State.zones ??= new List<Vs01ZoneState>();
            envelope.vs01State.skills ??= new List<Vs01SkillState>();
            envelope.vs01State.unlockedPerkIds ??= new List<string>();
            envelope.vs01State.collectibles ??= new List<string>();
            envelope.vs01State.rareEventsSeen ??= new List<string>();
            envelope.vs01State.generationCharacterIds ??= new List<string>();
            if (string.IsNullOrWhiteSpace(envelope.vs01State.currentLocationId))
            {
                envelope.vs01State.currentLocationId = "home";
            }
            if (!System.Enum.IsDefined(typeof(ZoneType), envelope.vs01State.currentZone))
            {
                envelope.vs01State.currentZone = ZoneType.Home;
            }
            if (envelope.vs01State.hourOfDay < 0f || envelope.vs01State.hourOfDay >= 24f)
            {
                envelope.vs01State.hourOfDay = 8f;
            }
            if (envelope.vs01State.foodPriceMultiplier <= 0f)
            {
                envelope.vs01State.foodPriceMultiplier = 1f;
            }
            if (envelope.vs01State.dailyIncome < 0)
            {
                envelope.vs01State.dailyIncome = 12;
            }
            if (envelope.vs01State.weeklyRentCost < 0)
            {
                envelope.vs01State.weeklyRentCost = 18;
            }
            if (envelope.vs01State.scarcityMultiplier <= 0f)
            {
                envelope.vs01State.scarcityMultiplier = 1f;
            }
            if (envelope.vs01State.regionWealthMultiplier <= 0f)
            {
                envelope.vs01State.regionWealthMultiplier = 1f;
            }
            if (envelope.vs01State.reputationMultiplier <= 0f)
            {
                envelope.vs01State.reputationMultiplier = 1f;
            }
            if (envelope.vs01State.supplyDemandMultiplier <= 0f)
            {
                envelope.vs01State.supplyDemandMultiplier = 1f;
            }
            if (envelope.vs01State.furnitureCount < 0)
            {
                envelope.vs01State.furnitureCount = 0;
            }
            if (envelope.vs01State.homeComfort <= 0f)
            {
                envelope.vs01State.homeComfort = 55f;
            }
            if (envelope.vs01State.homeCleanliness <= 0f)
            {
                envelope.vs01State.homeCleanliness = 65f;
            }
            if (envelope.vs01State.storageCapacityBase <= 0)
            {
                envelope.vs01State.storageCapacityBase = 6;
            }
            if (envelope.vs01State.storageCapacityBonus < 0)
            {
                envelope.vs01State.storageCapacityBonus = 0;
            }
            if (envelope.vs01State.neighborhoodReputation < 0f || envelope.vs01State.neighborhoodReputation > 100f)
            {
                envelope.vs01State.neighborhoodReputation = 50f;
            }
            if (envelope.vs01State.npcOutsideFactor < 0f)
            {
                envelope.vs01State.npcOutsideFactor = 0.8f;
            }
            if (envelope.vs01State.socialReputation < 0f || envelope.vs01State.socialReputation > 100f)
            {
                envelope.vs01State.socialReputation = 50f;
            }
            if (envelope.vs01State.familyTension < 0f || envelope.vs01State.familyTension > 100f)
            {
                envelope.vs01State.familyTension = 15f;
            }
            if (envelope.vs01State.legacyReputation < 0f || envelope.vs01State.legacyReputation > 100f)
            {
                envelope.vs01State.legacyReputation = 50f;
            }
            if (string.IsNullOrWhiteSpace(envelope.vs01State.legacyFamilyName))
            {
                envelope.vs01State.legacyFamilyName = "Founders";
            }

            if (envelope.vs01State.npcs.Count == 0)
            {
                envelope.vs01State.npcs.Add(new Vs01NpcState
                {
                    npcId = "npc_vendor_01",
                    displayName = "Mara",
                    personalityTraits = new List<PersonalityTraitType> { PersonalityTraitType.Practical },
                    friendship = 30f,
                    trust = 30f,
                    respect = 35f
                });
            }

            foreach (var npc in envelope.vs01State.npcs)
            {
                if (npc.socialReputationOfPlayer < 0f || npc.socialReputationOfPlayer > 100f)
                {
                    npc.socialReputationOfPlayer = 50f;
                }

                if (npc.knownSecretsCount < 0)
                {
                    npc.knownSecretsCount = 0;
                }

                npc.gossipHeat = Clamp01Range(npc.gossipHeat);
                npc.rumorBelief = Clamp01Range(npc.rumorBelief);
                npc.rivalryWithPlayer = Clamp01Range(npc.rivalryWithPlayer);
                npc.romanceInterest = Clamp01Range(npc.romanceInterest);
                npc.familyTensionWithPlayer = Clamp01Range(npc.familyTensionWithPlayer);
            }

            if (envelope.vs01State.zones.Count == 0)
            {
                envelope.vs01State.zones = CreateDefaultZones();
            }

            foreach (var skill in envelope.vs01State.skills)
            {
                if (skill.level <= 0)
                {
                    skill.level = 1;
                }

                if (skill.xp < 0f)
                {
                    skill.xp = 0f;
                }
            }

            if (envelope.geneticSchemaVersion <= 0)
            {
                envelope.geneticSchemaVersion = 1;
                warnings.Add("Missing genetic schema version; defaulted to v1.");
            }

            EnsurePlayerCharacter(envelope, warnings);
            EnsureSoloHouseholdFallback(envelope, warnings);
            ValidateRelationships(envelope, warnings);

            return new CompatibilityResult(warnings);
        }

        private static void EnsurePlayerCharacter(SaveGameEnvelope envelope, List<string> warnings)
        {
            CharacterData player = null;

            if (!string.IsNullOrWhiteSpace(envelope.playerCharacterId))
            {
                player = envelope.characters.FirstOrDefault(c => c.characterId == envelope.playerCharacterId);
            }

            if (player != null)
            {
                return;
            }

            // Prefer existing player-marked character when possible.
            player = envelope.characters.FirstOrDefault(c => c.isPlayerControlled)
                     ?? envelope.characters.FirstOrDefault();

            if (player == null)
            {
                player = CreateDefaultPlayer();
                envelope.characters.Add(player);
                warnings.Add("No characters found; created fallback player character.");
            }

            player.isPlayerControlled = true;
            player.role = CharacterRole.PlayerMain;
            envelope.playerCharacterId = player.characterId;
            warnings.Add("Player character ID missing/invalid; reassigned to fallback character.");
        }

        private static void EnsureSoloHouseholdFallback(SaveGameEnvelope envelope, List<string> warnings)
        {
            var characterIds = new HashSet<string>(envelope.characters.Select(c => c.characterId));

            // Remove unknown member references in existing households.
            foreach (var household in envelope.households)
            {
                household.memberCharacterIds ??= new List<string>();
                household.memberCharacterIds = household.memberCharacterIds
                    .Where(characterIds.Contains)
                    .Distinct()
                    .ToList();
            }

            // Ensure player belongs to at least one valid household.
            var playerInHousehold = envelope.households.Any(h =>
                h.memberCharacterIds.Contains(envelope.playerCharacterId));

            if (playerInHousehold)
            {
                return;
            }

            var soloHousehold = new HouseholdData
            {
                householdId = Guid.NewGuid().ToString("N"),
                householdName = "Solo Household",
                householdType = HouseholdType.Solo,
                homeLocationId = "starter_home",
                memberCharacterIds = new List<string> { envelope.playerCharacterId }
            };

            envelope.households.Add(soloHousehold);

            var player = envelope.characters.First(c => c.characterId == envelope.playerCharacterId);
            player.householdId = soloHousehold.householdId;

            warnings.Add("Player had no valid household; created solo household fallback.");
        }

        private static void ValidateRelationships(SaveGameEnvelope envelope, List<string> warnings)
        {
            var characterIds = new HashSet<string>(envelope.characters.Select(c => c.characterId));

            var originalCount = envelope.relationshipLinks.Count;
            envelope.relationshipLinks = envelope.relationshipLinks
                .Where(r => characterIds.Contains(r.characterAId) && characterIds.Contains(r.characterBId))
                .ToList();

            var removed = originalCount - envelope.relationshipLinks.Count;
            if (removed > 0)
            {
                warnings.Add($"Removed {removed} invalid relationship link(s) with missing character references.");
            }
        }

        private static float Clamp01Range(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }

        private static List<Vs01ZoneState> CreateDefaultZones()
        {
            return new List<Vs01ZoneState>
            {
                new() { zoneType = ZoneType.Home, zoneId = "home", displayName = "Home", npcPool = new List<string> { "npc_neighbor_01" }, resources = new List<string> { "bed", "kitchen", "storage" }, events = new List<string> { "rest", "cleaning" }, dangerLevel = 0.05f },
                new() { zoneType = ZoneType.Town, zoneId = "town", displayName = "Town", npcPool = new List<string> { "npc_vendor_01", "npc_neighbor_01" }, resources = new List<string> { "social_hub" }, events = new List<string> { "gossip", "street_event" }, dangerLevel = 0.2f },
                new() { zoneType = ZoneType.Store, zoneId = "store", displayName = "Store", npcPool = new List<string> { "npc_vendor_01" }, resources = new List<string> { "water_bottle", "stale_food" }, events = new List<string> { "trade", "price_change" }, dangerLevel = 0.1f },
                new() { zoneType = ZoneType.Forest, zoneId = "forest", displayName = "Forest", resources = new List<string> { "wood", "herbs" }, events = new List<string> { "gathering", "wild_encounter" }, dangerLevel = 0.65f },
                new() { zoneType = ZoneType.Lake, zoneId = "lake", displayName = "Lake", resources = new List<string> { "fish", "water" }, events = new List<string> { "fishing", "rain_event" }, dangerLevel = 0.35f },
                new() { zoneType = ZoneType.Clinic, zoneId = "clinic", displayName = "Clinic", resources = new List<string> { "medicine" }, events = new List<string> { "treatment" }, dangerLevel = 0.05f },
                new() { zoneType = ZoneType.Workplace, zoneId = "workplace", displayName = "Workplace", npcPool = new List<string> { "npc_vendor_01" }, resources = new List<string> { "income" }, events = new List<string> { "shift", "work_conflict" }, dangerLevel = 0.25f }
            };
        }

        private static CharacterData CreateDefaultPlayer()
        {
            return new CharacterData
            {
                characterId = Guid.NewGuid().ToString("N"),
                isPlayerControlled = true,
                role = CharacterRole.PlayerMain,
                displayName = "Player",
                ageYears = 18,
                appearance = new AppearanceProfile(),
                genetics = new GeneticProfile()
            };
        }
    }

    [Serializable]
    public class CompatibilityResult
    {
        public CompatibilityResult(List<string> warnings)
        {
            Warnings = warnings ?? new List<string>();
        }

        public List<string> Warnings { get; }
    }
}
