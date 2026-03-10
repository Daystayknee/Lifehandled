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
