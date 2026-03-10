using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Gameplay;
using Lifehandled.Application.UseCases.Economy;
using Lifehandled.Application.UseCases.World;
using Lifehandled.Domain.Common;
using Lifehandled.Application.UseCases.NPC;
using Lifehandled.Application.UseCases.LifeSim;
using UnityEngine;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// VS01 runtime tick driver for world + survival state.
    /// </summary>
    public class Vs01NeedsRuntimeController : MonoBehaviour
    {
        [Header("Tick")]
        [SerializeField] private float realSecondsPerTick = 1f;
        [SerializeField] private float inGameMinutesPerTick = 1f;

        [Header("Debug Override")]
        [SerializeField] private bool forceRain;

        private float _tickTimer;
        private readonly WorldSimulationTickUseCase _worldTickUseCase = new();
        private readonly EconomySimulationTickUseCase _economyTickUseCase = new();
        private readonly SurvivalNeedsTickUseCase _survivalTickUseCase = new();
        private readonly NpcScheduleTickUseCase _npcScheduleTickUseCase = new();
        private readonly HomeLifeTickUseCase _homeLifeTickUseCase = new();

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer < realSecondsPerTick)
            {
                return;
            }

            _tickTimer = 0f;
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                return;
            }

            _worldTickUseCase.Execute(context, inGameMinutesPerTick);
            _economyTickUseCase.Execute(context);

            var isRaining = forceRain || context.weather == WeatherType.Rain || context.weather == WeatherType.Storm;
            _survivalTickUseCase.Execute(context, isRaining, inGameMinutesPerTick);
            _homeLifeTickUseCase.Execute(context, inGameMinutesPerTick);
            _npcScheduleTickUseCase.Execute(context);
        }
    }
}
