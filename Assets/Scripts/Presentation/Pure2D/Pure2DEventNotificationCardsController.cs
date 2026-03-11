using System.Collections.Generic;
using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Lightweight notification stack for modern feed/dashboard micro-motion.
    /// </summary>
    public class Pure2DEventNotificationCardsController : MonoBehaviour
    {
        [SerializeField] private Text notificationText;
        [SerializeField] private float cardLifetimeSeconds = 4f;
        [SerializeField] private float pulseAmplitude = 0.035f;

        private readonly Queue<string> _queue = new();
        private string _lastWorldEvent = string.Empty;
        private string _lastNpcReaction = string.Empty;
        private float _timeRemaining;
        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            if (notificationText != null)
            {
                _baseScale = notificationText.rectTransform.localScale;
                notificationText.text = string.Empty;
            }
        }

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                return;
            }

            CaptureIncoming(context);
            TickCardMotion();
            TryShowNextCard();
        }

        private void CaptureIncoming(GameSessionContext context)
        {
            if (!string.IsNullOrWhiteSpace(context.lastWorldEvent) && context.lastWorldEvent != _lastWorldEvent)
            {
                _lastWorldEvent = context.lastWorldEvent;
                _queue.Enqueue($"🗞 {context.lastWorldEvent}");
            }

            if (!string.IsNullOrWhiteSpace(context.lastNpcReaction) && context.lastNpcReaction != _lastNpcReaction)
            {
                _lastNpcReaction = context.lastNpcReaction;
                _queue.Enqueue($"💬 {context.lastNpcReaction}");
            }
        }

        private void TickCardMotion()
        {
            if (notificationText == null || string.IsNullOrWhiteSpace(notificationText.text))
            {
                return;
            }

            _timeRemaining -= Time.unscaledDeltaTime;
            var pulse = 1f + (Mathf.Sin(Time.unscaledTime * 7f) * pulseAmplitude);
            notificationText.rectTransform.localScale = _baseScale * pulse;

            if (_timeRemaining <= 0f)
            {
                notificationText.text = string.Empty;
                notificationText.rectTransform.localScale = _baseScale;
            }
        }

        private void TryShowNextCard()
        {
            if (notificationText == null || !string.IsNullOrWhiteSpace(notificationText.text) || _queue.Count == 0)
            {
                return;
            }

            notificationText.text = _queue.Dequeue();
            _timeRemaining = Mathf.Max(1f, cardLifetimeSeconds);
        }
    }
}
