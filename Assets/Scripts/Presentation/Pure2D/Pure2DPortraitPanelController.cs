using Lifehandled.Application.Session;
using Lifehandled.Presentation.Pure2D.Avatar;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 1 pure 2D portrait panel.
    /// Hosts a layered renderer and summary text for the selected character.
    /// </summary>
    public class Pure2DPortraitPanelController : MonoBehaviour
    {
        [SerializeField] private LayeredPortraitRenderer portraitRenderer;
        [SerializeField] private Text summaryText;
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
                summaryText.text = $"{character.displayName} | Style anime × semi-real | Face {character.appearance.faceShape} | Hair {character.appearance.hairType}/{character.appearance.hairLength}";
            }
        }
    }
}
