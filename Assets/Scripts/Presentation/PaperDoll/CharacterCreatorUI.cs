using System.Collections.Generic;
using UnityEngine;

namespace Lifehandled.Presentation.PaperDoll
{
    /// <summary>
    /// Simple creator controls for cycling features and toggling detail layers.
    /// Hook these methods to UI buttons.
    /// </summary>
    public class CharacterCreatorUI : MonoBehaviour
    {
        [SerializeField] private CharacterIdentity characterIdentity;
        [SerializeField] private CharacterRenderer characterRenderer;

        [Header("Selectable variants")]
        [SerializeField] private List<Sprite> eyeVariants = new();
        [SerializeField] private List<Sprite> noseVariants = new();
        [SerializeField] private Sprite frecklesSprite;
        [SerializeField] private Sprite beautyMarksSprite;

        private int _eyesIndex;
        private int _noseIndex;

        public void CycleEyes()
        {
            if (characterIdentity == null || eyeVariants.Count == 0)
            {
                return;
            }

            _eyesIndex = (_eyesIndex + 1) % eyeVariants.Count;
            characterIdentity.eyes = eyeVariants[_eyesIndex];
            RefreshPreview();
        }

        public void CycleNose()
        {
            if (characterIdentity == null || noseVariants.Count == 0)
            {
                return;
            }

            _noseIndex = (_noseIndex + 1) % noseVariants.Count;
            characterIdentity.nose = noseVariants[_noseIndex];
            RefreshPreview();
        }

        public void ToggleFreckles()
        {
            if (characterIdentity == null)
            {
                return;
            }

            characterIdentity.freckles = characterIdentity.freckles == null ? frecklesSprite : null;
            RefreshPreview();
        }

        public void ToggleBeautyMarks()
        {
            if (characterIdentity == null)
            {
                return;
            }

            characterIdentity.beautyMarks = characterIdentity.beautyMarks == null ? beautyMarksSprite : null;
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            if (characterRenderer != null)
            {
                characterRenderer.UpdateVisuals(characterIdentity);
            }
        }
    }
}
