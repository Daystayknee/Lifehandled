using System.Collections.Generic;
using Lifehandled.Application.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Social-media-like life journal feed.
    /// Cards stack vertically and read like a town timeline.
    /// </summary>
    public class Pure2DJournalFeedController : MonoBehaviour
    {
        [SerializeField] private Text feedText;
        [SerializeField] private int maxEntries = 14;

        private readonly Queue<string> _entries = new();
        private int _lastObservedDay = -1;
        private string _lastObservedWorldEvent = string.Empty;
        private string _lastObservedNpcReaction = string.Empty;
        private string _lastObservedEconomyEvent = string.Empty;
        private int _lastPlayerPostDay = -1;

        private void Start()
        {
            AddEntry("📌 Town Hall posted:\n\"Festival tomorrow 🎉\"");
        }

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
                AddEntry($"📅 Day {context.currentDay} — {context.monthName} {context.dayOfMonth:00}");
            }

            if (!string.IsNullOrWhiteSpace(context.lastWorldEvent) && context.lastWorldEvent != _lastObservedWorldEvent)
            {
                _lastObservedWorldEvent = context.lastWorldEvent;
                AddEntry(BuildWorldPost(context.lastWorldEvent));
            }

            if (!string.IsNullOrWhiteSpace(context.lastNpcReaction) && context.lastNpcReaction != _lastObservedNpcReaction)
            {
                _lastObservedNpcReaction = context.lastNpcReaction;
                AddEntry($"🧑 NPC posted:\n\"{context.lastNpcReaction}\"");
            }

            if (!string.IsNullOrWhiteSpace(context.lastEconomyEvent) && context.lastEconomyEvent != _lastObservedEconomyEvent)
            {
                _lastObservedEconomyEvent = context.lastEconomyEvent;
                AddEntry($"🏛 Town Hall posted:\n\"{context.lastEconomyEvent}\"");
            }

            if (_lastPlayerPostDay != context.currentDay)
            {
                _lastPlayerPostDay = context.currentDay;
                AddPlayerLifeActionPost(context);
            }
            Render();
        }

        private void AddPlayerLifeActionPost(GameSessionContext context)
        {
            var mood = context.playerCharacter?.needsStatus?.mood ?? 50f;
            var entry = mood >= 65f
                ? "You:\nCooked fish stew (+mood) 🍲"
                : mood < 35f
                    ? "You:\nBarely made it through the day 😮‍💨"
                    : "You:\nManaged the day one step at a time.";

            AddEntry(entry);
        }

        private static string BuildWorldPost(string worldEvent)
        {
            if (worldEvent.Contains("Storm") || worldEvent.Contains("storm") || worldEvent.Contains("Rain") || worldEvent.Contains("rain"))
            {
                return "🌧️ Storm Today\nMaya posted:\n\"The rain destroyed the crops 😭\"";
            }

            return $"🗞 Town feed:\n{worldEvent}";
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
                : string.Join("\n\n────────────\n\n", _entries);
        }
    }
}
