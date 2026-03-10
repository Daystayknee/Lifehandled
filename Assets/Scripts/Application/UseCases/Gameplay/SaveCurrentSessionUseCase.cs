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
            envelope.vs01State.wallet = context.wallet;

            envelope.vs01State.hunger = context.playerCharacter.needsStatus.hunger;
            envelope.vs01State.thirst = context.playerCharacter.needsStatus.thirst;
            envelope.vs01State.energy = context.playerCharacter.needsStatus.energy;
            envelope.vs01State.stress = context.playerCharacter.needsStatus.stress;

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
