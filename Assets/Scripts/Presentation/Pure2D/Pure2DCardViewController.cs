using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 4 card-UI polish helper.
    /// Applies card styling for scrapbook/feed/dashboard panels.
    /// </summary>
    public class Pure2DCardViewController : MonoBehaviour
    {
        [SerializeField] private Image cardBackground;
        [SerializeField] private Outline cardOutline;
        [SerializeField] private Shadow cardShadow;
        [SerializeField] private Text titleText;
        [SerializeField] private Color titleColor = new(0.95f, 0.92f, 1f);
        [SerializeField] private Color bodyColor = new(0.2f, 0.15f, 0.25f, 0.92f);

        private void Awake()
        {
            if (cardBackground != null)
            {
                cardBackground.color = bodyColor;
            }

            if (titleText != null)
            {
                titleText.color = titleColor;
            }

            if (cardOutline != null)
            {
                cardOutline.effectColor = new Color(1f, 0.8f, 0.95f, 0.9f);
                cardOutline.effectDistance = new Vector2(1.2f, -1.2f);
            }

            if (cardShadow != null)
            {
                cardShadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
                cardShadow.effectDistance = new Vector2(2f, -2f);
            }
        }
    }
}
