using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal HUD for VS01 Batch 1/2 startup + runtime needs visibility.
    /// </summary>
    public class Vs01SessionHudController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text sessionStateText;

        [Header("Needs Bars (Optional)")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider thirstSlider;
        [SerializeField] private Slider energySlider;
        [SerializeField] private Slider stressSlider;

        [Header("Optional Placeholder Anchors")]
        [SerializeField] private Transform[] placeholderInteractableAnchors;
        [SerializeField] private Text anchorInfoText;

        private void Start()
        {
            SetAnchorInfo();
        }

        private void Update()
        {
            RefreshHud();
        }

        private void RefreshHud()
        {
            var context = SessionContextRegistry.Current;
            if (context == null || context.playerCharacter == null || context.playerCharacter.data == null)
            {
                SetText(sessionStateText, "Session context: MISSING");
                SetText(playerNameText, "Player: (none)");
                SetText(statusText, "No runtime status available");
                SetNeedsBars(0f, 0f, 0f, 0f);
                return;
            }

            var player = context.playerCharacter;
            var needs = player.needsStatus ?? new NeedsStatus();

            SetText(sessionStateText, $"Session context: READY | PlayerId={context.playerCharacterId}");
            SetText(playerNameText, $"Player: {player.data.displayName}");
            SetText(statusText,
                $"Hunger {needs.hunger:0} | Thirst {needs.thirst:0} | Energy {needs.energy:0} | Stress {needs.stress:0}");

            SetNeedsBars(needs.hunger, needs.thirst, needs.energy, needs.stress);
        }

        private void SetNeedsBars(float hunger, float thirst, float energy, float stress)
        {
            SetSlider(hungerSlider, hunger);
            SetSlider(thirstSlider, thirst);
            SetSlider(energySlider, energy);
            SetSlider(stressSlider, stress);
        }

        private static void SetSlider(Slider slider, float value)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = NeedsStatus.ClampToRange(value);
        }

        private void SetAnchorInfo()
        {
            var count = placeholderInteractableAnchors == null ? 0 : placeholderInteractableAnchors.Length;
            SetText(anchorInfoText, $"Placeholder anchors: {count}");
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
            {
                target.text = value;
            }
        }
    }
}
