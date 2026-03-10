using System.Linq;
using Lifehandled.Application.Ports;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;
using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.UseCases.Session
{
    /// <summary>
    /// Batch 3 runtime connection: builds a session context from save data,
    /// assigns the player character, loads household members, and applies
    /// initial genetics modifiers.
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
                playerCharacter = CreateRuntimeCharacter(playerData, false)
            };

            if (activeHousehold != null)
            {
                var members = envelope.characters
                    .Where(c => c.characterId != playerData.characterId &&
                                activeHousehold.memberCharacterIds.Contains(c.characterId));

                foreach (var member in members)
                {
                    context.householdMembers.Add(CreateRuntimeCharacter(member, true));
                }
            }

            return new SessionInitializationResult
            {
                Context = context,
                CompatibilityWarnings = compatibility.Warnings
            };
        }

        private RuntimeCharacterState CreateRuntimeCharacter(CharacterData character, bool lightSim)
        {
            var modifiers = _geneticModifierProvider.BuildModifiers(character.genetics);

            return new RuntimeCharacterState
            {
                data = character,
                geneticModifiers = modifiers,
                needsStatus = NeedsStatus.CreateDefault(modifiers),
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
