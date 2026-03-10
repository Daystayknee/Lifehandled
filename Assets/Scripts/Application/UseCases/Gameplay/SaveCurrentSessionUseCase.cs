using System.Collections.Generic;
using System.Linq;
using Lifehandled.Application.Ports;
using Lifehandled.Application.Session;
using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.UseCases.Gameplay
{
    public class SaveCurrentSessionUseCase
    {
        private readonly IGameSaveStore _saveStore;

        public SaveCurrentSessionUseCase(IGameSaveStore saveStore)
        {
            _saveStore = saveStore;
        }

        public void Execute(GameSessionContext context)
        {
            context.economy ??= new EconomyState();
            context.home ??= new HomeLifeState();

            var envelope = _saveStore.Load();
            envelope.vs01State.currentDay = context.currentDay;
            envelope.vs01State.hourOfDay = context.hourOfDay;
            envelope.vs01State.wallet = context.wallet;
            envelope.vs01State.currentLocationId = context.currentLocationId;
            envelope.vs01State.talkCountToday = context.talkCountToday;
            envelope.vs01State.socialReputation = context.socialReputation;
            envelope.vs01State.familyTension = context.familyTension;

            envelope.vs01State.season = context.season;
            envelope.vs01State.weather = context.weather;
            envelope.vs01State.isDaytime = context.isDaytime;
            envelope.vs01State.shopOpen = context.shopOpen;
            envelope.vs01State.npcOutsideFactor = context.npcOutsideFactor;
            envelope.vs01State.foodPriceMultiplier = context.foodPriceMultiplier;

            envelope.vs01State.currentJob = context.economy.currentJob;
            envelope.vs01State.dailyIncome = context.economy.dailyIncome;
            envelope.vs01State.housingTier = context.economy.housingTier;
            envelope.vs01State.weeklyRentCost = context.economy.weeklyRentCost;
            envelope.vs01State.scarcityMultiplier = context.economy.scarcityMultiplier;
            envelope.vs01State.regionWealthMultiplier = context.economy.regionWealthMultiplier;
            envelope.vs01State.reputationMultiplier = context.economy.reputationMultiplier;
            envelope.vs01State.supplyDemandMultiplier = context.economy.supplyDemandMultiplier;
            envelope.vs01State.lastIncomePaidDay = context.economy.lastIncomePaidDay;
            envelope.vs01State.lastRentPaidDay = context.economy.lastRentPaidDay;

            envelope.vs01State.isHomeOwned = context.home.isHomeOwned;
            envelope.vs01State.furnitureCount = context.home.furnitureCount;
            envelope.vs01State.homeComfort = context.home.homeComfort;
            envelope.vs01State.homeCleanliness = context.home.cleanliness;
            envelope.vs01State.storageCapacityBase = context.home.storageCapacityBase;
            envelope.vs01State.storageCapacityBonus = context.home.storageCapacityBonus;
            envelope.vs01State.neighborhoodReputation = context.home.neighborhoodReputation;

            var needs = context.playerCharacter.needsStatus;
            envelope.vs01State.hunger = needs.hunger;
            envelope.vs01State.thirst = needs.thirst;
            envelope.vs01State.energy = needs.energy;
            envelope.vs01State.warmth = needs.warmth;
            envelope.vs01State.hygiene = needs.hygiene;
            envelope.vs01State.stress = needs.stress;
            envelope.vs01State.mood = needs.mood;
            envelope.vs01State.illnessRisk = needs.illnessRisk;
            envelope.vs01State.wetness = needs.wetness;

            envelope.vs01State.inventory = new List<Vs01ItemStack>();
            foreach (var item in context.inventory.items)
            {
                if (item.count > 0)
                {
                    envelope.vs01State.inventory.Add(new Vs01ItemStack { itemId = item.itemId, count = item.count });
                }
            }

            envelope.vs01State.npcs = context.npcs.Select(n => new Vs01NpcState
            {
                npcId = n.profile.npcId,
                displayName = n.profile.displayName,
                personalityTraits = n.profile.personalityTraits,
                hunger = n.profile.needs.hunger,
                energy = n.profile.needs.energy,
                social = n.profile.needs.social,
                mood = n.profile.mood,
                currentScheduleBlock = n.profile.schedule.currentBlock,
                isAvailableForTalk = n.profile.schedule.isAvailableForTalk,
                friendship = n.profile.relationshipToPlayer.friendship,
                trust = n.profile.relationshipToPlayer.trust,
                attraction = n.profile.relationshipToPlayer.attraction,
                respect = n.profile.relationshipToPlayer.respect,
                fear = n.profile.relationshipToPlayer.fear,
                resentment = n.profile.relationshipToPlayer.resentment,
                knownSecretsCount = n.profile.drama?.knownSecretsCount ?? 0,
                gossipHeat = n.profile.drama?.gossipHeat ?? 0f,
                rumorBelief = n.profile.drama?.rumorBelief ?? 0f,
                rivalryWithPlayer = n.profile.drama?.rivalryWithPlayer ?? 0f,
                romanceInterest = n.profile.drama?.romanceInterest ?? 0f,
                familyTensionWithPlayer = n.profile.drama?.familyTensionWithPlayer ?? 0f,
                socialReputationOfPlayer = n.profile.drama?.socialReputationOfPlayer ?? 50f,
                memory = n.profile.memory.Select(m => new Vs01NpcMemoryEntry
                {
                    day = m.day,
                    hour = m.hour,
                    interactionType = m.interactionType,
                    outcome = m.outcome
                }).ToList()
            }).ToList();

            _saveStore.Save(envelope);
        }
    }
}
