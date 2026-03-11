using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 4 weather visual polish for dashboard/zone panels.
    /// </summary>
    public class Pure2DWeatherOverlayController : MonoBehaviour
    {
        [SerializeField] private Image weatherOverlayImage;
        [SerializeField] private Image weatherIconImage;
        [SerializeField] private Pure2DIconRegistry iconRegistry;

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                return;
            }

            ApplyOverlay(context.weather);
            ApplyIcon(context.weather);
        }

        private void ApplyOverlay(WeatherType weather)
        {
            if (weatherOverlayImage == null)
            {
                return;
            }

            weatherOverlayImage.color = weather switch
            {
                WeatherType.Clear => new Color(1f, 0.95f, 0.8f, 0.06f),
                WeatherType.Cloudy => new Color(0.75f, 0.8f, 0.9f, 0.12f),
                WeatherType.Rain => new Color(0.45f, 0.55f, 0.75f, 0.18f),
                WeatherType.Storm => new Color(0.25f, 0.3f, 0.45f, 0.26f),
                _ => new Color(1f, 1f, 1f, 0f)
            };
        }

        private void ApplyIcon(WeatherType weather)
        {
            if (weatherIconImage == null || iconRegistry == null)
            {
                return;
            }

            var key = weather switch
            {
                WeatherType.Clear => "weather_clear",
                WeatherType.Cloudy => "weather_cloudy",
                WeatherType.Rain => "weather_rain",
                WeatherType.Storm => "weather_storm",
                _ => string.Empty
            };

            weatherIconImage.sprite = iconRegistry.Resolve(key);
            weatherIconImage.enabled = weatherIconImage.sprite != null;
        }
    }
}
