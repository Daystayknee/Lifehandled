using Lifehandled.Application.Session;
using UnityEngine;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// VS01 Batch 2: minimal runtime needs decay loop for the selected player.
    /// </summary>
    public class Vs01NeedsRuntimeController : MonoBehaviour
    {
        [Header("Decay Per In-Game Minute")]
        [SerializeField] private float hungerDecayPerMinute = 0.4f;
        [SerializeField] private float thirstDecayPerMinute = 0.6f;
        [SerializeField] private float energyDecayPerMinute = 0.3f;
        [SerializeField] private float stressGainPerMinute = 0.2f;

        [Header("Tick")]
        [SerializeField] private float realSecondsPerTick = 1f;
        [SerializeField] private float inGameMinutesPerTick = 1f;

        private float _tickTimer;

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer < realSecondsPerTick)
            {
                return;
            }

            _tickTimer = 0f;
            TickNeeds();
        }

        private void TickNeeds()
        {
            var player = SessionContextRegistry.Current?.playerCharacter;
            if (player == null)
            {
                return;
            }

            var needs = player.needsStatus;
            if (needs == null)
            {
                needs = new NeedsStatus();
                player.needsStatus = needs;
            }

            needs.hunger = NeedsStatus.ClampToRange(needs.hunger + (hungerDecayPerMinute * inGameMinutesPerTick));
            needs.thirst = NeedsStatus.ClampToRange(needs.thirst + (thirstDecayPerMinute * inGameMinutesPerTick));
            needs.energy = NeedsStatus.ClampToRange(needs.energy - (energyDecayPerMinute * inGameMinutesPerTick));
            needs.stress = NeedsStatus.ClampToRange(needs.stress + (stressGainPerMinute * inGameMinutesPerTick));
        }
    }
}
