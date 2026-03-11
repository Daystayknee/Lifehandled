using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 4 scrapbook styling pass: decorative tags + tone color.
    /// </summary>
    public class Pure2DScrapbookStyleController : MonoBehaviour
    {
        [SerializeField] private Text headerText;
        [SerializeField] private Text tagLineText;
        [SerializeField] private Image paperTintImage;

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                return;
            }

            if (headerText != null)
            {
                headerText.text = $"📓 Life Journal — Day {context.currentDay}";
            }

            if (tagLineText != null)
            {
                tagLineText.text = $"#{context.currentZone.ToString().ToLowerInvariant()} #{context.weather.ToString().ToLowerInvariant()} #{(context.activeHolidayId == "none" ? "daily_life" : context.activeHolidayId)}";
            }

            if (paperTintImage != null)
            {
                paperTintImage.color = context.weather switch
                {
                    Domain.Common.WeatherType.Clear => new Color(1f, 0.96f, 0.9f, 0.94f),
                    Domain.Common.WeatherType.Cloudy => new Color(0.94f, 0.94f, 0.94f, 0.95f),
                    Domain.Common.WeatherType.Rain => new Color(0.88f, 0.9f, 0.96f, 0.95f),
                    Domain.Common.WeatherType.Storm => new Color(0.82f, 0.85f, 0.92f, 0.95f),
                    _ => new Color(1f, 1f, 1f, 0.95f)
                };
            }
        }
    }
}
