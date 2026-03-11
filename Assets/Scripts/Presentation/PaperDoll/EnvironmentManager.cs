using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.PaperDoll
{
    /// <summary>
    /// Handles active environment switching and broadcasts stat modifiers.
    /// </summary>
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private List<EnvironmentProfile> environments = new();
        [SerializeField] private Image backgroundImage;
        [SerializeField] private EnvironmentType startEnvironment = EnvironmentType.Mall;

        public EnvironmentProfile ActiveEnvironment { get; private set; }

        private void Start()
        {
            SwitchEnvironment(startEnvironment);
        }

        public void SwitchEnvironment(EnvironmentType type)
        {
            var profile = environments.Find(e => e != null && e.environmentType == type);
            if (profile == null)
            {
                return;
            }

            ActiveEnvironment = profile;
            if (backgroundImage != null)
            {
                backgroundImage.sprite = profile.backgroundSprite;
                backgroundImage.enabled = profile.backgroundSprite != null;
            }
        }
    }
}
