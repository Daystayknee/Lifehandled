using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;

namespace Lifehandled.Domain.Character
{
    /// <summary>
    /// Seed-driven modular appearance profile so visuals can be deterministic across save/load.
    /// Includes evolving biology/condition state for lightweight life-sim progression.
    /// </summary>
    [Serializable]
    public class AppearanceProfile
    {
        public int appearanceSeed;

        // Face structure
        public FaceShapeType faceShape = FaceShapeType.Oval;
        public JawlineType jawline = JawlineType.Soft;
        public ChinType chin = ChinType.Rounded;

        // Eyes
        public EyeShapeType eyeShape = EyeShapeType.Almond;
        public EyeColorType eyeColor = EyeColorType.Brown;
        public bool hasHeterochromia;
        public bool hasEpicanthicFold;
        public bool hasHeavyLids;
        public bool hasDeepSetEyes;
        public bool hasLargeEyes;

        // Nose / mouth
        public NoseShapeType noseShape = NoseShapeType.Straight;
        public LipShapeType lipShape = LipShapeType.Full;
        public bool hasPlumpLips;
        public bool hasUnevenLips;
        public bool hasDownturnedLipCorners;
        public bool hasUpturnedLipCorners;

        // Skin features
        public List<string> skinFeatureIds = new();

        // Advanced skin variation
        public string skinUndertone = "neutral"; // cool/warm/neutral
        public float skinTexture01 = 0.5f;
        public float poreVisibility01 = 0.5f;
        public float wrinkleIntensity01 = 0.2f;
        public float sunDamage01 = 0.1f;
        public float tanLevel01 = 0.2f;
        public float scarVisibility01 = 0.1f;
        public float stretchMarks01 = 0.1f;

        // Skin conditions (dynamic)
        public float acne01 = 0.1f;
        public float rashes01 = 0.05f;
        public float dryness01 = 0.2f;
        public float sunburn01 = 0f;

        // Hair
        public HairType hairType = HairType.H2B;
        public HairLengthType hairLength = HairLengthType.Medium;
        public HairStyleType hairStyle = HairStyleType.Loose;
        public HairColorType hairColor = HairColorType.Brown;

        // Expanded hair traits
        public float hairThickness01 = 0.5f;
        public float hairDensity01 = 0.5f;
        public float hairGrowthSpeed01 = 0.5f;
        public string hairlineShapeId = "rounded";
        public float baldingPattern01 = 0f;
        public float grayingRate01 = 0.2f;

        // Body hair
        public float armHair01 = 0.4f;
        public float legHair01 = 0.5f;
        public float facialHair01 = 0.2f;
        public float chestHair01 = 0.2f;
        public float backHair01 = 0.1f;

        // Body morph sliders (0..1) and height in cm for prototype simplicity.
        public float bodyFat01 = 0.5f;
        public float muscleMass01 = 0.5f;
        public float frameSize01 = 0.5f;
        public float heightCm = 170f;

        public float shoulders01 = 0.5f;
        public float chest01 = 0.5f;
        public float arms01 = 0.5f;
        public float forearms01 = 0.5f;
        public float hands01 = 0.5f;
        public float fingers01 = 0.5f;
        public float neck01 = 0.5f;
        public float waist01 = 0.5f;
        public float stomach01 = 0.5f;
        public float back01 = 0.5f;
        public float hips01 = 0.5f;
        public float thighs01 = 0.5f;
        public float calves01 = 0.5f;
        public float ankles01 = 0.5f;
        public float feet01 = 0.5f;
        public float glutes01 = 0.5f;
        public float posture01 = 0.5f;
        public float limbLength01 = 0.5f;
        public bool hasDimples;

        // Dynamic body condition states
        public float weightShift01 = 0.5f;
        public float muscleGrowth01 = 0.5f;
        public float fatigue01 = 0.2f;
        public float dehydration01 = 0.2f;
        public float illness01 = 0.05f;
        public float injury01 = 0f;

        // Biological tendency traits (baseline, influenced by genetics)
        public float metabolismSpeed01 = 0.5f;
        public float staminaCapacity01 = 0.5f;
        public float immuneStrength01 = 0.5f;
        public float painTolerance01 = 0.5f;
        public float sleepQualityTendency01 = 0.5f;
        public float stressTolerance01 = 0.5f;
        public float agingRate01 = 0.5f;

        // Clothing slots
        public string upperWearId = "shirt_basic";
        public string lowerWearId = "pants_basic";
        public string footwearId = "sneakers_basic";
        public List<string> accessoryIds = new();

        // Compatibility aliases retained from older saves/logic.
        public string bodyFrame = "Average";
        public int skinToneIndex;
        public string hairStyleId = "default_hair";
        public string hairColorId = "default_hair_color";
        public string eyeColorId = "default_eye_color";
        public List<string> distinctiveFeatureIds = new();
        public string outfitPresetId = "default_outfit";
    }
}
