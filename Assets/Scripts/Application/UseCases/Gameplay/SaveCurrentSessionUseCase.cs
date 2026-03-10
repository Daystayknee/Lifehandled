using System.Collections.Generic;
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
            var envelope = _saveStore.Load();
            envelope.vs01State.currentDay = context.currentDay;
            envelope.vs01State.hourOfDay = context.hourOfDay;
            envelope.vs01State.wallet = context.wallet;
            envelope.vs01State.currentLocationId = context.currentLocationId;
            envelope.vs01State.talkCountToday = context.talkCountToday;

            envelope.vs01State.season = context.season;
            envelope.vs01State.weather = context.weather;
            envelope.vs01State.isDaytime = context.isDaytime;
            envelope.vs01State.shopOpen = context.shopOpen;
            envelope.vs01State.npcOutsideFactor = context.npcOutsideFactor;
            envelope.vs01State.foodPriceMultiplier = context.foodPriceMultiplier;

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

            _saveStore.Save(envelope);
        }
    }
}
