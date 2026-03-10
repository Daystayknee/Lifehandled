using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Gameplay;
using Lifehandled.Infrastructure.Persistence.Stores;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal interaction panel for VS01:
    /// consume, buy, sleep/end-day, save, and reload validation.
    /// </summary>
    public class Vs01InteractionPanelController : MonoBehaviour
    {
        [SerializeField] private Button consumeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button sleepButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button reloadValidateButton;

        [SerializeField] private Text inventoryText;
        [SerializeField] private Text walletText;
        [SerializeField] private Text dayText;
        [SerializeField] private Text contextText;
        [SerializeField] private Text shopText;
        [SerializeField] private Text feedbackText;

        private readonly ConsumeStarterItemUseCase _consumeUseCase = new();
        private readonly BuyStarterItemUseCase _buyUseCase = new();
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
            if (sleepButton != null) sleepButton.onClick.AddListener(OnSleepClicked);
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
            if (reloadValidateButton != null) reloadValidateButton.onClick.AddListener(OnReloadValidateClicked);
        }

        private void OnDestroy()
        {
            if (consumeButton != null) consumeButton.onClick.RemoveListener(OnConsumeClicked);
            if (buyButton != null) buyButton.onClick.RemoveListener(OnBuyClicked);
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
                return;
            }

            SetText(inventoryText,
                $"Water: {context.inventory.GetCount(ConsumeStarterItemUseCase.WaterBottleId)} | StaleFood: {context.inventory.GetCount(ConsumeStarterItemUseCase.BadFoodId)}");
            SetText(walletText, $"Wallet: ${context.wallet}");
            SetText(dayText, $"Day: {context.currentDay}");

            var playerName = context.playerCharacter?.data?.displayName ?? "(none)";
            SetText(contextText, $"Player: {playerName} | HouseholdMembers: {context.householdMembers.Count}");

            var price = BuyStarterItemUseCase.ResolvePrice(context.foodPriceMultiplier);
            SetText(shopText, $"Shop: {(context.shopOpen ? "OPEN" : "CLOSED")} | Water Price: ${price}");
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
