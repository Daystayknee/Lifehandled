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
            "jam_toast", "noodle_bowl", "baked_potato", "fish_taco", "fruit_salad", "lentil_stew", "spiced_tea_snack", "wild_roots", "cabbage_roll", "tofu_bowl",
            "canned_beans", "canned_peaches", "canned_tuna", "dried_apricot", "granola_bar", "protein_biscuit", "herbal_broth", "tomato_bisque", "miso_soup", "beet_salad",
            "chicken_wrap", "veggie_wrap", "beef_stew", "seafood_pasta", "cheese_platter", "roasted_veggies", "fruit_tart", "cream_cake", "spice_cookie", "restaurant_ramen",
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

        public static readonly List<string> SocialEventIds = new()
        {
            "party", "festival", "market", "wedding", "funeral", "protest", "emergency"
        };

        public static readonly List<string> HobbyIds = new()
        {
            "painting", "gardening", "fishing", "gaming", "cooking", "reading", "working_out"
        };

        public static readonly List<string> BuildingIds = new()
        {
            "apartments", "houses", "shops", "restaurants", "workplaces"
        };

        public static readonly List<string> NatureRegionIds = new()
        {
            "forests", "rivers", "mountains", "beaches"
        };

        public static readonly List<string> InteractableObjectIds = new()
        {
            "vending_machine", "bench", "trash_can", "atm", "cooking_station"
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

        public static readonly List<AnimalDefinition> AnimalDefinitions = new()
        {
            new("deer", AnimalClassType.Wild, AnimalGameplayRoleType.HuntingResource, "forest", "meat_hide", 0.2f),
            new("rabbit", AnimalClassType.Wild, AnimalGameplayRoleType.HuntingResource, "forest", "meat_fur", 0.35f),
            new("bird", AnimalClassType.Wild, AnimalGameplayRoleType.EcosystemBehavior, "town_center", "feather", 0.65f),
            new("fish", AnimalClassType.Wild, AnimalGameplayRoleType.HuntingResource, "lake", "fish_meat", 0.6f),
            new("fox", AnimalClassType.Wild, AnimalGameplayRoleType.EcosystemBehavior, "forest", "rare_pelt", 0.12f),
            new("dog", AnimalClassType.Domestic, AnimalGameplayRoleType.Companionship, "apartments", "companionship", 0.7f),
            new("cat", AnimalClassType.Domestic, AnimalGameplayRoleType.Companionship, "apartments", "companionship", 0.72f),
            new("chicken", AnimalClassType.Domestic, AnimalGameplayRoleType.FarmingProduct, "workplace", "eggs", 0.55f),
            new("cow", AnimalClassType.Domestic, AnimalGameplayRoleType.FarmingProduct, "workplace", "milk", 0.45f),
            new("horse", AnimalClassType.Domestic, AnimalGameplayRoleType.Companionship, "town_center", "travel_bonus", 0.3f)
        };

        public static readonly List<NpcArchetypeDefinition> NpcArchetypes = BuildNpcArchetypes();

        public static List<Vs01NpcState> CreateNpcRoster()
        {
            var roster = new List<Vs01NpcState>();
            var archetypeCycle = NpcArchetypes.ToList();

            for (var i = 1; i <= 20; i++)
            {
                var archetype = archetypeCycle[(i - 1) % archetypeCycle.Count];
                roster.Add(BuildNpcFromArchetype($"npc_citizen_{i:00}", $"Citizen {i:00}", archetype));
            }

            var shopArchetypes = NpcArchetypes.Where(a =>
                    a.archetype == NpcArchetypeType.ShopOwner || a.archetype == NpcArchetypeType.Bartender || a.archetype == NpcArchetypeType.Mechanic || a.archetype == NpcArchetypeType.TravelingMerchant)
                .ToList();
            for (var i = 1; i <= 10; i++)
            {
                var archetype = shopArchetypes[(i - 1) % shopArchetypes.Count];
                roster.Add(BuildNpcFromArchetype($"npc_shopkeeper_{i:00}", $"Shopkeeper {i:00}", archetype));
            }

            var specialArchetypes = NpcArchetypes.Where(a =>
                    a.archetype == NpcArchetypeType.WealthyResident || a.archetype == NpcArchetypeType.MysteriousOutsider || a.archetype == NpcArchetypeType.TravelingMerchant)
                .ToList();
            for (var i = 1; i <= 5; i++)
            {
                var archetype = specialArchetypes[(i - 1) % specialArchetypes.Count];
                var npc = BuildNpcFromArchetype($"npc_special_{i:00}", $"Special {i:00}", archetype);
                npc.attraction = 10f;
                roster.Add(npc);
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
                skinUndertone = random.NextDouble() < 0.33 ? "cool" : random.NextDouble() < 0.5 ? "warm" : "neutral",
                skinTexture01 = (float)random.NextDouble(),
                poreVisibility01 = (float)random.NextDouble(),
                wrinkleIntensity01 = (float)random.NextDouble() * 0.4f,
                sunDamage01 = (float)random.NextDouble() * 0.35f,
                tanLevel01 = (float)random.NextDouble() * 0.5f,
                scarVisibility01 = (float)random.NextDouble() * 0.3f,
                stretchMarks01 = (float)random.NextDouble() * 0.3f,
                acne01 = (float)random.NextDouble() * 0.3f,
                rashes01 = (float)random.NextDouble() * 0.15f,
                dryness01 = (float)random.NextDouble() * 0.4f,
                sunburn01 = 0f,
                hairThickness01 = (float)random.NextDouble(),
                hairDensity01 = (float)random.NextDouble(),
                hairGrowthSpeed01 = (float)random.NextDouble(),
                hairlineShapeId = random.NextDouble() < 0.5 ? "rounded" : "m_shape",
                baldingPattern01 = (float)random.NextDouble() * 0.25f,
                grayingRate01 = (float)random.NextDouble() * 0.4f,
                armHair01 = (float)random.NextDouble(),
                legHair01 = (float)random.NextDouble(),
                facialHair01 = (float)random.NextDouble(),
                chestHair01 = (float)random.NextDouble(),
                backHair01 = (float)random.NextDouble(),
                weightShift01 = 0.5f,
                muscleGrowth01 = 0.5f,
                fatigue01 = 0.2f,
                dehydration01 = 0.2f,
                illness01 = 0.05f,
                injury01 = 0f,
                metabolismSpeed01 = (float)random.NextDouble(),
                staminaCapacity01 = (float)random.NextDouble(),
                immuneStrength01 = (float)random.NextDouble(),
                painTolerance01 = (float)random.NextDouble(),
                sleepQualityTendency01 = (float)random.NextDouble(),
                stressTolerance01 = (float)random.NextDouble(),
                agingRate01 = (float)random.NextDouble(),
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

        public static GeneticProfile BlendGeneticsForOffspring(GeneticProfile parentA, GeneticProfile parentB, string parentAId, string parentBId, int generationIndex, int seed)
        {
            parentA ??= new GeneticProfile();
            parentB ??= new GeneticProfile();
            var random = new Random(seed);

            float Read(GeneticProfile profile, string geneId)
            {
                var gene = profile.geneValues.FirstOrDefault(g => g.geneId == geneId);
                return gene?.value ?? 0.5f;
            }

            float Blend(string geneId)
            {
                var a = Read(parentA, geneId);
                var b = Read(parentB, geneId);
                var mid = (a + b) * 0.5f;
                var mutation = (float)(random.NextDouble() * 0.1 - 0.05);
                var value = mid + mutation;
                if (value < 0f) value = 0f;
                if (value > 1f) value = 1f;
                return value;
            }

            return new GeneticProfile
            {
                genomeVersion = 1,
                inheritance = new InheritanceMetadata
                {
                    isFounder = false,
                    generationIndex = generationIndex,
                    geneSource = GeneSourceType.Inherited
                },
                lineage = new LineageProfile
                {
                    parentACharacterId = parentAId ?? string.Empty,
                    parentBCharacterId = parentBId ?? string.Empty,
                    ancestorCharacterIds = new List<string> { parentAId ?? string.Empty, parentBId ?? string.Empty }
                },
                geneValues =
                {
                    new GeneValue { geneId = GeneticGeneIds.MetabolismRate, value = Blend(GeneticGeneIds.MetabolismRate) },
                    new GeneValue { geneId = GeneticGeneIds.StaminaEfficiency, value = Blend(GeneticGeneIds.StaminaEfficiency) },
                    new GeneValue { geneId = GeneticGeneIds.SleepRecoveryEfficiency, value = Blend(GeneticGeneIds.SleepRecoveryEfficiency) },
                    new GeneValue { geneId = GeneticGeneIds.StressSensitivity, value = Blend(GeneticGeneIds.StressSensitivity) },
                    new GeneValue { geneId = GeneticGeneIds.IllnessVulnerability, value = Blend(GeneticGeneIds.IllnessVulnerability) },
                    new GeneValue { geneId = GeneticGeneIds.ImmuneSystemStrength, value = Blend(GeneticGeneIds.ImmuneSystemStrength) },
                    new GeneValue { geneId = GeneticGeneIds.PainTolerance, value = Blend(GeneticGeneIds.PainTolerance) },
                    new GeneValue { geneId = GeneticGeneIds.SleepQualityTendency, value = Blend(GeneticGeneIds.SleepQualityTendency) },
                    new GeneValue { geneId = GeneticGeneIds.StressTolerance, value = Blend(GeneticGeneIds.StressTolerance) },
                    new GeneValue { geneId = GeneticGeneIds.AgingRate, value = Blend(GeneticGeneIds.AgingRate) },
                    new GeneValue { geneId = GeneticGeneIds.HairGrowthSpeed, value = Blend(GeneticGeneIds.HairGrowthSpeed) }
                }
            };
        }

        public static AppearanceProfile CreateInheritedAppearance(AppearanceProfile parentA, AppearanceProfile parentB, int seed)
        {
            parentA ??= CreateAppearanceFromSeed(seed + 11, isNpc: false);
            parentB ??= CreateAppearanceFromSeed(seed + 37, isNpc: false);
            var random = new Random(seed);

            var appearance = new AppearanceProfile
            {
                appearanceSeed = seed,
                faceShape = random.NextDouble() < 0.5 ? parentA.faceShape : parentB.faceShape,
                jawline = random.NextDouble() < 0.5 ? parentA.jawline : parentB.jawline,
                chin = random.NextDouble() < 0.5 ? parentA.chin : parentB.chin,
                eyeShape = random.NextDouble() < 0.5 ? parentA.eyeShape : parentB.eyeShape,
                eyeColor = random.NextDouble() < 0.5 ? parentA.eyeColor : parentB.eyeColor,
                hairType = random.NextDouble() < 0.5 ? parentA.hairType : parentB.hairType,
                hairLength = random.NextDouble() < 0.5 ? parentA.hairLength : parentB.hairLength,
                hairStyle = random.NextDouble() < 0.5 ? parentA.hairStyle : parentB.hairStyle,
                hairColor = random.NextDouble() < 0.5 ? parentA.hairColor : parentB.hairColor,
                noseShape = random.NextDouble() < 0.5 ? parentA.noseShape : parentB.noseShape,
                lipShape = random.NextDouble() < 0.5 ? parentA.lipShape : parentB.lipShape,
                frameSize01 = (parentA.frameSize01 + parentB.frameSize01) * 0.5f,
                heightCm = (parentA.heightCm + parentB.heightCm) * 0.5f + (float)(random.NextDouble() * 6d - 3d),
                upperWearId = parentA.upperWearId,
                lowerWearId = parentB.lowerWearId,
                footwearId = random.NextDouble() < 0.5 ? parentA.footwearId : parentB.footwearId,
                hasHeterochromia = parentA.hasHeterochromia || parentB.hasHeterochromia && random.NextDouble() < 0.5,
                hasDimples = parentA.hasDimples || parentB.hasDimples && random.NextDouble() < 0.55,
                skinUndertone = random.NextDouble() < 0.5 ? parentA.skinUndertone : parentB.skinUndertone,
                skinTexture01 = (parentA.skinTexture01 + parentB.skinTexture01) * 0.5f,
                poreVisibility01 = (parentA.poreVisibility01 + parentB.poreVisibility01) * 0.5f,
                wrinkleIntensity01 = (parentA.wrinkleIntensity01 + parentB.wrinkleIntensity01) * 0.5f,
                sunDamage01 = (parentA.sunDamage01 + parentB.sunDamage01) * 0.5f,
                tanLevel01 = (parentA.tanLevel01 + parentB.tanLevel01) * 0.5f,
                scarVisibility01 = (parentA.scarVisibility01 + parentB.scarVisibility01) * 0.5f,
                stretchMarks01 = (parentA.stretchMarks01 + parentB.stretchMarks01) * 0.5f,
                acne01 = (parentA.acne01 + parentB.acne01) * 0.5f,
                rashes01 = (parentA.rashes01 + parentB.rashes01) * 0.5f,
                dryness01 = (parentA.dryness01 + parentB.dryness01) * 0.5f,
                sunburn01 = 0f,
                hairThickness01 = (parentA.hairThickness01 + parentB.hairThickness01) * 0.5f,
                hairDensity01 = (parentA.hairDensity01 + parentB.hairDensity01) * 0.5f,
                hairGrowthSpeed01 = (parentA.hairGrowthSpeed01 + parentB.hairGrowthSpeed01) * 0.5f,
                hairlineShapeId = random.NextDouble() < 0.5 ? parentA.hairlineShapeId : parentB.hairlineShapeId,
                baldingPattern01 = (parentA.baldingPattern01 + parentB.baldingPattern01) * 0.5f,
                grayingRate01 = (parentA.grayingRate01 + parentB.grayingRate01) * 0.5f,
                armHair01 = (parentA.armHair01 + parentB.armHair01) * 0.5f,
                legHair01 = (parentA.legHair01 + parentB.legHair01) * 0.5f,
                facialHair01 = (parentA.facialHair01 + parentB.facialHair01) * 0.5f,
                chestHair01 = (parentA.chestHair01 + parentB.chestHair01) * 0.5f,
                backHair01 = (parentA.backHair01 + parentB.backHair01) * 0.5f,
                metabolismSpeed01 = (parentA.metabolismSpeed01 + parentB.metabolismSpeed01) * 0.5f,
                staminaCapacity01 = (parentA.staminaCapacity01 + parentB.staminaCapacity01) * 0.5f,
                immuneStrength01 = (parentA.immuneStrength01 + parentB.immuneStrength01) * 0.5f,
                painTolerance01 = (parentA.painTolerance01 + parentB.painTolerance01) * 0.5f,
                sleepQualityTendency01 = (parentA.sleepQualityTendency01 + parentB.sleepQualityTendency01) * 0.5f,
                stressTolerance01 = (parentA.stressTolerance01 + parentB.stressTolerance01) * 0.5f,
                agingRate01 = (parentA.agingRate01 + parentB.agingRate01) * 0.5f
            };

            appearance.skinFeatureIds = parentA.skinFeatureIds.Concat(parentB.skinFeatureIds).Distinct().Take(3).ToList();
            appearance.distinctiveFeatureIds = appearance.skinFeatureIds.ToList();
            appearance.bodyFrame = appearance.frameSize01 > 0.66f ? "Large" : appearance.frameSize01 < 0.33f ? "Slim" : "Average";
            appearance.hairStyleId = appearance.hairStyle.ToString();
            appearance.hairColorId = appearance.hairColor.ToString();
            appearance.eyeColorId = appearance.eyeColor.ToString();
            appearance.outfitPresetId = $"{appearance.upperWearId}_{appearance.lowerWearId}_{appearance.footwearId}";

            return appearance;
        }


        public static void ApplyGeneticInfluenceToAppearance(AppearanceProfile appearance, GeneticProfile genetics)
        {
            if (appearance == null || genetics == null)
            {
                return;
            }

            float Read(string geneId)
            {
                if (genetics.geneValues == null)
                {
                    return 0.5f;
                }

                var gene = genetics.geneValues.FirstOrDefault(g => g.geneId == geneId);
                return gene?.value ?? 0.5f;
            }

            appearance.metabolismSpeed01 = Read(GeneticGeneIds.MetabolismRate);
            appearance.staminaCapacity01 = Read(GeneticGeneIds.StaminaEfficiency);
            appearance.sleepQualityTendency01 = Read(GeneticGeneIds.SleepQualityTendency);
            appearance.stressTolerance01 = Read(GeneticGeneIds.StressTolerance);
            appearance.immuneStrength01 = Read(GeneticGeneIds.ImmuneSystemStrength);
            appearance.painTolerance01 = Read(GeneticGeneIds.PainTolerance);
            appearance.agingRate01 = Read(GeneticGeneIds.AgingRate);
            appearance.hairGrowthSpeed01 = Read(GeneticGeneIds.HairGrowthSpeed);
            appearance.bodyFrame = appearance.frameSize01 > 0.66f ? "Large" : appearance.frameSize01 < 0.33f ? "Slim" : "Average";
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

        private static List<NpcArchetypeDefinition> BuildNpcArchetypes()
        {
            return new List<NpcArchetypeDefinition>
            {
                new(NpcArchetypeType.ShopOwner, DialogueStyleType.Casual, "merchant_clean", new() { PersonalityTraitType.Practical, PersonalityTraitType.Ambitious }, new() { EmotionalTraitType.Stubborn, EmotionalTraitType.Loyal }, new() { SocialTraitType.Charismatic }, new() { LifestyleTraitType.Frugal }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Work, VoiceType.Deep, SpeechStyleType.Casual, OccupationCategoryType.Service, "shop_owner", "day_shift", 18),
                new(NpcArchetypeType.Bartender, DialogueStyleType.Warm, "service_smart", new() { PersonalityTraitType.Outgoing, PersonalityTraitType.Empathetic }, new() { EmotionalTraitType.Optimistic, EmotionalTraitType.Forgiving }, new() { SocialTraitType.Flirtatious }, new() { LifestyleTraitType.Extravagant }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Social, VoiceType.Cheerful, SpeechStyleType.Casual, OccupationCategoryType.Service, "bartender", "night_shift", 14),
                new(NpcArchetypeType.Mechanic, DialogueStyleType.Blunt, "work_oily", new() { PersonalityTraitType.Practical, PersonalityTraitType.Reserved }, new() { EmotionalTraitType.Stubborn }, new() { SocialTraitType.Awkward }, new() { LifestyleTraitType.Frugal }, new() { SurvivalTraitType.StrongMetabolism }, NpcScheduleBlock.Work, VoiceType.Raspy, SpeechStyleType.Blunt, OccupationCategoryType.Labor, "mechanic", "day_shift", 16),
                new(NpcArchetypeType.Nurse, DialogueStyleType.Formal, "clinic_clean", new() { PersonalityTraitType.Kind, PersonalityTraitType.Empathetic }, new() { EmotionalTraitType.Loyal, EmotionalTraitType.Anxious }, new() { SocialTraitType.Charismatic }, new() { LifestyleTraitType.Neat }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Work, VoiceType.Soft, SpeechStyleType.Formal, OccupationCategoryType.Professional, "nurse", "day_shift", 20),
                new(NpcArchetypeType.Teacher, DialogueStyleType.Formal, "smart_casual", new() { PersonalityTraitType.Kind, PersonalityTraitType.Introverted }, new() { EmotionalTraitType.Optimistic }, new() { SocialTraitType.Introverted }, new() { LifestyleTraitType.Neat }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Work, VoiceType.Soft, SpeechStyleType.Formal, OccupationCategoryType.Professional, "teacher", "day_shift", 19),
                new(NpcArchetypeType.Neighbor, DialogueStyleType.Casual, "casual_basic", new() { PersonalityTraitType.Kind }, new() { EmotionalTraitType.Forgiving }, new() { SocialTraitType.Awkward }, new() { LifestyleTraitType.Messy }, new() { SurvivalTraitType.StrongMetabolism }, NpcScheduleBlock.Home, VoiceType.Cheerful, SpeechStyleType.Casual, OccupationCategoryType.Service, "waiter", "split_shift", 11),
                new(NpcArchetypeType.Friend, DialogueStyleType.Warm, "social_trendy", new() { PersonalityTraitType.Outgoing, PersonalityTraitType.Empathetic }, new() { EmotionalTraitType.Optimistic, EmotionalTraitType.Loyal }, new() { SocialTraitType.Charismatic, SocialTraitType.Flirtatious }, new() { LifestyleTraitType.Extravagant }, new() { SurvivalTraitType.StrongMetabolism }, NpcScheduleBlock.Social, VoiceType.Energetic, SpeechStyleType.Casual, OccupationCategoryType.Creative, "musician", "night_shift", 13),
                new(NpcArchetypeType.Rival, DialogueStyleType.Blunt, "competitive", new() { PersonalityTraitType.Ambitious, PersonalityTraitType.Irritable }, new() { EmotionalTraitType.Jealous, EmotionalTraitType.Stubborn }, new() { SocialTraitType.Manipulative }, new() { LifestyleTraitType.Extravagant }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Errands, VoiceType.Deep, SpeechStyleType.Sarcastic, OccupationCategoryType.Professional, "lawyer", "day_shift", 24),
                new(NpcArchetypeType.RomanticInterest, DialogueStyleType.Flirty, "stylish", new() { PersonalityTraitType.Outgoing, PersonalityTraitType.Empathetic }, new() { EmotionalTraitType.Loyal, EmotionalTraitType.Anxious }, new() { SocialTraitType.Flirtatious, SocialTraitType.Charismatic }, new() { LifestyleTraitType.Neat }, new() { SurvivalTraitType.StrongMetabolism }, NpcScheduleBlock.Social, VoiceType.Soft, SpeechStyleType.Shy, OccupationCategoryType.Creative, "artist", "flex_shift", 15),
                new(NpcArchetypeType.WealthyResident, DialogueStyleType.Formal, "luxury", new() { PersonalityTraitType.Reserved, PersonalityTraitType.Ambitious }, new() { EmotionalTraitType.Stubborn }, new() { SocialTraitType.Manipulative }, new() { LifestyleTraitType.Extravagant }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Errands, VoiceType.Monotone, SpeechStyleType.Formal, OccupationCategoryType.Professional, "investor", "flex_shift", 32),
                new(NpcArchetypeType.MysteriousOutsider, DialogueStyleType.Cryptic, "traveler_dark", new() { PersonalityTraitType.Reserved, PersonalityTraitType.Anxious }, new() { EmotionalTraitType.Anxious, EmotionalTraitType.Jealous }, new() { SocialTraitType.Introverted, SocialTraitType.Awkward }, new() { LifestyleTraitType.Messy }, new() { SurvivalTraitType.FragileImmuneSystem }, NpcScheduleBlock.Commute, VoiceType.Raspy, SpeechStyleType.Shy, OccupationCategoryType.Criminal, "smuggler", "night_shift", 21),
                new(NpcArchetypeType.TravelingMerchant, DialogueStyleType.Casual, "merchant_travel", new() { PersonalityTraitType.Practical, PersonalityTraitType.Outgoing }, new() { EmotionalTraitType.Optimistic, EmotionalTraitType.Stubborn }, new() { SocialTraitType.Charismatic }, new() { LifestyleTraitType.Frugal }, new() { SurvivalTraitType.Resilient }, NpcScheduleBlock.Commute, VoiceType.Energetic, SpeechStyleType.Casual, OccupationCategoryType.Service, "traveling_merchant", "split_shift", 17)
            };
        }

        private static Vs01NpcState BuildNpcFromArchetype(string npcId, string displayName, NpcArchetypeDefinition archetype)
        {
            var fallbackFood = archetype.archetype == NpcArchetypeType.Bartender ? "grilled_meat" : archetype.archetype == NpcArchetypeType.Mechanic ? "grilled_meat" : "soup";
            var fallbackActivity = archetype.archetype == NpcArchetypeType.Teacher ? "reading" : archetype.archetype == NpcArchetypeType.TravelingMerchant ? "market" : "fishing";

            return new Vs01NpcState
            {
                npcId = npcId,
                displayName = displayName,
                archetype = archetype.archetype,
                dialogueStyle = archetype.dialogueStyle,
                voiceType = archetype.voiceType,
                speechStyle = archetype.speechStyle,
                clothingStyleId = archetype.clothingStyleId,
                occupationCategory = archetype.occupationCategory,
                occupationJobId = archetype.occupationJobId,
                occupationSchedulePreset = archetype.occupationSchedulePreset,
                occupationDailyIncome = archetype.occupationDailyIncome,
                occupationReputationImpact = archetype.occupationReputationImpact,
                occupationSocialCircleRadius = archetype.occupationSocialCircleRadius,
                upbringing = archetype.upbringing,
                educationLevel = archetype.educationLevel,
                hometown = archetype.hometown,
                pastRelationshipNotes = archetype.pastRelationshipNotes.ToList(),
                previousJobs = archetype.previousJobs.ToList(),
                traumaTags = archetype.traumaTags.ToList(),
                favoriteFoodItemIds = archetype.favoriteFoodItemIds.Count > 0 ? archetype.favoriteFoodItemIds.ToList() : new List<string> { fallbackFood },
                favoriteActivityIds = archetype.favoriteActivityIds.Count > 0 ? archetype.favoriteActivityIds.ToList() : new List<string> { fallbackActivity },
                preferredClothingStyleId = archetype.preferredClothingStyleId,
                musicTasteId = archetype.musicTasteId,
                hobbyIds = archetype.hobbyIds.Count > 0 ? archetype.hobbyIds.ToList() : new List<string> { fallbackActivity },
                personalityTraits = archetype.personalityTraits.ToList(),
                emotionalTraits = archetype.emotionalTraits.ToList(),
                socialTraits = archetype.socialTraits.ToList(),
                lifestyleTraits = archetype.lifestyleTraits.ToList(),
                survivalTraits = archetype.survivalTraits.ToList(),
                currentScheduleBlock = archetype.defaultSchedule,
                friendship = archetype.archetype == NpcArchetypeType.Rival ? 15f : 25f,
                trust = archetype.archetype == NpcArchetypeType.MysteriousOutsider ? 18f : 24f,
                respect = archetype.archetype == NpcArchetypeType.WealthyResident ? 38f : 26f
            };
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

                var category = i < 25 ? FoodCategoryType.BasicSurvival : i < 55 ? FoodCategoryType.Prepared : FoodCategoryType.Luxury;
                var hunger = category == FoodCategoryType.BasicSurvival ? 11f : category == FoodCategoryType.Prepared ? 18f : 14f;
                var hydration = category == FoodCategoryType.BasicSurvival ? 3f : 2f;
                var mood = category == FoodCategoryType.Luxury ? 6f : category == FoodCategoryType.Prepared ? 3f : 1f;
                var health = itemId.Contains("fried") || itemId.Contains("cream") ? -1f : 2f;
                var illnessRisk = itemId.Contains("wild") || itemId.Contains("canned") ? 2f : -1f;
                var energy = category == FoodCategoryType.Prepared ? 4f : 2f;

                defs.Add(new FoodDefinition(itemId, category, hunger, hydration, mood, health, illnessRisk, energy));
            }

            return defs;
        }
    }

    public class NpcArchetypeDefinition
    {
        public NpcArchetypeDefinition(
            NpcArchetypeType archetype,
            DialogueStyleType dialogueStyle,
            string clothingStyleId,
            List<PersonalityTraitType> personalityTraits,
            List<EmotionalTraitType> emotionalTraits,
            List<SocialTraitType> socialTraits,
            List<LifestyleTraitType> lifestyleTraits,
            List<SurvivalTraitType> survivalTraits,
            NpcScheduleBlock defaultSchedule,
            VoiceType voiceType = VoiceType.Soft,
            SpeechStyleType speechStyle = SpeechStyleType.Casual,
            OccupationCategoryType occupationCategory = OccupationCategoryType.Service,
            string occupationJobId = "barista",
            string occupationSchedulePreset = "day_shift",
            int occupationDailyIncome = 10,
            float occupationReputationImpact = 1f,
            float occupationSocialCircleRadius = 1f,
            string upbringing = "ordinary_town",
            EducationLevelType educationLevel = EducationLevelType.HighSchool,
            string hometown = "founders_town",
            List<string> pastRelationshipNotes = null,
            List<string> previousJobs = null,
            List<string> traumaTags = null,
            List<string> favoriteFoodItemIds = null,
            List<string> favoriteActivityIds = null,
            string preferredClothingStyleId = "casual_basic",
            string musicTasteId = "pop",
            List<string> hobbyIds = null)
        {
            this.archetype = archetype;
            this.dialogueStyle = dialogueStyle;
            this.clothingStyleId = clothingStyleId;
            this.personalityTraits = personalityTraits ?? new List<PersonalityTraitType>();
            this.emotionalTraits = emotionalTraits ?? new List<EmotionalTraitType>();
            this.socialTraits = socialTraits ?? new List<SocialTraitType>();
            this.lifestyleTraits = lifestyleTraits ?? new List<LifestyleTraitType>();
            this.survivalTraits = survivalTraits ?? new List<SurvivalTraitType>();
            this.defaultSchedule = defaultSchedule;
            this.voiceType = voiceType;
            this.speechStyle = speechStyle;
            this.occupationCategory = occupationCategory;
            this.occupationJobId = occupationJobId;
            this.occupationSchedulePreset = occupationSchedulePreset;
            this.occupationDailyIncome = occupationDailyIncome;
            this.occupationReputationImpact = occupationReputationImpact;
            this.occupationSocialCircleRadius = occupationSocialCircleRadius;
            this.upbringing = upbringing;
            this.educationLevel = educationLevel;
            this.hometown = hometown;
            this.pastRelationshipNotes = pastRelationshipNotes ?? new List<string>();
            this.previousJobs = previousJobs ?? new List<string>();
            this.traumaTags = traumaTags ?? new List<string>();
            this.favoriteFoodItemIds = favoriteFoodItemIds ?? new List<string>();
            this.favoriteActivityIds = favoriteActivityIds ?? new List<string>();
            this.preferredClothingStyleId = preferredClothingStyleId;
            this.musicTasteId = musicTasteId;
            this.hobbyIds = hobbyIds ?? new List<string>();
        }

        public NpcArchetypeType archetype { get; }
        public DialogueStyleType dialogueStyle { get; }
        public string clothingStyleId { get; }
        public List<PersonalityTraitType> personalityTraits { get; }
        public List<EmotionalTraitType> emotionalTraits { get; }
        public List<SocialTraitType> socialTraits { get; }
        public List<LifestyleTraitType> lifestyleTraits { get; }
        public List<SurvivalTraitType> survivalTraits { get; }
        public NpcScheduleBlock defaultSchedule { get; }
        public VoiceType voiceType { get; }
        public SpeechStyleType speechStyle { get; }
        public OccupationCategoryType occupationCategory { get; }
        public string occupationJobId { get; }
        public string occupationSchedulePreset { get; }
        public int occupationDailyIncome { get; }
        public float occupationReputationImpact { get; }
        public float occupationSocialCircleRadius { get; }
        public string upbringing { get; }
        public EducationLevelType educationLevel { get; }
        public string hometown { get; }
        public List<string> pastRelationshipNotes { get; }
        public List<string> previousJobs { get; }
        public List<string> traumaTags { get; }
        public List<string> favoriteFoodItemIds { get; }
        public List<string> favoriteActivityIds { get; }
        public string preferredClothingStyleId { get; }
        public string musicTasteId { get; }
        public List<string> hobbyIds { get; }
    }

    public class AnimalDefinition
    {
        public AnimalDefinition(string animalId, AnimalClassType animalClass, AnimalGameplayRoleType gameplayRole, string homeZoneId, string outputItemId, float encounterWeight)
        {
            this.animalId = animalId;
            this.animalClass = animalClass;
            this.gameplayRole = gameplayRole;
            this.homeZoneId = homeZoneId;
            this.outputItemId = outputItemId;
            this.encounterWeight = encounterWeight;
        }

        public string animalId { get; }
        public AnimalClassType animalClass { get; }
        public AnimalGameplayRoleType gameplayRole { get; }
        public string homeZoneId { get; }
        public string outputItemId { get; }
        public float encounterWeight { get; }
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
