using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Gameplay;
using Lifehandled.Application.UseCases.LifeSim;
using Lifehandled.Application.UseCases.NPC;
using Lifehandled.Application.UseCases.World;
using Lifehandled.Domain.Common;
using Lifehandled.Infrastructure.Persistence.Stores;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal interaction panel for VS01:
    /// consume, buy, talk, sleep/end-day, save, and reload validation.
    /// </summary>
    public class Vs01InteractionPanelController : MonoBehaviour
    {
        [SerializeField] private Button consumeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button talkButton;
        [SerializeField] private Button helpNpcButton;
        [SerializeField] private Button insultNpcButton;
        [SerializeField] private Button stealFromShopButton;
        [SerializeField] private Button gossipButton;
        [SerializeField] private Button travelHomeButton;
        [SerializeField] private Button travelTownButton;
        [SerializeField] private Button travelStoreButton;
        [SerializeField] private Button travelForestButton;
        [SerializeField] private Button travelLakeButton;
        [SerializeField] private Button travelClinicButton;
        [SerializeField] private Button travelWorkplaceButton;
        [SerializeField] private Button gainCookingSkillButton;
        [SerializeField] private Button unlockPerkButton;
        [SerializeField] private Button collectRelicButton;
        [SerializeField] private Button rareEventButton;
        [SerializeField] private Button upgradePropertyButton;
        [SerializeField] private Button nextGenerationButton;
        [SerializeField] private Button cookButton;
        [SerializeField] private Button cleanButton;
        [SerializeField] private Button buyFurnitureButton;
        [SerializeField] private Button sleepButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button reloadValidateButton;

        [SerializeField] private Text inventoryText;
        [SerializeField] private Text walletText;
        [SerializeField] private Text dayText;
        [SerializeField] private Text contextText;
        [SerializeField] private Text shopText;
        [SerializeField] private Text homeText;
        [SerializeField] private Text npcText;
        [SerializeField] private Text zoneText;
        [SerializeField] private Text progressionText;
        [SerializeField] private Text feedbackText;

        private readonly ConsumeStarterItemUseCase _consumeUseCase = new();
        private readonly BuyStarterItemUseCase _buyUseCase = new();
        private readonly NpcSocialInteractionUseCase _talkUseCase = new();
        private readonly NpcDramaEventUseCase _dramaUseCase = new();
        private readonly CookSimpleMealUseCase _cookUseCase = new();
        private readonly CleanHomeUseCase _cleanHomeUseCase = new();
        private readonly BuyFurnitureUseCase _buyFurnitureUseCase = new();
        private readonly TravelToZoneUseCase _travelToZoneUseCase = new();
        private readonly SkillProgressionUseCase _skillProgressionUseCase = new();
        private readonly PerkUnlockUseCase _perkUnlockUseCase = new();
        private readonly CollectiblesUseCase _collectiblesUseCase = new();
        private readonly RareEventUseCase _rareEventUseCase = new();
        private readonly PropertyUpgradeUseCase _propertyUpgradeUseCase = new();
        private readonly AdvanceGenerationUseCase _advanceGenerationUseCase = new();
        private readonly SleepEndDayUseCase _sleepUseCase = new();
        private SaveCurrentSessionUseCase _saveUseCase;
        private ReloadSessionValidationUseCase _reloadValidationUseCase;

        private void Awake()
        {
            var saveStore = new JsonNewGameSaveStore();
            _saveUseCase = new SaveCurrentSessionUseCase(saveStore);
            _reloadValidationUseCase = new ReloadSessionValidationUseCase(saveStore);

            if (consumeButton != null) consumeButton.onClick.AddListener(OnConsumeClicked);
            if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);
            if (talkButton != null) talkButton.onClick.AddListener(OnTalkClicked);
            if (helpNpcButton != null) helpNpcButton.onClick.AddListener(OnHelpNpcClicked);
            if (insultNpcButton != null) insultNpcButton.onClick.AddListener(OnInsultNpcClicked);
            if (stealFromShopButton != null) stealFromShopButton.onClick.AddListener(OnStealFromShopClicked);
            if (gossipButton != null) gossipButton.onClick.AddListener(OnGossipClicked);
            if (travelHomeButton != null) travelHomeButton.onClick.AddListener(() => OnTravelClicked(ZoneType.Home));
            if (travelTownButton != null) travelTownButton.onClick.AddListener(() => OnTravelClicked(ZoneType.TownCenter));
            if (travelStoreButton != null) travelStoreButton.onClick.AddListener(() => OnTravelClicked(ZoneType.GasStation));
            if (travelForestButton != null) travelForestButton.onClick.AddListener(() => OnTravelClicked(ZoneType.Forest));
            if (travelLakeButton != null) travelLakeButton.onClick.AddListener(() => OnTravelClicked(ZoneType.Lake));
            if (travelClinicButton != null) travelClinicButton.onClick.AddListener(() => OnTravelClicked(ZoneType.Clinic));
            if (travelWorkplaceButton != null) travelWorkplaceButton.onClick.AddListener(() => OnTravelClicked(ZoneType.Apartments));
            if (gainCookingSkillButton != null) gainCookingSkillButton.onClick.AddListener(OnGainCookingSkillClicked);
            if (unlockPerkButton != null) unlockPerkButton.onClick.AddListener(OnUnlockPerkClicked);
            if (collectRelicButton != null) collectRelicButton.onClick.AddListener(OnCollectRelicClicked);
            if (rareEventButton != null) rareEventButton.onClick.AddListener(OnRareEventClicked);
            if (upgradePropertyButton != null) upgradePropertyButton.onClick.AddListener(OnUpgradePropertyClicked);
            if (nextGenerationButton != null) nextGenerationButton.onClick.AddListener(OnNextGenerationClicked);
            if (cookButton != null) cookButton.onClick.AddListener(OnCookClicked);
            if (cleanButton != null) cleanButton.onClick.AddListener(OnCleanClicked);
            if (buyFurnitureButton != null) buyFurnitureButton.onClick.AddListener(OnBuyFurnitureClicked);
            if (sleepButton != null) sleepButton.onClick.AddListener(OnSleepClicked);
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
            if (reloadValidateButton != null) reloadValidateButton.onClick.AddListener(OnReloadValidateClicked);
        }

        private void OnDestroy()
        {
            if (consumeButton != null) consumeButton.onClick.RemoveListener(OnConsumeClicked);
            if (buyButton != null) buyButton.onClick.RemoveListener(OnBuyClicked);
            if (talkButton != null) talkButton.onClick.RemoveListener(OnTalkClicked);
            if (helpNpcButton != null) helpNpcButton.onClick.RemoveListener(OnHelpNpcClicked);
            if (insultNpcButton != null) insultNpcButton.onClick.RemoveListener(OnInsultNpcClicked);
            if (stealFromShopButton != null) stealFromShopButton.onClick.RemoveListener(OnStealFromShopClicked);
            if (gossipButton != null) gossipButton.onClick.RemoveListener(OnGossipClicked);
            if (travelHomeButton != null) travelHomeButton.onClick.RemoveAllListeners();
            if (travelTownButton != null) travelTownButton.onClick.RemoveAllListeners();
            if (travelStoreButton != null) travelStoreButton.onClick.RemoveAllListeners();
            if (travelForestButton != null) travelForestButton.onClick.RemoveAllListeners();
            if (travelLakeButton != null) travelLakeButton.onClick.RemoveAllListeners();
            if (travelClinicButton != null) travelClinicButton.onClick.RemoveAllListeners();
            if (travelWorkplaceButton != null) travelWorkplaceButton.onClick.RemoveAllListeners();
            if (gainCookingSkillButton != null) gainCookingSkillButton.onClick.RemoveListener(OnGainCookingSkillClicked);
            if (unlockPerkButton != null) unlockPerkButton.onClick.RemoveListener(OnUnlockPerkClicked);
            if (collectRelicButton != null) collectRelicButton.onClick.RemoveListener(OnCollectRelicClicked);
            if (rareEventButton != null) rareEventButton.onClick.RemoveListener(OnRareEventClicked);
            if (upgradePropertyButton != null) upgradePropertyButton.onClick.RemoveListener(OnUpgradePropertyClicked);
            if (nextGenerationButton != null) nextGenerationButton.onClick.RemoveListener(OnNextGenerationClicked);
            if (cookButton != null) cookButton.onClick.RemoveListener(OnCookClicked);
            if (cleanButton != null) cleanButton.onClick.RemoveListener(OnCleanClicked);
            if (buyFurnitureButton != null) buyFurnitureButton.onClick.RemoveListener(OnBuyFurnitureClicked);
            if (sleepButton != null) sleepButton.onClick.RemoveListener(OnSleepClicked);
            if (saveButton != null) saveButton.onClick.RemoveListener(OnSaveClicked);
            if (reloadValidateButton != null) reloadValidateButton.onClick.RemoveListener(OnReloadValidateClicked);
        }

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                SetText(inventoryText, "Inventory: -");
                SetText(walletText, "Wallet: -");
                SetText(dayText, "Day: -");
                SetText(contextText, "Player/Household: -");
                SetText(shopText, "Shop: -");
                SetText(homeText, "Home: -");
                SetText(npcText, "NPC: -");
                SetText(zoneText, "Zone: -");
                SetText(progressionText, "Progression: -");
                return;
            }

            context.economy ??= new EconomyState();
            context.home ??= new HomeLifeState();
            context.zones ??= BuildZoneCatalogUseCase.CreateDefault();

            SetText(inventoryText,
                $"Water: {context.inventory.GetCount(ConsumeStarterItemUseCase.WaterBottleId)} | StaleFood: {context.inventory.GetCount(ConsumeStarterItemUseCase.BadFoodId)}");
            SetText(walletText, $"Wallet: ${context.wallet}");
            SetText(dayText, $"Day: {context.currentDay}");

            var playerName = context.playerCharacter?.data?.displayName ?? "(none)";
            SetText(contextText, $"Player: {playerName} | HouseholdMembers: {context.householdMembers.Count} | TalksToday: {context.talkCountToday} | SocRep: {context.socialReputation:0} | FamilyTension: {context.familyTension:0}");

            var price = BuyStarterItemUseCase.ResolvePrice(context);
            SetText(shopText,
                $"Shop: {(context.shopOpen ? "OPEN" : "CLOSED")} | Water Price: ${price} | Job: {context.economy.currentJob} +${context.economy.dailyIncome}/day | Rent ${context.economy.weeklyRentCost}/week");

            var storageUsed = context.inventory.GetTotalItemCount();
            var storageCap = context.home.GetStorageCapacity();
            SetText(homeText,
                $"Home {(context.home.isHomeOwned ? "OWNED" : "RENT")} | Comfort {context.home.homeComfort:0} | Clean {context.home.cleanliness:0} | Storage {storageUsed}/{storageCap} | Rep {context.home.neighborhoodReputation:0}");

            var npcCount = context.npcs?.Count ?? 0;
            var npcName = npcCount > 0 ? context.npcs[0].profile.displayName : "none";
            var npcMood = npcCount > 0 ? context.npcs[0].profile.mood.ToString("0") : "-";
            var npcDrama = npcCount > 0 ? context.npcs[0].profile.drama : null;
            SetText(npcText, $"NPC: {npcName} | Mood: {npcMood} | Rivalry: {(npcDrama?.rivalryWithPlayer ?? 0f):0} | Romance: {(npcDrama?.romanceInterest ?? 0f):0} | Gossip: {(npcDrama?.gossipHeat ?? 0f):0} | Time: {context.hourOfDay:00.0}");

            var activeZone = context.zones?.Find(z => z.zoneType == context.currentZone);
            var npcPoolCount = activeZone?.npcPool?.Count ?? 0;
            var resourceCount = activeZone?.resources?.Count ?? 0;
            var eventCount = activeZone?.events?.Count ?? 0;
            var todayEvent = activeZone?.events?.Find(e => e.StartsWith($"social_event_day:{context.currentDay}:"));
            var todayEventValue = string.IsNullOrWhiteSpace(todayEvent)
                ? "none"
                : todayEvent.Substring($"social_event_day:{context.currentDay}:".Length);
            SetText(zoneText,
                $"Zone: {context.currentZone} | Event {todayEventValue} | NPC Pool {npcPoolCount} | Resources {resourceCount} | Events {eventCount} | Danger {(activeZone?.dangerLevel ?? 0f):0.00}");

            context.progression ??= new ProgressionState();
            context.collectibles ??= new System.Collections.Generic.List<string>();
            context.rareEventsSeen ??= new System.Collections.Generic.List<string>();
            context.familyLineage ??= new FamilyLineageState();

            SetText(progressionText,
                $"Skills C:{context.progression.GetSkillLevel("cooking")} F:{context.progression.GetSkillLevel("fishing")} Sur:{context.progression.GetSkillLevel("survival")} Ch:{context.progression.GetSkillLevel("charisma")} N:{context.progression.GetSkillLevel("negotiation")} | Perks {context.progression.unlockedPerkIds.Count} | Collectibles {context.collectibles.Count} | RareEvents {context.rareEventsSeen.Count} | Gen {context.familyLineage.generationIndex}");
        }

        private void OnConsumeClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _consumeUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnBuyClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _buyUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnTalkClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _talkUseCase.TalkToNpc(context, string.Empty, out var message);
            SetFeedback(ok, message);
        }

        private void OnHelpNpcClicked()
        {
            TriggerDrama(DramaEventType.HelpedNpc);
        }

        private void OnInsultNpcClicked()
        {
            TriggerDrama(DramaEventType.InsultedNpc);
        }

        private void OnStealFromShopClicked()
        {
            TriggerDrama(DramaEventType.StoleFromShop);
        }

        private void OnGossipClicked()
        {
            TriggerDrama(DramaEventType.Gossiped);
        }

        private void TriggerDrama(DramaEventType type)
        {
            var context = SessionContextRegistry.Current;
            var ok = _dramaUseCase.Execute(context, string.Empty, type, out var message);
            SetFeedback(ok, message);
        }

        private void OnTravelClicked(ZoneType zoneType)
        {
            var context = SessionContextRegistry.Current;
            var ok = _travelToZoneUseCase.Execute(context, zoneType, out var message);
            SetFeedback(ok, message);
        }

        private void OnGainCookingSkillClicked()
        {
            var context = SessionContextRegistry.Current;
            var skillId = ResolveSkillTrainingForZone(context?.currentZone ?? ZoneType.Home);
            var ok = _skillProgressionUseCase.GainXp(context, skillId, 12f, out var message);
            SetFeedback(ok, message);
        }

        private static string ResolveSkillTrainingForZone(ZoneType zone)
        {
            return zone switch
            {
                ZoneType.Lake => "fishing",
                ZoneType.Forest => "survival",
                ZoneType.TownCenter => "charisma",
                ZoneType.GasStation => "negotiation",
                _ => "cooking"
            };
        }

        private void OnUnlockPerkClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _perkUnlockUseCase.TryUnlock(context, "efficient_cook", "cooking", 2, out var message);
            SetFeedback(ok, message);
        }

        private void OnCollectRelicClicked()
        {
            var context = SessionContextRegistry.Current;
            var collectibleId = $"relic_{context.currentZone.ToString().ToLowerInvariant()}";
            var ok = _collectiblesUseCase.Collect(context, collectibleId, out var message);
            SetFeedback(ok, message);
        }

        private void OnRareEventClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _rareEventUseCase.TryTrigger(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnUpgradePropertyClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _propertyUpgradeUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnNextGenerationClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _advanceGenerationUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnCookClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _cookUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnCleanClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _cleanHomeUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnBuyFurnitureClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _buyFurnitureUseCase.Execute(context, out var message);
            SetFeedback(ok, message);
        }

        private void OnSleepClicked()
        {
            var context = SessionContextRegistry.Current;
            var ok = _sleepUseCase.Execute(context, out var message);
            SetFeedback(ok, message);

            if (ok)
            {
                _saveUseCase.Execute(context);
                SetFeedback(true, $"{message} Auto-saved.");
            }
        }

        private void OnSaveClicked()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                SetFeedback(false, "No active session.");
                return;
            }

            _saveUseCase.Execute(context);
            SetFeedback(true, "Session saved.");
        }

        private void OnReloadValidateClicked()
        {
            var ok = _reloadValidationUseCase.Execute(out var message);
            SetFeedback(ok, message);
        }

        private void SetFeedback(bool success, string message)
        {
            if (feedbackText == null) return;
            feedbackText.text = success ? $"OK: {message}" : $"WARN: {message}";
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
            {
                target.text = value;
            }
        }
    }
}
