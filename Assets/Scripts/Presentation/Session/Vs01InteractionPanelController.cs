using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Gameplay;
using Lifehandled.Application.UseCases.LifeSim;
using Lifehandled.Application.UseCases.NPC;
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
        [SerializeField] private Text feedbackText;

        private readonly ConsumeStarterItemUseCase _consumeUseCase = new();
        private readonly BuyStarterItemUseCase _buyUseCase = new();
        private readonly NpcSocialInteractionUseCase _talkUseCase = new();
        private readonly CookSimpleMealUseCase _cookUseCase = new();
        private readonly CleanHomeUseCase _cleanHomeUseCase = new();
        private readonly BuyFurnitureUseCase _buyFurnitureUseCase = new();
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
                return;
            }

            context.economy ??= new EconomyState();
            context.home ??= new HomeLifeState();

            SetText(inventoryText,
                $"Water: {context.inventory.GetCount(ConsumeStarterItemUseCase.WaterBottleId)} | StaleFood: {context.inventory.GetCount(ConsumeStarterItemUseCase.BadFoodId)}");
            SetText(walletText, $"Wallet: ${context.wallet}");
            SetText(dayText, $"Day: {context.currentDay}");

            var playerName = context.playerCharacter?.data?.displayName ?? "(none)";
            SetText(contextText, $"Player: {playerName} | HouseholdMembers: {context.householdMembers.Count} | TalksToday: {context.talkCountToday}");

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
            SetText(npcText, $"NPC: {npcName} | Mood: {npcMood} | Time: {context.hourOfDay:00.0}");
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
