using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lifehandled.Presentation.Pure2D
{
    [CreateAssetMenu(menuName = "Lifehandled/Pure2D/Icon Registry", fileName = "Pure2DIconRegistry")]
    public class Pure2DIconRegistry : ScriptableObject
    {
        [SerializeField] private List<IconEntry> icons = new();
        private Dictionary<string, Sprite> _lookup;

        public Sprite Resolve(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            _lookup ??= BuildLookup();
            return _lookup.TryGetValue(key, out var sprite) ? sprite : null;
        }

        private Dictionary<string, Sprite> BuildLookup()
        {
            var dict = new Dictionary<string, Sprite>();
            foreach (var icon in icons)
            {
                if (icon == null || string.IsNullOrWhiteSpace(icon.key))
                {
                    continue;
                }

                dict[icon.key] = icon.sprite;
            }

            return dict;
        }
    }

    [Serializable]
    public class IconEntry
    {
        public string key;
        public Sprite sprite;
    }
}
