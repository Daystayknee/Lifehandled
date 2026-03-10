using System;
using System.Collections.Generic;
using System.Linq;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;
using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.Content
{
    public static class PrototypeWorldContentCatalog
    {
        public static readonly List<string> FoodItemIds = new()
        {
            "apple", "banana", "berries", "bread", "cheese", "cooked_fish", "stew", "soup", "rice", "beans",
            "potato", "carrot", "corn", "tomato", "lettuce", "mushroom", "egg", "milk", "yogurt", "sandwich",
            "jerky", "trail_mix", "oatmeal", "pasta", "meat_pie", "grilled_meat", "orange", "pear", "peach", "plum",
            "pumpkin_soup", "herb_salad", "fried_rice", "curry", "pancakes", "waffle", "pickles", "seaweed", "nuts", "honey",
            "jam_toast", "noodle_bowl", "baked_potato", "fish_taco", "fruit_salad", "lentil_stew", "spiced_tea_snack", "wild_roots", "cabbage_roll", "tofu_bowl"
        };

        public static readonly List<string> ToolItemIds = new()
        {
            "axe", "pickaxe", "fishing_rod", "hammer", "wrench", "screwdriver", "shovel", "hoe", "watering_can", "lantern",
            "flashlight", "binoculars", "rope", "bucket", "net", "medkit", "thermometer", "stethoscope", "multitool", "cooking_pan",
            "knife", "saw", "pliers", "tape", "drill", "compass", "map_kit", "radio", "lockpick_set", "repair_kit"
        };

        public static readonly List<string> CollectibleIds = new()
        {
            "town_badge_01", "town_badge_02", "town_badge_03", "town_badge_04", "town_badge_05", "forest_totem_01", "forest_totem_02", "forest_totem_03", "forest_totem_04", "forest_totem_05",
            "lake_shell_01", "lake_shell_02", "lake_shell_03", "lake_shell_04", "lake_shell_05", "clinic_token_01", "clinic_token_02", "clinic_token_03", "clinic_token_04", "clinic_token_05",
            "apartment_keychain_01", "apartment_keychain_02", "apartment_keychain_03", "apartment_keychain_04", "apartment_keychain_05", "station_patch_01", "station_patch_02", "station_patch_03", "station_patch_04", "station_patch_05",
            "special_relic_01", "special_relic_02", "special_relic_03", "special_relic_04", "special_relic_05", "legacy_photo_01", "legacy_photo_02", "legacy_photo_03", "legacy_photo_04", "legacy_photo_05"
        };

        public static readonly List<string> GlobalEventIds = new()
        {
            "storms", "rumors", "break_ins", "social_drama", "illness_outbreaks"
        };

        public static readonly List<FoodDefinition> FoodDefinitions = BuildFoodDefinitions();

        public static readonly List<ClothingDefinition> ClothingDefinitions = new()
        {
            new("shirt_basic", ClothingCategoryType.UpperWear, warmth: 4f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 1f),
            new("jacket_denim", ClothingCategoryType.UpperWear, warmth: 8f, social: 2f, jobEligibility: 1f, attractiveness: 2f, weatherResistance: 3f),
            new("sweater_wool", ClothingCategoryType.UpperWear, warmth: 10f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 4f),
            new("coat_rain", ClothingCategoryType.UpperWear, warmth: 7f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 8f),
            new("pants_basic", ClothingCategoryType.LowerWear, warmth: 3f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 1f),
            new("skirt_basic", ClothingCategoryType.LowerWear, warmth: 2f, social: 2f, jobEligibility: 1f, attractiveness: 2f, weatherResistance: 1f),
            new("shorts_basic", ClothingCategoryType.LowerWear, warmth: 1f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 0.5f),
            new("leggings_basic", ClothingCategoryType.LowerWear, warmth: 4f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 2f),
            new("sneakers_basic", ClothingCategoryType.Footwear, warmth: 2f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 1f),
            new("boots_work", ClothingCategoryType.Footwear, warmth: 5f, social: 1f, jobEligibility: 2f, attractiveness: 1f, weatherResistance: 5f),
            new("sandals_basic", ClothingCategoryType.Footwear, warmth: 0.5f, social: 1f, jobEligibility: 0.5f, attractiveness: 1f, weatherResistance: 0.5f),
            new("glasses_round", ClothingCategoryType.Accessory, warmth: 0f, social: 1f, jobEligibility: 1f, attractiveness: 1f, weatherResistance: 0f),
            new("watch_basic", ClothingCategoryType.Accessory, warmth: 0f, social: 1f, jobEligibility: 2f, attractiveness: 0.5f, weatherResistance: 0f)
        };

        public static List<Vs01NpcState> CreateNpcRoster()
        {
            var roster = new List<Vs01NpcState>();

            for (var i = 1; i <= 20; i++)
            {
                roster.Add(new Vs01NpcState
                {
                    npcId = $"npc_citizen_{i:00}",
                    displayName = $"Citizen {i:00}",
                    personalityTraits = { i % 2 == 0 ? PersonalityTraitType.Kind : PersonalityTraitType.Practical },
                    friendship = 25f,
                    trust = 22f,
                    respect = 24f
                });
            }

            for (var i = 1; i <= 10; i++)
            {
                roster.Add(new Vs01NpcState
                {
                    npcId = $"npc_shopkeeper_{i:00}",
                    displayName = $"Shopkeeper {i:00}",
                    personalityTraits = { PersonalityTraitType.Practical, PersonalityTraitType.Reserved },
                    friendship = 20f,
                    trust = 28f,
                    respect = 35f
                });
            }

            for (var i = 1; i <= 5; i++)
            {
                roster.Add(new Vs01NpcState
                {
                    npcId = $"npc_special_{i:00}",
                    displayName = $"Special {i:00}",
                    personalityTraits = { PersonalityTraitType.Outgoing, PersonalityTraitType.Anxious },
                    friendship = 30f,
                    trust = 30f,
                    respect = 30f,
                    attraction = 10f
                });
            }

            return roster;
        }

        public static AppearanceProfile CreateAppearanceFromSeed(int appearanceSeed, bool isNpc)
        {
            var random = new Random(appearanceSeed);
            var profile = new AppearanceProfile
            {
                appearanceSeed = appearanceSeed,
                faceShape = PickEnum<FaceShapeType>(random),
                jawline = PickEnum<JawlineType>(random),
                chin = PickEnum<ChinType>(random),
                eyeShape = PickEnum<EyeShapeType>(random),
                eyeColor = PickEnum<EyeColorType>(random),
                noseShape = PickEnum<NoseShapeType>(random),
                lipShape = PickEnum<LipShapeType>(random),
                hairType = PickEnum<HairType>(random),
                hairLength = PickEnum<HairLengthType>(random),
                hairStyle = PickEnum<HairStyleType>(random),
                hairColor = PickEnum<HairColorType>(random),
                hasHeterochromia = random.NextDouble() < 0.03,
                hasEpicanthicFold = random.NextDouble() < 0.2,
                hasHeavyLids = random.NextDouble() < 0.35,
                hasDeepSetEyes = random.NextDouble() < 0.3,
                hasLargeEyes = random.NextDouble() < 0.4,
                hasPlumpLips = random.NextDouble() < 0.35,
                hasUnevenLips = random.NextDouble() < 0.12,
                hasDownturnedLipCorners = random.NextDouble() < 0.2,
                hasUpturnedLipCorners = random.NextDouble() < 0.2,
                hasDimples = random.NextDouble() < 0.28,
                bodyFat01 = (float)random.NextDouble(),
                muscleMass01 = (float)random.NextDouble(),
                frameSize01 = (float)random.NextDouble(),
                heightCm = 150f + (float)random.NextDouble() * 50f,
                shoulders01 = (float)random.NextDouble(),
                chest01 = (float)random.NextDouble(),
                arms01 = (float)random.NextDouble(),
                forearms01 = (float)random.NextDouble(),
                hands01 = (float)random.NextDouble(),
                fingers01 = (float)random.NextDouble(),
                neck01 = (float)random.NextDouble(),
                waist01 = (float)random.NextDouble(),
                stomach01 = (float)random.NextDouble(),
                back01 = (float)random.NextDouble(),
                hips01 = (float)random.NextDouble(),
                thighs01 = (float)random.NextDouble(),
                calves01 = (float)random.NextDouble(),
                ankles01 = (float)random.NextDouble(),
                feet01 = (float)random.NextDouble(),
                glutes01 = (float)random.NextDouble(),
                posture01 = (float)random.NextDouble(),
                limbLength01 = (float)random.NextDouble(),
                upperWearId = random.NextDouble() < 0.5 ? "shirt_basic" : "jacket_denim",
                lowerWearId = random.NextDouble() < 0.2 ? "shorts_basic" : "pants_basic",
                footwearId = random.NextDouble() < 0.3 ? "boots_work" : "sneakers_basic"
            };

            if (random.NextDouble() < 0.22) profile.skinFeatureIds.Add("freckles");
            if (random.NextDouble() < 0.12) profile.skinFeatureIds.Add("moles");
            if (random.NextDouble() < 0.04) profile.skinFeatureIds.Add("vitiligo");
            if (random.NextDouble() < 0.06) profile.skinFeatureIds.Add("birthmark");
            if (random.NextDouble() < 0.1) profile.skinFeatureIds.Add("scar");
            if (random.NextDouble() < 0.08) profile.skinFeatureIds.Add("acne");
            if (random.NextDouble() < 0.18) profile.skinFeatureIds.Add("wrinkles");

            if (isNpc && random.NextDouble() < 0.35)
            {
                profile.accessoryIds.Add(random.NextDouble() < 0.5 ? "glasses_round" : "watch_basic");
            }

            profile.bodyFrame = profile.frameSize01 > 0.66f ? "Large" : profile.frameSize01 < 0.33f ? "Slim" : "Average";
            profile.hairStyleId = profile.hairStyle.ToString();
            profile.hairColorId = profile.hairColor.ToString();
            profile.eyeColorId = profile.eyeColor.ToString();
            profile.distinctiveFeatureIds = profile.skinFeatureIds.ToList();
            profile.outfitPresetId = $"{profile.upperWearId}_{profile.lowerWearId}_{profile.footwearId}";
            profile.skinToneIndex = random.Next(0, 8);

            return profile;
        }

        public static bool TryGetFoodDefinition(string itemId, out FoodDefinition definition)
        {
            definition = FoodDefinitions.FirstOrDefault(f => f.itemId == itemId);
            return definition != null;
        }

        private static T PickEnum<T>(Random random) where T : struct, Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(random.Next(values.Length));
        }

        private static List<FoodDefinition> BuildFoodDefinitions()
        {
            var defs = new List<FoodDefinition>
            {
                new("water_bottle", FoodCategoryType.BasicSurvival, hunger: 4f, hydration: 26f, mood: 0f, health: 2f, illnessRisk: -2f, energy: 1f),
                new("stale_food", FoodCategoryType.BasicSurvival, hunger: 15f, hydration: 0f, mood: -4f, health: -3f, illnessRisk: 8f, energy: -2f),
                new("simple_meal", FoodCategoryType.Prepared, hunger: 28f, hydration: 4f, mood: 4f, health: 3f, illnessRisk: -4f, energy: 3f)
            };

            for (var i = 0; i < FoodItemIds.Count; i++)
            {
                var itemId = FoodItemIds[i];
                if (defs.Any(d => d.itemId == itemId))
                {
                    continue;
                }

                var category = i < 20 ? FoodCategoryType.BasicSurvival : i < 38 ? FoodCategoryType.Prepared : FoodCategoryType.Luxury;
                var hunger = category == FoodCategoryType.BasicSurvival ? 11f : category == FoodCategoryType.Prepared ? 18f : 14f;
                var hydration = category == FoodCategoryType.BasicSurvival ? 3f : 2f;
                var mood = category == FoodCategoryType.Luxury ? 6f : category == FoodCategoryType.Prepared ? 3f : 1f;
                var health = itemId.Contains("fried") ? -1f : 2f;
                var illnessRisk = itemId.Contains("wild") ? 2f : -1f;
                var energy = category == FoodCategoryType.Prepared ? 4f : 2f;

                defs.Add(new FoodDefinition(itemId, category, hunger, hydration, mood, health, illnessRisk, energy));
            }

            return defs;
        }
    }

    public class FoodDefinition
    {
        public FoodDefinition(string itemId, FoodCategoryType category, float hunger, float hydration, float mood, float health, float illnessRisk, float energy)
        {
            this.itemId = itemId;
            this.category = category;
            hungerRestore = hunger;
            hydrationRestore = hydration;
            moodDelta = mood;
            healthDelta = health;
            illnessRiskDelta = illnessRisk;
            energyDelta = energy;
        }

        public string itemId { get; }
        public FoodCategoryType category { get; }
        public float hungerRestore { get; }
        public float hydrationRestore { get; }
        public float moodDelta { get; }
        public float healthDelta { get; }
        public float illnessRiskDelta { get; }
        public float energyDelta { get; }
    }

    public class ClothingDefinition
    {
        public ClothingDefinition(string itemId, ClothingCategoryType category, float warmth, float social, float jobEligibility, float attractiveness, float weatherResistance)
        {
            this.itemId = itemId;
            this.category = category;
            warmthBonus = warmth;
            socialReputationBonus = social;
            jobEligibilityScore = jobEligibility;
            attractivenessBonus = attractiveness;
            weatherResistanceBonus = weatherResistance;
        }

        public string itemId { get; }
        public ClothingCategoryType category { get; }
        public float warmthBonus { get; }
        public float socialReputationBonus { get; }
        public float jobEligibilityScore { get; }
        public float attractivenessBonus { get; }
        public float weatherResistanceBonus { get; }
    }
}
