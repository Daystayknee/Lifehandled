using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lifehandled.Presentation.Pure2D.Avatar
{
    [CreateAssetMenu(menuName = "Lifehandled/Pure2D/Avatar Preset Catalog", fileName = "AvatarPresetCatalog")]
    public class AvatarPresetCatalog : ScriptableObject
    {
        [SerializeField] private List<AvatarPresetData> presets = new();

        public AvatarPresetData Resolve(string presetId)
        {
            if (string.IsNullOrWhiteSpace(presetId))
            {
                return presets.FirstOrDefault();
            }

            return presets.FirstOrDefault(p => p != null && p.presetId == presetId) ?? presets.FirstOrDefault();
        }
    }

    [Serializable]
    public class AvatarPresetData
    {
        public string presetId = "default";
        public string displayName = "Default";
        public string portraitStyleTag = "anime_semi_real";

        public string baseBodyPartId = "body_average";
        public string facePartId = "face_oval";
        public string eyePartId = "eyes_almond";
        public string nosePartId = "nose_straight";
        public string mouthPartId = "mouth_neutral";
        public string eyebrowPartId = "brow_standard";
        public string earPartId = "ears_default";
        public string hairBackPartId = "hair_back_medium";
        public string hairFrontPartId = "hair_front_default";
        public string upperWearPartId = "shirt_basic";
        public string lowerWearPartId = "pants_basic";
        public string footwearPartId = "sneakers_basic";
    }
}
