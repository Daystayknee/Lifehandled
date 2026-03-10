using System;
using Lifehandled.Application.Ports;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.Household;
using Lifehandled.Domain.Social;
using Lifehandled.Infrastructure.Persistence.DTO;

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
                    inventory =
                    {
                        new Vs01ItemStack { itemId = "water_bottle", count = 1 },
                        new Vs01ItemStack { itemId = "stale_food", count = 1 }
                    },
                    npcs =
                    {
                        new Vs01NpcState
                        {
                            npcId = "npc_vendor_01",
                            displayName = "Mara",
                            personalityTraits = { PersonalityTraitType.Practical, PersonalityTraitType.Reserved },
                            friendship = 35f,
                            trust = 30f,
                            respect = 40f
                        }
                    },
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
            return new CharacterData
            {
                characterId = Guid.NewGuid().ToString("N"),
                displayName = string.IsNullOrWhiteSpace(name) ? "Character" : name,
                role = role,
                isPlayerControlled = isPlayerControlled,
                appearance = new AppearanceProfile(),
                genetics = _founderGeneticsFactory.CreateFounderGenetics()
            };
        }
    }
}
