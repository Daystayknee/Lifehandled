using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.World;
using Lifehandled.Domain.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 1 pure 2D map/travel foundation.
    /// Connect this to node buttons for zone-based navigation.
    /// </summary>
    public class Pure2DMapTravelController : MonoBehaviour
    {
        [Header("Optional node buttons")]
        [SerializeField] private Button homeButton;
        [SerializeField] private Button townButton;
        [SerializeField] private Button storeButton;
        [SerializeField] private Button forestButton;
        [SerializeField] private Button lakeButton;

        [SerializeField] private Text mapStatusText;

        private readonly TravelToZoneUseCase _travelToZoneUseCase = new();

        private void Awake()
        {
            Bind(homeButton, ZoneType.Home);
            Bind(townButton, ZoneType.TownCenter);
            Bind(storeButton, ZoneType.GasStation);
            Bind(forestButton, ZoneType.Forest);
            Bind(lakeButton, ZoneType.Lake);
        }

        private void Bind(Button button, ZoneType zone)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.AddListener(() => Travel(zone));
        }

        public void Travel(ZoneType zone)
        {
            var context = SessionContextRegistry.Current;
            var ok = _travelToZoneUseCase.Execute(context, zone, out var message);
            SetStatus(ok ? $"Travel OK: {message}" : $"Travel WARN: {message}");
        }

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                return;
            }

            if (mapStatusText != null && string.IsNullOrWhiteSpace(mapStatusText.text))
            {
                mapStatusText.text = $"Map ready. Current node: {context.currentZone}";
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
