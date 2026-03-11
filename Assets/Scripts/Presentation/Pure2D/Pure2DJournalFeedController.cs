using System.Collections.Generic;
using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 1 pure 2D journal/feed panel.
    /// Builds a compact social-feed style timeline from runtime events.
    /// </summary>
    public class Pure2DJournalFeedController : MonoBehaviour
    {
        [SerializeField] private Text feedText;
        [SerializeField] private int maxEntries = 12;

        private readonly Queue<string> _entries = new();
        private int _lastObservedDay = -1;
        private string _lastObservedWorldEvent = string.Empty;
        private string _lastObservedNpcReaction = string.Empty;

        private void Update()
        {
            var context = SessionContextRegistry.Current;
            if (context == null)
            {
                return;
            }

            if (_lastObservedDay != context.currentDay)
            {
                _lastObservedDay = context.currentDay;
                AddEntry($"📅 Day {context.currentDay}: {context.dayOfWeekName} {context.monthName}-{context.dayOfMonth:00}");
            }

            if (!string.IsNullOrWhiteSpace(context.lastWorldEvent) && context.lastWorldEvent != _lastObservedWorldEvent)
            {
                _lastObservedWorldEvent = context.lastWorldEvent;
                AddEntry($"🗞 {context.lastWorldEvent}");
            }

            if (!string.IsNullOrWhiteSpace(context.lastNpcReaction) && context.lastNpcReaction != _lastObservedNpcReaction)
            {
                _lastObservedNpcReaction = context.lastNpcReaction;
                AddEntry($"💬 NPC: {context.lastNpcReaction}");
            }

            Render();
        }

        private void AddEntry(string entry)
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                return;
            }

            _entries.Enqueue(entry);
            while (_entries.Count > maxEntries)
            {
                _entries.Dequeue();
            }
        }

        private void Render()
        {
            if (feedText == null)
            {
                return;
            }

            feedText.text = _entries.Count == 0
                ? "Journal feed is empty."
                : string.Join("\n", _entries);
        }
    }
}
