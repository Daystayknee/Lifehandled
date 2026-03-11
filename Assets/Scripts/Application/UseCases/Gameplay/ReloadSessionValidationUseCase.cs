using Lifehandled.Application.Ports;
using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Session;
using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.UseCases.Gameplay
{
    /// <summary>
    /// Reloads save and reinitializes session context for validation.
    /// </summary>
    public class ReloadSessionValidationUseCase
    {
        private readonly IGameSaveStore _saveStore;

        public ReloadSessionValidationUseCase(IGameSaveStore saveStore)
        {
            _saveStore = saveStore;
        }

        public bool Execute(out string message)
        {
            var envelope = _saveStore.Load() ?? new SaveGameEnvelope();

            var initializer = new InitializeGameSessionUseCase(new Infrastructure.Genetics.DefaultGeneticModifierProvider());
            var result = initializer.Execute(envelope);
            SessionContextRegistry.Set(result.Context);

            var context = result.Context;
            var ok = context != null &&
                     !string.IsNullOrWhiteSpace(context.playerCharacterId) &&
                     context.playerCharacter?.data != null &&
                     context.playerCharacter.geneticModifiers != null;

            if (!ok)
            {
                message = "Reload validation failed: missing player context.";
                return false;
            }

            message =
                $"Reload OK: Player={context.playerCharacter.data.displayName}, HouseholdMembers={context.householdMembers.Count}, Genetics=loaded, NPCs={context.npcs.Count}, World={context.season}/{context.weather}, Day={context.currentDay} {context.hourOfDay:00.0}h";
            return true;
        }
    }
}
