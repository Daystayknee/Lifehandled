using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.World;
using Lifehandled.Domain.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Minimal icon-map travel view.
    /// </summary>
    public class Pure2DMapTravelController : MonoBehaviour
    {
        [Header("Optional node buttons")]
        [SerializeField] private Button homeButton;
        [SerializeField] private Button townButton;
        [SerializeField] private Button storeButton;
        [SerializeField] private Button forestButton;
        [SerializeField] private Button lakeButton;
        [SerializeField] private Button clinicButton;

        [SerializeField] private Text mapStatusText;
        [SerializeField] private Text mapLegendText;

        private readonly TravelToZoneUseCase _travelToZoneUseCase = new();

        private void Awake()
        {
            Bind(homeButton, ZoneType.Home, "🏠 Home");
            Bind(townButton, ZoneType.TownCenter, "🏙 Town");
            Bind(storeButton, ZoneType.Store, "🏪 Store");
            Bind(forestButton, ZoneType.Forest, "🌲 Forest");
            Bind(lakeButton, ZoneType.Lake, "🏞 Lake");
            Bind(clinicButton, ZoneType.Clinic, "🏥 Clinic");

            if (mapLegendText != null)
            {
                mapLegendText.text = "Town Map\n🏠 Home  🏪 Store  🌲 Forest  🏞 Lake  🏥 Clinic";
            }
        }

        private void Bind(Button button, ZoneType zone, string label)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.AddListener(() => Travel(zone, label));
        }

        private void Travel(ZoneType zone, string label)
        {
            var context = SessionContextRegistry.Current;
            var ok = _travelToZoneUseCase.Execute(context, zone, out var message);
            SetStatus(ok ? $"{label} → {message}" : $"Travel warning: {message}");
        }

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null || mapStatusText == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(mapStatusText.text))
            {
                mapStatusText.text = $"Current location: {context.currentZone}";
            }
        }

        private void SetStatus(string value)
        {
            if (mapStatusText != null)
            {
                mapStatusText.text = value;
            }
        }
    }
}
