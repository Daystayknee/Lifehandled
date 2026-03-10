using Lifehandled.Application.Session;
using Lifehandled.Application.UseCases.Gameplay;
using Lifehandled.Infrastructure.Persistence.Stores;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Session
{
    /// <summary>
    /// Minimal interaction panel for VS01 Batch 3+4:
    /// consume, buy, and save buttons with simple text feedback.
    /// </summary>
    public class Vs01InteractionPanelController : MonoBehaviour
    {
        [SerializeField] private Button consumeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button saveButton;

        [SerializeField] private Text inventoryText;
        [SerializeField] private Text walletText;
        [SerializeField] private Text feedbackText;

        private readonly ConsumeStarterItemUseCase _consumeUseCase = new();
        private readonly BuyStarterItemUseCase _buyUseCase = new();
        private SaveCurrentSessionUseCase _saveUseCase;

        private void Awake()
        {
            _saveUseCase = new SaveCurrentSessionUseCase(new JsonNewGameSaveStore());

            if (consumeButton != null) consumeButton.onClick.AddListener(OnConsumeClicked);
            if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
        }

        private void OnDestroy()
        {
            if (consumeButton != null) consumeButton.onClick.RemoveListener(OnConsumeClicked);
            if (buyButton != null) buyButton.onClick.RemoveListener(OnBuyClicked);
            if (saveButton != null) saveButton.onClick.RemoveListener(OnSaveClicked);
        }

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                SetText(inventoryText, "Inventory: -");
                SetText(walletText, "Wallet: -");
                return;
            }

            SetText(inventoryText, $"Water: {context.inventory.GetCount(ConsumeStarterItemUseCase.WaterBottleId)}");
            SetText(walletText, $"Wallet: ${context.wallet}");
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
