using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Gameplay;
using UnityEngine;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// VS01 survival runtime tick driver.
    /// </summary>
    public class Vs01NeedsRuntimeController : MonoBehaviour
    {
        [Header("Tick")]
        [SerializeField] private float realSecondsPerTick = 1f;
        [SerializeField] private float inGameMinutesPerTick = 1f;

        [Header("Environment")]
        [SerializeField] private bool simulateRain;

        private float _tickTimer;
        private readonly SurvivalNeedsTickUseCase _tickUseCase = new();

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer < realSecondsPerTick)
            {
                return;
            }

            _tickTimer = 0f;
            var context = SessionContextRegistry.Current;
            _tickUseCase.Execute(context, simulateRain, inGameMinutesPerTick);
        }
    }
}
