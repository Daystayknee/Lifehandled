using System;
using System.Collections.Generic;
using System.Linq;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;
using Lifehandled.Application.Content;
using Lifehandled.Application.UseCases.World;

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
            if (envelope.vs01State.dayOfWeekIndex < 1 || envelope.vs01State.dayOfWeekIndex > 7)
            {
                envelope.vs01State.dayOfWeekIndex = 1;
            }
            if (string.IsNullOrWhiteSpace(envelope.vs01State.dayOfWeekName))
            {
                envelope.vs01State.dayOfWeekName = "Mon";
            }
            if (envelope.vs01State.dayOfMonth < 1 || envelope.vs01State.dayOfMonth > 30)
            {
                var safeDay = envelope.vs01State.currentDay < 1 ? 1 : envelope.vs01State.currentDay;
                envelope.vs01State.dayOfMonth = ((safeDay - 1) % 30) + 1;
            }
            if (envelope.vs01State.monthOfYear < 1 || envelope.vs01State.monthOfYear > 12)
            {
                var safeDay = envelope.vs01State.currentDay < 1 ? 1 : envelope.vs01State.currentDay;
                envelope.vs01State.monthOfYear = ((safeDay - 1) / 30) % 12 + 1;
            }
            if (string.IsNullOrWhiteSpace(envelope.vs01State.monthName))
            {
                envelope.vs01State.monthName = "Springrise";
            }
            if (string.IsNullOrWhiteSpace(envelope.vs01State.activeHolidayId))
            {
                envelope.vs01State.activeHolidayId = "none";
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

            foreach (var character in envelope.characters)
            {
                character.appearance ??= new AppearanceProfile();
                if (string.IsNullOrWhiteSpace(character.appearance.skinUndertone))
                {
                    character.appearance.skinUndertone = "neutral";
                }

                if (string.IsNullOrWhiteSpace(character.appearance.hairlineShapeId))
                {
                    character.appearance.hairlineShapeId = "rounded";
                }

                character.RecalculateAgeClassification();
            }

            if (envelope.vs01State.npcs.Count == 0)
            {
                envelope.vs01State.npcs = PrototypeWorldContentCatalog.CreateNpcRoster();
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

                if (string.IsNullOrWhiteSpace(npc.clothingStyleId))
                {
                    npc.clothingStyleId = "casual_basic";
                }

                if (string.IsNullOrWhiteSpace(npc.occupationJobId))
                {
                    npc.occupationJobId = "barista";
                }

                if (string.IsNullOrWhiteSpace(npc.occupationSchedulePreset))
                {
                    npc.occupationSchedulePreset = "day_shift";
                }

                if (npc.occupationDailyIncome < 0)
                {
                    npc.occupationDailyIncome = 10;
                }

                if (npc.occupationReputationImpact <= 0f)
                {
                    npc.occupationReputationImpact = 1f;
                }

                if (npc.occupationSocialCircleRadius <= 0f)
                {
                    npc.occupationSocialCircleRadius = 1f;
                }

                if (string.IsNullOrWhiteSpace(npc.upbringing))
                {
                    npc.upbringing = "ordinary_town";
                }

                if (string.IsNullOrWhiteSpace(npc.hometown))
                {
                    npc.hometown = "founders_town";
                }

                npc.pastRelationshipNotes ??= new List<string>();
                npc.previousJobs ??= new List<string>();
                npc.traumaTags ??= new List<string>();
                npc.favoriteFoodItemIds ??= new List<string>();
                npc.favoriteActivityIds ??= new List<string>();
                npc.hobbyIds ??= new List<string>();

                if (string.IsNullOrWhiteSpace(npc.preferredClothingStyleId))
                {
                    npc.preferredClothingStyleId = npc.clothingStyleId;
                }

                if (string.IsNullOrWhiteSpace(npc.musicTasteId))
                {
                    npc.musicTasteId = "pop";
                }

                if (npc.lifestyleTraits == null)
                {
                    npc.lifestyleTraits = new List<LifestyleTraitType>();
                }

                if (npc.emotionalTraits == null)
                {
                    npc.emotionalTraits = new List<EmotionalTraitType>();
                }

                if (npc.socialTraits == null)
                {
                    npc.socialTraits = new List<SocialTraitType>();
                }

                if (npc.survivalTraits == null)
                {
                    npc.survivalTraits = new List<SurvivalTraitType>();
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

            EnsureCoreSkills(envelope.vs01State.skills);

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

        private static void EnsureCoreSkills(List<Vs01SkillState> skills)
        {
            EnsureSkill(skills, "cooking");
            EnsureSkill(skills, "fishing");
            EnsureSkill(skills, "crafting");
            EnsureSkill(skills, "fitness");
            EnsureSkill(skills, "charisma");
            EnsureSkill(skills, "medicine");
            EnsureSkill(skills, "survival");
            EnsureSkill(skills, "negotiation");
        }

        private static void EnsureSkill(List<Vs01SkillState> skills, string skillId)
        {
            if (skills.Any(s => s.skillId == skillId))
            {
                return;
            }

            skills.Add(new Vs01SkillState
            {
                skillId = skillId,
                level = 1,
                xp = 0f
            });
        }

        private static List<Vs01ZoneState> CreateDefaultZones()
        {
            return BuildZoneCatalogUseCase.CreateDefault().Select(z => new Vs01ZoneState
            {
                zoneType = z.zoneType,
                zoneId = z.zoneId,
                displayName = z.displayName,
                npcPool = z.npcPool,
                resources = z.resources,
                events = z.events,
                dangerLevel = z.dangerLevel
            }).ToList();
        }

        private static CharacterData CreateDefaultPlayer()
        {
            var character = new CharacterData
            {
                characterId = Guid.NewGuid().ToString("N"),
                isPlayerControlled = true,
                role = CharacterRole.PlayerMain,
                displayName = "Player",
                ageYears = 18,
                appearance = new AppearanceProfile(),
                genetics = new GeneticProfile()
            };

            character.RecalculateAgeClassification();
            return character;
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
