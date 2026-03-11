using Lifehandled.Domain.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Lifehandled.Presentation.Pure2D.Avatar
{
    /// <summary>
    /// Phase 2 layered portrait renderer.
    /// Resolves sprite parts from appearance data and applies them to UI layers.
    /// </summary>
    public class LayeredPortraitRenderer : MonoBehaviour
    {
        [SerializeField] private AvatarPartLibrary partLibrary;
        [SerializeField] private AvatarPresetCatalog presetCatalog;
        [SerializeField] private string fallbackPresetId = "default";

        [Header("UI Layers")]
        [SerializeField] private Image baseBodyLayer;
        [SerializeField] private Image faceLayer;
        [SerializeField] private Image eyeLayer;
        [SerializeField] private Image noseLayer;
        [SerializeField] private Image mouthLayer;
        [SerializeField] private Image eyebrowLayer;
        [SerializeField] private Image earLayer;
        [SerializeField] private Image hairBackLayer;
        [SerializeField] private Image hairFrontLayer;
        [SerializeField] private Image clothingUpperLayer;
        [SerializeField] private Image clothingLowerLayer;
        [SerializeField] private Image footwearLayer;

        public void Render(AppearanceProfile appearance)
        {
            if (appearance == null)
            {
                return;
            }

            var selection = ResolveSelection(appearance);
            Apply(baseBodyLayer, selection.baseBodyPartId);
            Apply(faceLayer, selection.facePartId);
            Apply(eyeLayer, selection.eyePartId);
            Apply(noseLayer, selection.nosePartId);
            Apply(mouthLayer, selection.mouthPartId);
            Apply(eyebrowLayer, selection.eyebrowPartId);
            Apply(earLayer, selection.earPartId);
            Apply(hairBackLayer, selection.hairBackPartId);
            Apply(hairFrontLayer, selection.hairFrontPartId);
            Apply(clothingUpperLayer, selection.upperWearPartId);
            Apply(clothingLowerLayer, selection.lowerWearPartId);
            Apply(footwearLayer, selection.footwearPartId);
        }

        private AvatarPartSelection ResolveSelection(AppearanceProfile appearance)
        {
            var preset = presetCatalog != null
                ? presetCatalog.Resolve(fallbackPresetId)
                : null;

            return new AvatarPartSelection
            {
                baseBodyPartId = SelectBodyPart(appearance, preset),
                facePartId = SelectFacePart(appearance, preset),
                eyePartId = SelectEyePart(appearance, preset),
                nosePartId = SelectNosePart(appearance, preset),
                mouthPartId = SelectMouthPart(appearance, preset),
                eyebrowPartId = SelectEyebrowPart(appearance, preset),
                earPartId = SelectEarPart(appearance, preset),
                hairBackPartId = SelectHairBackPart(appearance, preset),
                hairFrontPartId = SelectHairFrontPart(appearance, preset),
                upperWearPartId = string.IsNullOrWhiteSpace(appearance.upperWearId) ? preset?.upperWearPartId : appearance.upperWearId,
                lowerWearPartId = string.IsNullOrWhiteSpace(appearance.lowerWearId) ? preset?.lowerWearPartId : appearance.lowerWearId,
                footwearPartId = string.IsNullOrWhiteSpace(appearance.footwearId) ? preset?.footwearPartId : appearance.footwearId
            };
        }

        private static string SelectBodyPart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            if (appearance.bodyFat01 > 0.67f) return "body_curvy";
            if (appearance.muscleMass01 > 0.67f) return "body_athletic";
            return preset?.baseBodyPartId ?? "body_average";
        }

        private static string SelectFacePart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            return appearance.faceShape switch
            {
                Domain.Common.FaceShapeType.Round => "face_round",
                Domain.Common.FaceShapeType.Square => "face_square",
                Domain.Common.FaceShapeType.Heart => "face_heart",
                Domain.Common.FaceShapeType.Diamond => "face_diamond",
                _ => preset?.facePartId ?? "face_oval"
            };
        }

        private static string SelectEyePart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            return appearance.eyeShape switch
            {
                Domain.Common.EyeShapeType.Round => "eyes_round",
                Domain.Common.EyeShapeType.Upturned => "eyes_upturned",
                Domain.Common.EyeShapeType.Downturned => "eyes_downturned",
                Domain.Common.EyeShapeType.Monolid => "eyes_monolid",
                _ => preset?.eyePartId ?? "eyes_almond"
            };
        }

        private static string SelectNosePart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            return appearance.noseShape switch
            {
                Domain.Common.NoseShapeType.Button => "nose_button",
                Domain.Common.NoseShapeType.Roman => "nose_roman",
                Domain.Common.NoseShapeType.Hooked => "nose_hooked",
                _ => preset?.nosePartId ?? "nose_straight"
            };
        }

        private static string SelectMouthPart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            if (appearance.hasPlumpLips)
            {
                return "mouth_plump";
            }

            return preset?.mouthPartId ?? "mouth_neutral";
        }

        private static string SelectEyebrowPart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            if (appearance.hasHeavyLids)
            {
                return "brow_heavy";
            }

            return preset?.eyebrowPartId ?? "brow_standard";
        }

        private static string SelectEarPart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            if (appearance.hasDimples)
            {
                return "ears_soft";
            }

            return preset?.earPartId ?? "ears_default";
        }

        private static string SelectHairBackPart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            if (appearance.hairLength == Domain.Common.HairLengthType.Short)
            {
                return "hair_back_short";
            }

            if (appearance.hairLength == Domain.Common.HairLengthType.Long)
            {
                return "hair_back_long";
            }

            return preset?.hairBackPartId ?? "hair_back_medium";
        }

        private static string SelectHairFrontPart(AppearanceProfile appearance, AvatarPresetData preset)
        {
            return appearance.hairType switch
            {
                Domain.Common.HairType.H1A or Domain.Common.HairType.H1B or Domain.Common.HairType.H1C => "hair_front_straight",
                Domain.Common.HairType.H2A or Domain.Common.HairType.H2B or Domain.Common.HairType.H2C => "hair_front_wavy",
                Domain.Common.HairType.H3A or Domain.Common.HairType.H3B or Domain.Common.HairType.H3C => "hair_front_curly",
                Domain.Common.HairType.H4A or Domain.Common.HairType.H4B or Domain.Common.HairType.H4C => "hair_front_coily",
                _ => preset?.hairFrontPartId ?? "hair_front_default"
            };
        }

        private void Apply(Image layer, string partId)
        {
            if (layer == null)
            {
                return;
            }

            if (partLibrary == null || string.IsNullOrWhiteSpace(partId))
            {
                layer.enabled = false;
                return;
            }

            var sprite = partLibrary.Resolve(partId);
            layer.sprite = sprite;
            layer.enabled = sprite != null;
        }
    }

    public class AvatarPartSelection
    {
        public string baseBodyPartId;
        public string facePartId;
        public string eyePartId;
        public string nosePartId;
        public string mouthPartId;
        public string eyebrowPartId;
        public string earPartId;
        public string hairBackPartId;
        public string hairFrontPartId;
        public string upperWearPartId;
        public string lowerWearPartId;
        public string footwearPartId;
    }
}
