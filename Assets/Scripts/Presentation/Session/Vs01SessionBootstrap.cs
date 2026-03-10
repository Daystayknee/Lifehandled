using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Session;
using Lifehandled.Infrastructure.Genetics;
using Lifehandled.Infrastructure.Persistence.Stores;
using UnityEngine;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Batch 5 readiness bootstrap for first playable scene.
    /// Loads save data, builds runtime session context, and exposes it via registry.
    /// </summary>
    public class Vs01SessionBootstrap : MonoBehaviour
    {
        [SerializeField] private string saveFileName = "savegame.json";

        private void Awake()
        {
            var saveStore = new JsonNewGameSaveStore(saveFileName);
            var envelope = saveStore.Load();

            var initializeSession = new InitializeGameSessionUseCase(new DefaultGeneticModifierProvider());
            var result = initializeSession.Execute(envelope);

            SessionContextRegistry.Set(result.Context);

            if (result.CompatibilityWarnings != null)
            {
                foreach (var warning in result.CompatibilityWarnings)
                {
                    Debug.LogWarning($"[VS01 Session Bootstrap] {warning}");
                }
            }

            Debug.Log($"[VS01 Session Bootstrap] Player={result.Context.playerCharacterId}, HouseholdMembers={result.Context.householdMembers.Count}");
        }
    }
}
