using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal HUD for VS01 startup + survival runtime visibility.
    /// </summary>
    public class Vs01SessionHudController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text primaryStatusText;
        [SerializeField] private Text secondaryStatusText;
        [SerializeField] private Text sessionStateText;

        [Header("Needs Bars (Optional)")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider thirstSlider;
        [SerializeField] private Slider energySlider;
        [SerializeField] private Slider warmthSlider;
        [SerializeField] private Slider hygieneSlider;
        [SerializeField] private Slider stressSlider;
        [SerializeField] private Slider moodSlider;
        [SerializeField] private Slider illnessRiskSlider;

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
                SetText(primaryStatusText, "No runtime status available");
                SetText(secondaryStatusText, string.Empty);
                SetNeedsBars(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
                return;
            }

            var player = context.playerCharacter;
            var needs = player.needsStatus ?? new NeedsStatus();

            SetText(sessionStateText, $"Session: READY | PlayerId={context.playerCharacterId} | Location={context.currentLocationId}");
            SetText(playerNameText, $"Player: {player.data.displayName}");
            SetText(primaryStatusText,
                $"Hunger {needs.hunger:0} | Thirst {needs.thirst:0} | Energy {needs.energy:0} | Warmth {needs.warmth:0} | Hygiene {needs.hygiene:0}");
            SetText(secondaryStatusText,
                $"Stress {needs.stress:0} | Mood {needs.mood:0} | IllnessRisk {needs.illnessRisk:0} | Wetness {needs.wetness:0}");

            SetNeedsBars(
                needs.hunger, needs.thirst, needs.energy, needs.warmth, needs.hygiene,
                needs.stress, needs.mood, needs.illnessRisk);
        }

        private void SetNeedsBars(float hunger, float thirst, float energy, float warmth, float hygiene, float stress, float mood, float illnessRisk)
        {
            SetSlider(hungerSlider, hunger);
            SetSlider(thirstSlider, thirst);
            SetSlider(energySlider, energy);
            SetSlider(warmthSlider, warmth);
            SetSlider(hygieneSlider, hygiene);
            SetSlider(stressSlider, stress);
            SetSlider(moodSlider, mood);
            SetSlider(illnessRiskSlider, illnessRisk);
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
