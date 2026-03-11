using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lifehandled.Presentation.Pure2D.Avatar
{
    [CreateAssetMenu(menuName = "Lifehandled/Pure2D/Avatar Part Library", fileName = "AvatarPartLibrary")]
    public class AvatarPartLibrary : ScriptableObject
    {
        [SerializeField] private List<AvatarPartEntry> parts = new();
        private Dictionary<string, Sprite> _lookup;

        public Sprite Resolve(string partId)
        {
            if (string.IsNullOrWhiteSpace(partId))
            {
                return null;
            }

            _lookup ??= BuildLookup(parts);
            return _lookup.TryGetValue(partId, out var sprite) ? sprite : null;
        }

        private static Dictionary<string, Sprite> BuildLookup(List<AvatarPartEntry> source)
        {
            var result = new Dictionary<string, Sprite>();
            if (source == null)
            {
                return result;
            }

            foreach (var entry in source)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.partId))
                {
                    continue;
                }

                result[entry.partId] = entry.sprite;
            }

            return result;
        }
    }

    [Serializable]
    public class AvatarPartEntry
    {
        public string partId;
        public AvatarPartCategory category = AvatarPartCategory.Face;
        public Sprite sprite;
    }

    public enum AvatarPartCategory
    {
        BaseBody,
        Face,
        Eyes,
        Nose,
        Mouth,
        Eyebrows,
        Ears,
        HairBack,
        HairFront,
        ClothingUpper,
        ClothingLower,
        Footwear,
        Accessory,
        Overlay
    }
}
