using System.Collections.Generic;
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
                    npcPool = new List<string> { "npc_neighbor_01" },
                    resources = new List<string> { "bed", "kitchen", "storage" },
                    events = new List<string> { "rest", "cleaning" },
                    dangerLevel = 0.05f
                },
                new()
                {
                    zoneType = ZoneType.Town,
                    zoneId = "town",
                    displayName = "Town",
                    npcPool = new List<string> { "npc_vendor_01", "npc_neighbor_01" },
                    resources = new List<string> { "social_hub" },
                    events = new List<string> { "gossip", "street_event" },
                    dangerLevel = 0.2f
                },
                new()
                {
                    zoneType = ZoneType.Store,
                    zoneId = "store",
                    displayName = "Store",
                    npcPool = new List<string> { "npc_vendor_01" },
                    resources = new List<string> { "water_bottle", "stale_food" },
                    events = new List<string> { "trade", "price_change" },
                    dangerLevel = 0.1f
                },
                new()
                {
                    zoneType = ZoneType.Forest,
                    zoneId = "forest",
                    displayName = "Forest",
                    npcPool = new List<string>(),
                    resources = new List<string> { "wood", "herbs" },
                    events = new List<string> { "gathering", "wild_encounter" },
                    dangerLevel = 0.65f
                },
                new()
                {
                    zoneType = ZoneType.Lake,
                    zoneId = "lake",
                    displayName = "Lake",
                    npcPool = new List<string>(),
                    resources = new List<string> { "fish", "water" },
                    events = new List<string> { "fishing", "rain_event" },
                    dangerLevel = 0.35f
                },
                new()
                {
                    zoneType = ZoneType.Clinic,
                    zoneId = "clinic",
                    displayName = "Clinic",
                    npcPool = new List<string>(),
                    resources = new List<string> { "medicine" },
                    events = new List<string> { "treatment" },
                    dangerLevel = 0.05f
                },
                new()
                {
                    zoneType = ZoneType.Workplace,
                    zoneId = "workplace",
                    displayName = "Workplace",
                    npcPool = new List<string> { "npc_vendor_01" },
                    resources = new List<string> { "income" },
                    events = new List<string> { "shift", "work_conflict" },
                    dangerLevel = 0.25f
                }
            };
        }
    }
}
