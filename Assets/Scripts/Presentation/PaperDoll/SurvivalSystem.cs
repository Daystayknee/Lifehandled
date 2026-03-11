using UnityEngine;

namespace Lifehandled.Presentation.PaperDoll
{
    /// <summary>
    /// Background survival tick that applies identity trait and environment modifiers.
    /// </summary>
    public class SurvivalSystem : MonoBehaviour
    {
        [SerializeField] private CharacterIdentity characterIdentity;
        [SerializeField] private EnvironmentManager environmentManager;
        [SerializeField] private float tickSeconds = 1f;

        private float _timer;

        private void Update()
        {
            if (characterIdentity == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < tickSeconds)
            {
                return;
            }

            var dt = _timer;
            _timer = 0f;
            Tick(dt / 60f); // Convert to minutes.
        }

        private void Tick(float minutes)
        {
            var env = environmentManager != null ? environmentManager.ActiveEnvironment : null;
            var envMod = env?.statModifier;

            var hungerDrain = 0.65f * characterIdentity.GetHungerDrainMultiplier();
            var socialDrain = 0.45f * characterIdentity.GetSocialDrainMultiplier();

            characterIdentity.hunger -= hungerDrain * minutes;
            characterIdentity.socialEnergy -= socialDrain * minutes;

            if (envMod != null)
            {
                characterIdentity.hunger += envMod.hungerDeltaPerMinute * minutes;
                characterIdentity.socialEnergy += envMod.socialEnergyDeltaPerMinute * minutes;
                characterIdentity.money += Mathf.RoundToInt(envMod.moneyDeltaPerMinute * minutes);
            }

            characterIdentity.money += Mathf.RoundToInt(characterIdentity.GetMoneyDeltaPerDayBonus() * (minutes / 1440f));
            characterIdentity.ClampStats();
        }
    }
}
