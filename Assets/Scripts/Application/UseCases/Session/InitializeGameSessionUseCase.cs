using System.Linq;
using Lifehandled.Application.Ports;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;
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
                wallet = envelope.vs01State.wallet,
                inventory = LoadInventory(envelope.vs01State),
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

        private RuntimeCharacterState CreateRuntimeCharacter(CharacterData character, bool lightSim, Vs01RuntimeState state)
        {
            var modifiers = _geneticModifierProvider.BuildModifiers(character.genetics);
            var defaultNeeds = NeedsStatus.CreateDefault(modifiers);

            return new RuntimeCharacterState
            {
                data = character,
                geneticModifiers = modifiers,
                needsStatus = new NeedsStatus
                {
                    hunger = state.hunger >= 0 ? state.hunger : defaultNeeds.hunger,
                    thirst = state.thirst >= 0 ? state.thirst : defaultNeeds.thirst,
                    energy = state.energy >= 0 ? state.energy : defaultNeeds.energy,
                    stress = state.stress >= 0 ? state.stress : defaultNeeds.stress
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
