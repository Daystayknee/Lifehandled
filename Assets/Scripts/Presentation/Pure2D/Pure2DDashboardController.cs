using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 1 pure 2D dashboard foundation.
    /// Central life-control view for top-level session visibility.
    /// </summary>
    public class Pure2DDashboardController : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text zoneText;
        [SerializeField] private Text timeWeatherText;
        [SerializeField] private Image weatherIconImage;
        [SerializeField] private Pure2DIconRegistry iconRegistry;

        [Header("Core Stats")]
        [SerializeField] private Text needsText;
        [SerializeField] private Text moneyText;
        [SerializeField] private Text householdText;
        [SerializeField] private Text reputationText;

        [Header("Life Feed Teaser")]
        [SerializeField] private Text worldEventText;
        [SerializeField] private Text npcReactionText;
        [SerializeField] private Text economyText;

        private void Update()
        {
            Render(SessionContextRegistry.Current);
        }

        private void Render(GameSessionContext context)
        {
            if (context?.playerCharacter?.data == null)
            {
                SetText(playerNameText, "Player: (none)");
                SetText(zoneText, "Zone: -");
                SetText(timeWeatherText, "Time: -");
                if (weatherIconImage != null) weatherIconImage.enabled = false;
                SetText(needsText, "Needs: -");
                SetText(moneyText, "Wallet: -");
                SetText(householdText, "Household: -");
                SetText(reputationText, "Reputation: -");
                SetText(worldEventText, "World: -");
                SetText(npcReactionText, "NPC: -");
                SetText(economyText, "Economy: -");
                return;
            }

            var needs = context.playerCharacter.needsStatus ?? new NeedsStatus();

            SetText(playerNameText, $"{context.playerCharacter.data.displayName}");
            SetText(zoneText, $"Zone: {context.currentZone}");
            SetText(timeWeatherText,
                $"🌤 Day {context.currentDay} ({context.dayOfWeekName}) {context.monthName}-{context.dayOfMonth:00} | {context.hourOfDay:00.0}h | {context.weather}");
            ApplyWeatherIcon(context.weather);
            SetText(needsText,
                $"H:{needs.hunger:0} T:{needs.thirst:0} E:{needs.energy:0} S:{needs.stress:0} M:{needs.mood:0}");
            SetText(moneyText, $"Wallet: ${context.wallet}");
            SetText(householdText,
                $"Members: {context.householdMembers.Count} | Tension: {context.familyTension:0} | Home: {(context.home?.isHomeOwned == true ? "Owned" : "Rent")}");
            SetText(reputationText,
                $"SocialRep: {context.socialReputation:0} | Holiday: {(context.activeHolidayId == "none" ? "none" : context.activeHolidayId)}");
            SetText(worldEventText, $"🗞 {context.lastWorldEvent}");
            SetText(npcReactionText, $"💬 {context.lastNpcReaction}");
            SetText(economyText, $"💸 {context.lastEconomyEvent}");
        }

        private void ApplyWeatherIcon(Lifehandled.Domain.Common.WeatherType weather)
        {
            if (weatherIconImage == null || iconRegistry == null)
            {
                return;
            }

            var key = weather switch
            {
                Lifehandled.Domain.Common.WeatherType.Clear => "weather_clear",
                Lifehandled.Domain.Common.WeatherType.Cloudy => "weather_cloudy",
                Lifehandled.Domain.Common.WeatherType.Rain => "weather_rain",
                Lifehandled.Domain.Common.WeatherType.Storm => "weather_storm",
                _ => string.Empty
            };

            weatherIconImage.sprite = iconRegistry.Resolve(key);
            weatherIconImage.enabled = weatherIconImage.sprite != null;
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
