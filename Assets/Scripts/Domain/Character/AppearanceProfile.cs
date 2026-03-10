using System;
using System.Collections.Generic;

namespace Lifehandled.Domain.Character
{
    /// <summary>
    /// Seed-driven appearance profile so visuals can be deterministic across save/load.
    /// </summary>
    [Serializable]
    public class AppearanceProfile
    {
        public int appearanceSeed;
        public string bodyFrame = "Average";
        public int skinToneIndex;
        public string hairStyleId = "default_hair";
        public string hairColorId = "default_hair_color";
        public string eyeColorId = "default_eye_color";
        public List<string> distinctiveFeatureIds = new();
        public string outfitPresetId = "default_outfit";
    }
}
