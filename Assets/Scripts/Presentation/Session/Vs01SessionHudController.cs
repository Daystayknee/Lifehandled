using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal HUD stub for VS01 Batch 1 startup validation.
    /// Shows selected player and basic runtime state from session context.
    /// </summary>
    public class Vs01SessionHudController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text sessionStateText;

        [Header("Optional Placeholder Anchors")]
        [SerializeField] private Transform[] placeholderInteractableAnchors;
        [SerializeField] private Text anchorInfoText;

        private void Start()
        {
            var context = SessionContextRegistry.Current;
            if (context == null || context.playerCharacter == null || context.playerCharacter.data == null)
            {
                SetText(sessionStateText, "Session context: MISSING");
                SetText(playerNameText, "Player: (none)");
                SetText(statusText, "No runtime status available");
                SetAnchorInfo();
                return;
            }

            var player = context.playerCharacter;
            var needs = player.needsStatus;

            SetText(sessionStateText, $"Session context: READY | PlayerId={context.playerCharacterId}");
            SetText(playerNameText, $"Player: {player.data.displayName}");
            SetText(statusText,
                $"Hunger {needs.hunger:0} | Thirst {needs.thirst:0} | Energy {needs.energy:0} | Stress {needs.stress:0}");

            SetAnchorInfo();
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
