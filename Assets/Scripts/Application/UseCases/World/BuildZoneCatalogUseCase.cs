using System.Collections.Generic;
using System.Linq;
using Lifehandled.Application.Content;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.World
{
    public static class BuildZoneCatalogUseCase
    {
        public static List<ZoneState> CreateDefault()
        {
            return new List<ZoneState>
            {
                new()
                {
                    zoneType = ZoneType.Home,
                    zoneId = "home",
                    displayName = "Home",
                    npcPool = new List<string> { "npc_citizen_01", "npc_citizen_02" },
                    resources = new List<string> { "bed", "kitchen", "storage" },
                    events = new List<string> { "rest", "cleaning" },
                    dangerLevel = 0.05f
                },
                new()
                {
                    zoneType = ZoneType.TownCenter,
                    zoneId = "town_center",
                    displayName = "Town Center",
                    npcPool = BuildNpcSlice("npc_citizen_", 1, 12),
                    resources = new List<string> { "quest_board", "social_hub" },
                    events = new List<string> { "rumors", "social_drama" },
                    dangerLevel = 0.2f
                },
                new()
                {
                    zoneType = ZoneType.Forest,
                    zoneId = "forest",
                    displayName = "Forest",
                    npcPool = new List<string> { "npc_special_01" },
                    resources = PrototypeWorldContentCatalog.ToolItemIds.Take(6).Concat(new[] { "wood", "herbs" }).ToList(),
                    events = new List<string> { "storms", "break_ins", "gathering" },
                    dangerLevel = 0.65f
                },
                new()
                {
                    zoneType = ZoneType.GasStation,
                    zoneId = "gas_station",
                    displayName = "Gas Station",
                    npcPool = BuildNpcSlice("npc_shopkeeper_", 1, 4),
                    resources = new List<string> { "fuel_can", "snacks" },
                    events = new List<string> { "rumors", "break_ins" },
                    dangerLevel = 0.3f
                },
                new()
                {
                    zoneType = ZoneType.Lake,
                    zoneId = "lake",
                    displayName = "Lake",
                    npcPool = new List<string> { "npc_special_02", "npc_citizen_13" },
                    resources = new List<string> { "fish", "water" },
                    events = new List<string> { "storms", "illness_outbreaks", "fishing" },
                    dangerLevel = 0.35f
                },
                new()
                {
                    zoneType = ZoneType.Clinic,
                    zoneId = "clinic",
                    displayName = "Clinic",
                    npcPool = new List<string> { "npc_special_03", "npc_shopkeeper_05" },
                    resources = new List<string> { "medicine" },
                    events = new List<string> { "illness_outbreaks", "treatment" },
                    dangerLevel = 0.05f
                },
                new()
                {
                    zoneType = ZoneType.Apartments,
                    zoneId = "apartments",
                    displayName = "Apartments",
                    npcPool = BuildNpcSlice("npc_citizen_", 14, 20),
                    resources = new List<string> { "rent_office", "mailbox" },
                    events = new List<string> { "social_drama", "break_ins" },
                    dangerLevel = 0.15f
                },
                new()
                {
                    zoneType = ZoneType.Store,
                    zoneId = "store",
                    displayName = "Store",
                    npcPool = BuildNpcSlice("npc_shopkeeper_", 6, 10),
                    resources = PrototypeWorldContentCatalog.FoodItemIds.Take(20).ToList(),
                    events = new List<string> { "trade", "rumors" },
                    dangerLevel = 0.1f
                },
                new()
                {
                    zoneType = ZoneType.Workplace,
                    zoneId = "workplace",
                    displayName = "Workplace",
                    npcPool = new List<string> { "npc_special_04", "npc_special_05" },
                    resources = new List<string> { "income", "tools" },
                    events = new List<string> { "shift", "social_drama" },
                    dangerLevel = 0.25f
                }
            };
        }

        private static List<string> BuildNpcSlice(string prefix, int start, int end)
        {
            var list = new List<string>();
            for (var i = start; i <= end; i++)
            {
                list.Add($"{prefix}{i:00}");
            }

            return list;
        }
    }
}
