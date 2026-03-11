using System;
using UnityEngine;

namespace Lifehandled.Presentation.PaperDoll
{
    [CreateAssetMenu(menuName = "Lifehandled/PaperDoll/Environment Profile", fileName = "EnvironmentProfile")]
    public class EnvironmentProfile : ScriptableObject
    {
        public EnvironmentType environmentType = EnvironmentType.Mall;
        public Sprite backgroundSprite;
        public EnvironmentStatModifier statModifier = new();
    }

    [Serializable]
    public class EnvironmentStatModifier
    {
        public float hungerDeltaPerMinute = 0.1f;
        public float socialEnergyDeltaPerMinute = -0.1f;
        public int moneyDeltaPerMinute;
    }

    public enum EnvironmentType
    {
        Mall = 0,
        Park = 1,
        Hospital = 2
    }
}
