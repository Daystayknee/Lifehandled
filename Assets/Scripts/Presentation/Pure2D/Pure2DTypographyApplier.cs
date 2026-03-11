using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Applies typography theme to heading/body/note text groups.
    /// </summary>
    public class Pure2DTypographyApplier : MonoBehaviour
    {
        [SerializeField] private Pure2DTypographyTheme theme;
        [SerializeField] private Text[] headingTexts;
        [SerializeField] private Text[] bodyTexts;
        [SerializeField] private Text[] noteTexts;

        private void Awake()
        {
            if (theme == null)
            {
                return;
            }

            Apply(theme.headingFont, theme.headingSize, headingTexts);
            Apply(theme.bodyFont, theme.bodySize, bodyTexts);
            Apply(theme.noteFont != null ? theme.noteFont : theme.bodyFont, theme.noteSize, noteTexts);
        }

        private static void Apply(Font font, int size, Text[] texts)
        {
            if (texts == null)
            {
                return;
            }

            foreach (var text in texts)
            {
                if (text == null)
                {
                    continue;
                }

                if (font != null)
                {
                    text.font = font;
                }

                if (size > 0)
                {
                    text.fontSize = size;
                }
            }
        }
    }
}
