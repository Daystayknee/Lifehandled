using UnityEngine;

namespace Lifehandled.Presentation.Pure2D
{
    /// <summary>
    /// Phase 4 transition helper for panel fade in/out.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Pure2DPanelTransitionController : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private bool playFadeInOnEnable = true;

        private CanvasGroup _canvasGroup;
        private float _targetAlpha = 1f;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (_canvasGroup == null)
            {
                return;
            }

            if (playFadeInOnEnable)
            {
                _canvasGroup.alpha = 0f;
                _targetAlpha = 1f;
            }
        }

        private void Update()
        {
            if (_canvasGroup == null)
            {
                return;
            }

            var speed = fadeDuration <= 0.01f ? 100f : 1f / fadeDuration;
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, Time.unscaledDeltaTime * speed);
        }

        public void FadeOut()
        {
            _targetAlpha = 0f;
        }

        public void FadeIn()
        {
            _targetAlpha = 1f;
        }
    }
}
