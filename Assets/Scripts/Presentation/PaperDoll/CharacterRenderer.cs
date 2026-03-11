using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lifehandled.Presentation.PaperDoll
{
    /// <summary>
    /// Runtime paper-doll visual mapper.
    /// Assign SpriteRenderers in the inspector and call UpdateVisuals(identity).
    /// </summary>
    public class CharacterRenderer : MonoBehaviour
    {
        [SerializeField] private CharacterIdentity defaultIdentity;
        [SerializeField] private List<LayerSlot> slots = new();

        private void Start()
        {
            if (defaultIdentity != null)
            {
                UpdateVisuals(defaultIdentity);
            }
        }

        public void UpdateVisuals(CharacterIdentity data)
        {
            if (data == null)
            {
                return;
            }

            SetSlot(LayerType.BaseBody, data.baseBody);
            SetSlot(LayerType.Eyes, data.eyes);
            SetSlot(LayerType.Nose, data.nose);
            SetSlot(LayerType.Lips, data.lips);
            SetSlot(LayerType.Makeup, data.makeup);
            SetSlot(LayerType.Freckles, data.freckles);
            SetSlot(LayerType.BeautyMarks, data.beautyMarks);
        }

        private void SetSlot(LayerType type, Sprite sprite)
        {
            var slot = slots.Find(s => s.layerType == type);
            if (slot == null || slot.renderer == null)
            {
                return;
            }

            slot.renderer.sprite = sprite;
            slot.renderer.enabled = sprite != null;
        }
    }

    [Serializable]
    public class LayerSlot
    {
        public LayerType layerType;
        public SpriteRenderer renderer;
    }

    public enum LayerType
    {
        BaseBody = 0,
        Eyes = 1,
        Nose = 2,
        Lips = 3,
        Makeup = 4,
        Freckles = 5,
        BeautyMarks = 6
    }
}
