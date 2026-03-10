using System.Collections.Generic;
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

        public static List<Vs01NpcState> CreateNpcRoster()
        {
            var roster = new List<Vs01NpcState>();

            // 20 citizens
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

            // 10 shopkeepers
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

            // 5 special characters
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
    }
}
