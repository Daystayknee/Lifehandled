using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal HUD for VS01 startup + survival/world runtime visibility.
    /// </summary>
    public class Vs01SessionHudController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text primaryStatusText;
        [SerializeField] private Text secondaryStatusText;
        [SerializeField] private Text worldStatusText;
        [SerializeField] private Text sessionStateText;

        [Header("Needs Bars (Optional)")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider thirstSlider;
        [SerializeField] private Slider energySlider;
        [SerializeField] private Slider warmthSlider;
        [SerializeField] private Slider hygieneSlider;
        [SerializeField] private Slider stressSlider;
        [SerializeField] private Slider moodSlider;
        [SerializeField] private Slider illnessRiskSlider;

        [Header("Optional Placeholder Anchors")]
        [SerializeField] private Transform[] placeholderInteractableAnchors;
        [SerializeField] private Text anchorInfoText;

        private void Start()
        {
            SetAnchorInfo();
        }

        private void Update()
        {
            RefreshHud();
        }

        private void RefreshHud()
        {
            var context = SessionContextRegistry.Current;
            if (context == null || context.playerCharacter == null || context.playerCharacter.data == null)
            {
                SetText(sessionStateText, "Session context: MISSING");
                SetText(playerNameText, "Player: (none)");
                SetText(primaryStatusText, "No runtime status available");
                SetText(secondaryStatusText, string.Empty);
                SetText(worldStatusText, string.Empty);
                SetNeedsBars(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
                return;
            }

            var player = context.playerCharacter;
            context.economy ??= new EconomyState();
            context.home ??= new HomeLifeState();
            context.progression ??= new ProgressionState();
            context.collectibles ??= new System.Collections.Generic.List<string>();
            context.familyLineage ??= new FamilyLineageState();
            var needs = player.needsStatus ?? new NeedsStatus();
            var modifiers = player.geneticModifiers ?? new Lifehandled.Domain.Character.GeneticModifierProfile();
            var ageStage = player.data.ageStage;
            var ageSubStage = player.data.ageSubStage;
            var daysToNextBirthday = AgeStageClassifier.ResolveDaysUntilNextBirthday(context.currentDay);
            var nextStageLabel = AgeStageClassifier.TryGetNextMajorStage(ageStage, out var nextMajorStage)
                ? nextMajorStage.ToString()
                : "none";
            var activeZone = context.zones?.FirstOrDefault(z => z.zoneType == context.currentZone);
            var currentSocialEvent = ResolveSocialEventTag(activeZone?.events, context.currentDay);
            var interactableCount = activeZone?.resources?.Count(r => r == "bench" || r == "atm" || r == "vending_machine" || r == "cooking_station" || r == "trash_can") ?? 0;

            SetText(sessionStateText,
                $"Session: READY | PlayerId={context.playerCharacterId} | Location={context.currentLocationId} | Geno M:{modifiers.metabolismMultiplier:0.00} S:{modifiers.staminaRecoveryMultiplier:0.00} I:{modifiers.illnessRiskGainMultiplier:0.00}");
            SetText(playerNameText, $"Player: {player.data.displayName}");
            SetText(primaryStatusText,
                $"Hunger {needs.hunger:0} | Thirst {needs.thirst:0} | Energy {needs.energy:0} | Warmth {needs.warmth:0} | Hygiene {needs.hygiene:0}");
            SetText(secondaryStatusText,
                $"Age {player.data.ageYears} ({ageStage}-{ageSubStage}) | Next {nextStageLabel} in ~{daysToNextBirthday}d | Stress {needs.stress:0} | Mood {needs.mood:0} | IllnessRisk {needs.illnessRisk:0} | Wetness {needs.wetness:0} | Perks {context.progression.unlockedPerkIds.Count} | Collect {context.collectibles.Count} | Gen {context.familyLineage.generationIndex}");
            var weekendTag = context.isWeekend ? "Weekend" : "Weekday";
            var holidayTag = string.IsNullOrWhiteSpace(context.activeHolidayId) || context.activeHolidayId == "none"
                ? "none"
                : context.activeHolidayId;
            var weatherMoodHint = ResolveWeatherMoodHint(context.weather, context.isDaytime);
            SetText(worldStatusText,
                $"Day {context.currentDay} ({context.dayOfWeekName}) {context.monthName}-{context.dayOfMonth:00} {weekendTag} | Holiday {holidayTag} | Zone {context.currentZone} | Event {currentSocialEvent} | Interactables {interactableCount} | {context.season} | {context.weather} ({weatherMoodHint}) | Shop {(context.shopOpen ? "OPEN" : "CLOSED")} | NPCsOut {context.npcOutsideFactor:0.00} | Food x{context.foodPriceMultiplier:0.00} | HomeComfort {context.home.homeComfort:0} | HomeClean {context.home.cleanliness:0} | SocRep {context.socialReputation:0} | FamilyTension {context.familyTension:0}");

            SetNeedsBars(
                needs.hunger, needs.thirst, needs.energy, needs.warmth, needs.hygiene,
                needs.stress, needs.mood, needs.illnessRisk);
        }

        private void SetNeedsBars(float hunger, float thirst, float energy, float warmth, float hygiene, float stress, float mood, float illnessRisk)
        {
            SetSlider(hungerSlider, hunger);
            SetSlider(thirstSlider, thirst);
            SetSlider(energySlider, energy);
            SetSlider(warmthSlider, warmth);
            SetSlider(hygieneSlider, hygiene);
            SetSlider(stressSlider, stress);
            SetSlider(moodSlider, mood);
            SetSlider(illnessRiskSlider, illnessRisk);
        }

        private static void SetSlider(Slider slider, float value)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = NeedsStatus.ClampToRange(value);
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

        private static string ResolveWeatherMoodHint(Lifehandled.Domain.Common.WeatherType weather, bool isDaytime)
        {
            return weather switch
            {
                Lifehandled.Domain.Common.WeatherType.Clear when isDaytime => "mood+",
                Lifehandled.Domain.Common.WeatherType.Cloudy => "mood-",
                Lifehandled.Domain.Common.WeatherType.Rain => "mood--",
                Lifehandled.Domain.Common.WeatherType.Storm => "mood---",
                _ => "neutral"
            };
        }

        private static string ResolveSocialEventTag(System.Collections.Generic.List<string> events, int day)
        {
            if (events == null || events.Count == 0)
            {
                return "none";
            }

            var prefix = $"social_event_day:{day}:";
            var eventTag = events.FirstOrDefault(e => e.StartsWith(prefix));
            if (string.IsNullOrWhiteSpace(eventTag))
            {
                return "none";
            }

            return eventTag.Substring(prefix.Length);
        }
    }
}
