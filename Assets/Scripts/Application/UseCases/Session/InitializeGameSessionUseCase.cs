using System.Linq;
using Lifehandled.Application.Ports;
using Lifehandled.Application.Session;
using Lifehandled.Application.Session.NPC;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.NPC;
using Lifehandled.Domain.Social;
using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.UseCases.Session
{
    /// <summary>
    /// Builds session context from save data, including VS01 runtime loop state.
    /// </summary>
    public class InitializeGameSessionUseCase
    {
        private readonly IGeneticModifierProvider _geneticModifierProvider;

        public InitializeGameSessionUseCase(IGeneticModifierProvider geneticModifierProvider)
        {
            _geneticModifierProvider = geneticModifierProvider;
        }

        public SessionInitializationResult Execute(SaveGameEnvelope envelope)
        {
            envelope ??= new SaveGameEnvelope();
            var compatibility = SaveGameEnvelopeCompatibility.ApplyDefaultsAndValidate(envelope);

            var playerData = envelope.characters.First(c => c.characterId == envelope.playerCharacterId);
            var activeHousehold = envelope.households
                .FirstOrDefault(h => h.memberCharacterIds.Contains(playerData.characterId));

            var context = new GameSessionContext
            {
                playerCharacterId = playerData.characterId,
                activeHouseholdId = activeHousehold?.householdId ?? string.Empty,
                currentDay = envelope.vs01State.currentDay,
                hourOfDay = envelope.vs01State.hourOfDay,
                wallet = envelope.vs01State.wallet,
                currentLocationId = envelope.vs01State.currentLocationId,
                currentZone = envelope.vs01State.currentZone,
                talkCountToday = envelope.vs01State.talkCountToday,
                socialReputation = envelope.vs01State.socialReputation,
                familyTension = envelope.vs01State.familyTension,
                season = envelope.vs01State.season,
                weather = envelope.vs01State.weather,
                isDaytime = envelope.vs01State.isDaytime,
                shopOpen = envelope.vs01State.shopOpen,
                npcOutsideFactor = envelope.vs01State.npcOutsideFactor,
                foodPriceMultiplier = envelope.vs01State.foodPriceMultiplier,
                dayOfWeekIndex = envelope.vs01State.dayOfWeekIndex,
                dayOfWeekName = envelope.vs01State.dayOfWeekName,
                dayOfMonth = envelope.vs01State.dayOfMonth,
                monthOfYear = envelope.vs01State.monthOfYear,
                monthName = envelope.vs01State.monthName,
                isWeekend = envelope.vs01State.isWeekend,
                activeHolidayId = envelope.vs01State.activeHolidayId,
                economy = new EconomyState
                {
                    currentJob = envelope.vs01State.currentJob,
                    dailyIncome = envelope.vs01State.dailyIncome,
                    housingTier = envelope.vs01State.housingTier,
                    weeklyRentCost = envelope.vs01State.weeklyRentCost,
                    scarcityMultiplier = envelope.vs01State.scarcityMultiplier,
                    regionWealthMultiplier = envelope.vs01State.regionWealthMultiplier,
                    reputationMultiplier = envelope.vs01State.reputationMultiplier,
                    supplyDemandMultiplier = envelope.vs01State.supplyDemandMultiplier,
                    lastIncomePaidDay = envelope.vs01State.lastIncomePaidDay,
                    lastRentPaidDay = envelope.vs01State.lastRentPaidDay
                },
                home = new HomeLifeState
                {
                    isHomeOwned = envelope.vs01State.isHomeOwned,
                    furnitureCount = envelope.vs01State.furnitureCount,
                    homeComfort = envelope.vs01State.homeComfort,
                    cleanliness = envelope.vs01State.homeCleanliness,
                    storageCapacityBase = envelope.vs01State.storageCapacityBase,
                    storageCapacityBonus = envelope.vs01State.storageCapacityBonus,
                    neighborhoodReputation = envelope.vs01State.neighborhoodReputation
                },
                zones = LoadZones(envelope.vs01State),
                progression = LoadProgression(envelope.vs01State),
                collectibles = envelope.vs01State.collectibles,
                rareEventsSeen = envelope.vs01State.rareEventsSeen,
                familyLineage = new FamilyLineageState
                {
                    generationIndex = envelope.vs01State.generationIndex,
                    legacyFamilyName = envelope.vs01State.legacyFamilyName,
                    legacyReputation = envelope.vs01State.legacyReputation,
                    generationCharacterIds = envelope.vs01State.generationCharacterIds
                },
                inventory = LoadInventory(envelope.vs01State),
                npcs = LoadNpcs(envelope.vs01State),
                playerCharacter = CreateRuntimeCharacter(playerData, false, envelope.vs01State)
            };

            if (activeHousehold != null)
            {
                var members = envelope.characters
                    .Where(c => c.characterId != playerData.characterId &&
                                activeHousehold.memberCharacterIds.Contains(c.characterId));

                foreach (var member in members)
                {
                    context.householdMembers.Add(CreateRuntimeCharacter(member, true, envelope.vs01State));
                }
            }

            return new SessionInitializationResult
            {
                Context = context,
                CompatibilityWarnings = compatibility.Warnings
            };
        }

        private static InventoryState LoadInventory(Vs01RuntimeState state)
        {
            var inventory = new InventoryState();
            foreach (var stack in state.inventory)
            {
                if (stack.count > 0)
                {
                    inventory.Add(stack.itemId, stack.count);
                }
            }

            return inventory;
        }

        private static System.Collections.Generic.List<ZoneState> LoadZones(Vs01RuntimeState state)
        {
            if (state.zones == null || state.zones.Count == 0)
            {
                return World.BuildZoneCatalogUseCase.CreateDefault();
            }

            return state.zones.Select(z => new ZoneState
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

        private static ProgressionState LoadProgression(Vs01RuntimeState state)
        {
            var progression = new ProgressionState
            {
                skills = state.skills.Select(s => new SkillProgress
                {
                    skillId = s.skillId,
                    level = s.level,
                    xp = s.xp
                }).ToList(),
                unlockedPerkIds = state.unlockedPerkIds
            };

            return progression;
        }

        private static System.Collections.Generic.List<NpcRuntimeState> LoadNpcs(Vs01RuntimeState state)
        {
            var npcs = new System.Collections.Generic.List<NpcRuntimeState>();

            foreach (var saved in state.npcs)
            {
                npcs.Add(new NpcRuntimeState
                {
                    profile = new NpcProfile
                    {
                        npcId = saved.npcId,
                        displayName = saved.displayName,
                        archetype = saved.archetype,
                        dialogueStyle = saved.dialogueStyle,
                        voiceType = saved.voiceType,
                        speechStyle = saved.speechStyle,
                        clothingStyleId = saved.clothingStyleId,
                        occupation = new OccupationProfile
                        {
                            category = saved.occupationCategory,
                            jobId = saved.occupationJobId,
                            schedulePreset = saved.occupationSchedulePreset,
                            dailyIncome = saved.occupationDailyIncome,
                            reputationImpact = saved.occupationReputationImpact,
                            socialCircleRadius = saved.occupationSocialCircleRadius
                        },
                        lifeHistory = new LifeHistoryProfile
                        {
                            upbringing = saved.upbringing,
                            educationLevel = saved.educationLevel,
                            hometown = saved.hometown,
                            pastRelationshipNotes = saved.pastRelationshipNotes ?? new System.Collections.Generic.List<string>(),
                            previousJobs = saved.previousJobs ?? new System.Collections.Generic.List<string>(),
                            traumaTags = saved.traumaTags ?? new System.Collections.Generic.List<string>()
                        },
                        preferences = new LifestylePreferences
                        {
                            favoriteFoodItemIds = saved.favoriteFoodItemIds ?? new System.Collections.Generic.List<string>(),
                            favoriteActivityIds = saved.favoriteActivityIds ?? new System.Collections.Generic.List<string>(),
                            preferredClothingStyleId = saved.preferredClothingStyleId,
                            musicTasteId = saved.musicTasteId,
                            hobbyIds = saved.hobbyIds ?? new System.Collections.Generic.List<string>()
                        },
                        personalityTraits = saved.personalityTraits ?? new System.Collections.Generic.List<PersonalityTraitType>(),
                        emotionalTraits = saved.emotionalTraits ?? new System.Collections.Generic.List<EmotionalTraitType>(),
                        socialTraits = saved.socialTraits ?? new System.Collections.Generic.List<SocialTraitType>(),
                        lifestyleTraits = saved.lifestyleTraits ?? new System.Collections.Generic.List<LifestyleTraitType>(),
                        survivalTraits = saved.survivalTraits ?? new System.Collections.Generic.List<SurvivalTraitType>(),
                        needs = new NpcNeeds
                        {
                            hunger = saved.hunger,
                            energy = saved.energy,
                            social = saved.social
                        },
                        mood = saved.mood,
                        schedule = new NpcSchedule
                        {
                            currentBlock = saved.currentScheduleBlock,
                            isAvailableForTalk = saved.isAvailableForTalk
                        },
                        relationshipToPlayer = new RelationshipStats
                        {
                            friendship = saved.friendship,
                            trust = saved.trust,
                            attraction = saved.attraction,
                            respect = saved.respect,
                            fear = saved.fear,
                            resentment = saved.resentment
                        },
                        drama = new NpcDramaState
                        {
                            knownSecretsCount = saved.knownSecretsCount,
                            gossipHeat = saved.gossipHeat,
                            rumorBelief = saved.rumorBelief,
                            rivalryWithPlayer = saved.rivalryWithPlayer,
                            romanceInterest = saved.romanceInterest,
                            familyTensionWithPlayer = saved.familyTensionWithPlayer,
                            socialReputationOfPlayer = saved.socialReputationOfPlayer
                        },
                        memory = saved.memory.Select(m => new NpcMemoryEntry
                        {
                            day = m.day,
                            hour = m.hour,
                            interactionType = m.interactionType,
                            outcome = m.outcome
                        }).ToList()
                    }
                });
            }

            return npcs;
        }

        private RuntimeCharacterState CreateRuntimeCharacter(CharacterData character, bool lightSim, Vs01RuntimeState state)
        {
            var modifiers = _geneticModifierProvider.BuildModifiers(character.genetics);
            var defaults = NeedsStatus.CreateDefault(modifiers);

            return new RuntimeCharacterState
            {
                data = character,
                geneticModifiers = modifiers,
                needsStatus = new NeedsStatus
                {
                    hunger = state.hunger >= 0 ? state.hunger : defaults.hunger,
                    thirst = state.thirst >= 0 ? state.thirst : defaults.thirst,
                    energy = state.energy >= 0 ? state.energy : defaults.energy,
                    warmth = state.warmth >= 0 ? state.warmth : defaults.warmth,
                    hygiene = state.hygiene >= 0 ? state.hygiene : defaults.hygiene,
                    stress = state.stress >= 0 ? state.stress : defaults.stress,
                    mood = state.mood >= 0 ? state.mood : defaults.mood,
                    illnessRisk = state.illnessRisk >= 0 ? state.illnessRisk : defaults.illnessRisk,
                    wetness = state.wetness >= 0 ? state.wetness : defaults.wetness
                },
                isLightSimulatedHouseholdMember = lightSim
            };
        }
    }

    public class SessionInitializationResult
    {
        public GameSessionContext Context { get; set; } = new();
        public System.Collections.Generic.List<string> CompatibilityWarnings { get; set; } = new();
    }
}
