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
                hourOfDay = envelope.vs01State.hourOfDay,
                wallet = envelope.vs01State.wallet,
                currentLocationId = envelope.vs01State.currentLocationId,
                talkCountToday = envelope.vs01State.talkCountToday,
                season = envelope.vs01State.season,
                weather = envelope.vs01State.weather,
                isDaytime = envelope.vs01State.isDaytime,
                shopOpen = envelope.vs01State.shopOpen,
                npcOutsideFactor = envelope.vs01State.npcOutsideFactor,
                foodPriceMultiplier = envelope.vs01State.foodPriceMultiplier,
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
            };        }

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
