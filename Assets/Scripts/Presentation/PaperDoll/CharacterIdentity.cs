using System;
using UnityEngine;

namespace Lifehandled.Presentation.PaperDoll
{
    /// <summary>
    /// ScriptableObject identity payload for a paper-doll life sim character.
    /// Keeps visuals + survival stats in a single save-friendly data asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Lifehandled/PaperDoll/Character Identity", fileName = "CharacterIdentity")]
    public class CharacterIdentity : ScriptableObject
    {
        [Header("Visual Layers")]
        public Sprite baseBody;
        public Sprite eyes;
        public Sprite nose;
        public Sprite lips;
        public Sprite makeup;
        public Sprite freckles;
        public Sprite beautyMarks;

        [Header("RPG Survival Stats")]
        [Range(0f, 100f)] public float hunger = 50f;
        [Range(0f, 100f)] public float socialEnergy = 50f;
        public int money = 40;

        [Header("Trait")]
        public IdentityTrait trait = IdentityTrait.Balanced;

        public float GetHungerDrainMultiplier()
        {
            return trait switch
            {
                IdentityTrait.HighMetabolism => 1.35f,
                IdentityTrait.LowMetabolism => 0.75f,
                _ => 1f
            };
        }

        public float GetSocialDrainMultiplier()
        {
            return trait switch
            {
                IdentityTrait.SocialButterfly => 0.75f,
                IdentityTrait.Introvert => 1.2f,
                _ => 1f
            };
        }

        public int GetMoneyDeltaPerDayBonus()
        {
            return trait switch
            {
                IdentityTrait.Frugal => 3,
                IdentityTrait.Splurger => -3,
                _ => 0
            };
        }

        public void ClampStats()
        {
            hunger = Mathf.Clamp(hunger, 0f, 100f);
            socialEnergy = Mathf.Clamp(socialEnergy, 0f, 100f);
        }
    }

    [Serializable]
    public enum IdentityTrait
    {
        Balanced = 0,
        HighMetabolism = 1,
        LowMetabolism = 2,
        SocialButterfly = 3,
        Introvert = 4,
        Frugal = 5,
        Splurger = 6
    }
}
