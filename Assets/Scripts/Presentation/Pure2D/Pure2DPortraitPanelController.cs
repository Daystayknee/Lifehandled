using Lifehandled.Application.Session;
using Lifehandled.Presentation.Pure2D.Avatar;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Portrait panel for visual-novel-like anime × semi-real character framing.
    /// </summary>
    public class Pure2DPortraitPanelController : MonoBehaviour
    {
        [SerializeField] private LayeredPortraitRenderer portraitRenderer;
        [SerializeField] private Text summaryText;
        [SerializeField] private Text styleNoteText;
        [SerializeField] private bool showPlayerCharacter = true;

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            var character = showPlayerCharacter
                ? context?.playerCharacter?.data
                : context?.householdMembers?.Count > 0
                    ? context.householdMembers[0].data
                    : null;

            if (character == null)
            {
                if (summaryText != null)
                {
                    summaryText.text = "Portrait: no character";
                }

                return;
            }

            if (portraitRenderer != null)
            {
                portraitRenderer.Render(character.appearance);
            }

            if (summaryText != null)
            {
                summaryText.text =
                    $"{character.displayName}\nMood {context?.playerCharacter?.needsStatus?.mood ?? 50f:0}  •  Energy {context?.playerCharacter?.needsStatus?.energy ?? 50f:0}  •  Hunger {context?.playerCharacter?.needsStatus?.hunger ?? 50f:0}";
            }

            if (styleNoteText != null)
            {
                styleNoteText.text = "Style: anime-inspired eyes + natural noses + soft shading + realistic skin tones. Avoid chibi/hyper-real extremes.";
            }
        }
    }
}
