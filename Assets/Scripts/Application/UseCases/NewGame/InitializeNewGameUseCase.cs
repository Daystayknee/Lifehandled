using System;
using Lifehandled.Application.Ports;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;
using Lifehandled.Infrastructure.Persistence.DTO;
using Lifehandled.Application.UseCases.World;
using Lifehandled.Application.Content;
using System.Linq;

namespace Lifehandled.Application.UseCases.NewGame
{
    /// <summary>
    /// Minimal new game bootstrap:
    /// - create main character
    /// - optionally add one household member
    /// - save those records
    /// - assign player-controlled character
    /// - apply basic genetics stubs
    /// </summary>
    public class InitializeNewGameUseCase
    {
        private readonly INewGameSaveStore _saveStore;
        private readonly IFounderGeneticsFactory _founderGeneticsFactory;

        public InitializeNewGameUseCase(
            INewGameSaveStore saveStore,
            IFounderGeneticsFactory founderGeneticsFactory)
        {
            _saveStore = saveStore;
            _founderGeneticsFactory = founderGeneticsFactory;
        }

        public NewGameSetupResult Execute(NewGameSetupRequest request)
        {
            request ??= new NewGameSetupRequest();

            var playerCharacter = CreateCharacter(
                name: request.mainCharacterName,
                role: CharacterRole.PlayerMain,
                isPlayerControlled: true);

            var household = new HouseholdData
            {
                householdId = Guid.NewGuid().ToString("N"),
                householdName = "Starter Household",
                householdType = request.includeHouseholdMember ? HouseholdType.Roommates : HouseholdType.Solo
            };

            household.memberCharacterIds.Add(playerCharacter.characterId);
            playerCharacter.householdId = household.householdId;

            var envelope = new SaveGameEnvelope
            {
                saveVersion = 1,
                geneticSchemaVersion = 1,
                playerCharacterId = playerCharacter.characterId,
                vs01State = new Vs01RuntimeState
                {
                    currentDay = 1,
                    hourOfDay = 8f,
                    wallet = 20,
                    currentJob = JobType.PartTimeShopHelper,
                    dailyIncome = 12,
                    housingTier = HousingTier.Basic,
                    weeklyRentCost = 18,
                    regionWealthMultiplier = 1f,
                    reputationMultiplier = 1f,
                    isHomeOwned = true,
                    furnitureCount = 1,
                    homeComfort = 55f,
                    homeCleanliness = 65f,
                    storageCapacityBase = 6,
                    storageCapacityBonus = 0,
                    neighborhoodReputation = 50f,
                    currentLocationId = "home",
                    currentZone = ZoneType.Home,
                    socialReputation = 50f,
                    familyTension = 15f,
                    generationIndex = 0,
                    legacyFamilyName = "Founders",
                    legacyReputation = 50f,
                    skills =
                    {
                        new Vs01SkillState { skillId = "cooking", level = 1, xp = 0f },
                        new Vs01SkillState { skillId = "social", level = 1, xp = 0f },
                        new Vs01SkillState { skillId = "survival", level = 1, xp = 0f }
                    },
                    unlockedPerkIds = { },
                    collectibles = PrototypeWorldContentCatalog.CollectibleIds.ToList(),
                    rareEventsSeen = { },
                    generationCharacterIds = { playerCharacter.characterId },
                    zones = BuildZoneCatalogUseCase.CreateDefault().ConvertAll(z => new Vs01ZoneState
                    {
                        zoneType = z.zoneType,
                        zoneId = z.zoneId,
                        displayName = z.displayName,
                        npcPool = z.npcPool,
                        resources = z.resources,
                        events = z.events,
                        dangerLevel = z.dangerLevel
                    }),
                    inventory =
                    {
                        new Vs01ItemStack { itemId = "water_bottle", count = 1 },
                        new Vs01ItemStack { itemId = "stale_food", count = 1 }
                    },
                    npcs = PrototypeWorldContentCatalog.CreateNpcRoster(),
                }
            };

            envelope.characters.Add(playerCharacter);
            envelope.households.Add(household);

            var hasMember = false;

            if (request.includeHouseholdMember)
            {
                var member = CreateCharacter(
                    name: request.householdMemberName,
                    role: CharacterRole.HouseholdMember,
                    isPlayerControlled: false);

                member.householdId = household.householdId;
                household.memberCharacterIds.Add(member.characterId);
                envelope.characters.Add(member);

                var relationship = new RelationshipLink
                {
                    relationshipLinkId = Guid.NewGuid().ToString("N"),
                    characterAId = playerCharacter.characterId,
                    characterBId = member.characterId,
                    linkType = request.optionalMemberRelationshipType,
                    isHouseholdBond = true
                };

                envelope.relationshipLinks.Add(relationship);
                playerCharacter.relationshipLinkIds.Add(relationship.relationshipLinkId);
                member.relationshipLinkIds.Add(relationship.relationshipLinkId);

                hasMember = true;
            }

            _saveStore.Save(envelope);

            return new NewGameSetupResult
            {
                Envelope = envelope,
                PlayerCharacterId = playerCharacter.characterId,
                HouseholdId = household.householdId,
                HasOptionalHouseholdMember = hasMember
            };
        }

        private CharacterData CreateCharacter(string name, CharacterRole role, bool isPlayerControlled)
        {
            var characterId = Guid.NewGuid().ToString("N");
            var appearanceSeed = characterId.GetHashCode();

            var genetics = _founderGeneticsFactory.CreateFounderGenetics();
            var appearance = PrototypeWorldContentCatalog.CreateAppearanceFromSeed(appearanceSeed, isNpc: role == CharacterRole.Npc);
            PrototypeWorldContentCatalog.ApplyGeneticInfluenceToAppearance(appearance, genetics);

            return new CharacterData
            {
                characterId = characterId,
                displayName = string.IsNullOrWhiteSpace(name) ? "Character" : name,
                role = role,
                isPlayerControlled = isPlayerControlled,
                appearance = appearance,
                genetics = genetics
            };
        }
    }
}
