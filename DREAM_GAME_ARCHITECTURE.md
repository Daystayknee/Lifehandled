# Dream Game Architecture

Life Sim × Survival × RPG

## 1) Recommended Tech Stack

- **Engine:** Unity
- **Language:** C#
- **Data:** ScriptableObjects + JSON save files (SQLite later)
- **UI:** UI Toolkit or Canvas (initially)
- **World Structure:** scene-based open zones first, seamless world later
- **AI:** Utility AI + behavior trees + schedule system
- **Art Pipeline:** stylized semi-real or modern digital pipeline

Unity is a strong fit for simulation-heavy games with lots of interacting systems (stats, items, events, UI, AI state, and progression).

---

## 2) Core System Layers (7-Layer Model)

### Layer A — Foundation

Core platform services:
- Game bootstrap
- Scene loading
- Time system
- Save/load system
- Event bus
- Data registry
- Audio manager
- Input manager

### Layer B — Player Simulation

Human-state simulation:
- Hunger, thirst, energy, sleep, hygiene
- Body temperature
- Stress, mood
- Illness/injury
- Needs decay
- Traits/personality

### Layer C — Survival Systems

Pressure and resource loop:
- Inventory weight
- Resource gathering
- Crafting
- Shelter safety
- Weather exposure
- Food spoilage
- Water purification
- Fire/warmth
- Farming/foraging/hunting
- Tool durability

### Layer D — Life Simulation

Daily-living loop:
- Jobs/side hustles
- Bills/rent/taxes
- Shopping/cooking
- Housing ownership/rentals
- Furniture placement
- Routine activities
- Cleaning/maintenance
- Vehicles/fuel
- Phone/messages/schedule planner
- Reputation/social identity

### Layer E — RPG Layer

Long-term progression:
- Skills/perks/traits
- Quests
- Relationships/factions
- Story flags
- Morality/values
- Unlockables/rare events

### Layer F — World Simulation

Living-world runtime:
- Day/night and seasons
- Dynamic weather
- NPC schedules
- Shop open/close logic
- Price fluctuations
- Random encounters
- Traffic/travel time
- Area danger ratings
- Wildlife spawns/loot tables
- Property values

### Layer G — Narrative / Drama Engine

Emotional and social depth:
- Relationship memory
- Secrets/gossip/reputation spread
- Choice consequences/event chains
- Dynamic dialogue
- Family ties/rivalries/romance
- Emergencies, betrayals, favors, debts

---

## 3) High-Level Runtime Map

```text
GameManager
 ├── TimeSystem
 ├── SaveSystem
 ├── EventBus
 ├── WorldManager
 ├── EconomyManager
 ├── WeatherManager
 ├── NPCManager
 ├── QuestManager
 ├── RelationshipManager
 ├── InventoryDatabase
 └── UIManager

Player
 ├── PlayerStats
 ├── NeedsSystem
 ├── SkillSystem
 ├── TraitSystem
 ├── HealthSystem
 ├── Inventory
 ├── Equipment
 ├── FinanceWallet
 ├── HousingData
 └── Journal/Phone

NPC
 ├── Identity
 ├── Personality
 ├── Schedule
 ├── Needs
 ├── RelationshipLinks
 ├── JobRole
 ├── DialogueBrain
 ├── Inventory
 └── MemoryLog
```

---

## 4) Unity Folder Layout

```text
Assets/
 ├── Scripts/
 │   ├── Core/
 │   ├── Managers/
 │   ├── Player/
 │   ├── NPC/
 │   ├── World/
 │   ├── Survival/
 │   ├── LifeSim/
 │   ├── RPG/
 │   ├── UI/
 │   ├── SaveLoad/
 │   └── Data/
 ├── ScriptableObjects/
 │   ├── Items/
 │   ├── Traits/
 │   ├── Skills/
 │   ├── Recipes/
 │   ├── NPCArchetypes/
 │   ├── Events/
 │   └── Locations/
 ├── Prefabs/
 ├── Art/
 ├── Audio/
 └── Scenes/
```

---

## 5) V1 Vertical Slice (Minimum Playable Loop)

The player can:
1. Wake up
2. Check needs
3. Eat/drink
4. Travel to location
5. Gather/buy resources
6. Talk to 2–3 NPCs
7. Manage money
8. Sleep
9. Save progress

This validates the core loop before adding feature depth.

---

## 6) Gameplay Loops

### Minute-to-minute
- Move
- Inspect
- Collect
- Use item
- Talk
- Decide

### Hour-to-hour
- Manage needs
- Travel
- Work
- Shop
- Craft
- Rest

### Day-to-day
- Pay bills
- Build relationships
- Improve skills
- Prepare for weather/events
- Upgrade home/resources

### Long-term
- Build a life and identity
- Unlock systems/areas
- Gain assets
- Influence the world
- Survive and thrive

---

## 7) Main Data Models

### PlayerProfile
- Name, age, appearance seed
- Traits, skills, stats, needs, mood
- Inventory/equipment
- Money/housing
- Relationships
- Quest states
- Known locations

### NPCProfile
- Unique ID, name, age, occupation
- Personality traits
- Daily schedule
- Home/work locations
- Relationships
- Likes/dislikes
- Secrets
- Current needs
- Dialogue flags
- Inventory

### ItemData
- ID, name, category
- Stack size, weight, value
- Decay rate
- Nutrition/hydration
- Durability
- Tags
- Use effects

### WorldState
- Current day/time
- Weather/season
- Economy values
- Active events
- NPC states
- Shop inventories
- Spawn states
- Player-owned assets

---

## 8) Manager Breakdown

### Core Managers
- GameManager
- TimeManager
- SaveManager
- SceneFlowManager
- UIManager

### Simulation Managers
- WeatherManager
- EconomyManager
- NPCManager
- RelationshipManager
- QuestManager
- SpawnManager

### Player-Side Systems
- NeedsSystem
- HealthSystem
- InventorySystem
- CraftingSystem
- SkillSystem
- FinanceSystem
- HousingSystem

---

## 9) Relationship Architecture

Each NPC needs **static** + **dynamic** state.

### Static
- Name
- Baseline personality
- Role/job
- Home
- Age
- Style/theme

### Dynamic
- Current mood
- Opinion of player
- Attraction
- Trust
- Fear
- Familiarity
- Memory tags
- Recent interactions
- Gossip heard

Example `RelationshipStats`:
- Friendship
- Trust
- Attraction
- Respect
- Fear
- Resentment
- Dependency

---

## 10) Interconnected World Systems (Example Chain)

Rain starts → player gets wet → body temperature falls → fatigue rises faster → sleep quality worsens → morning mood drops → player performs worse at work → earns less money → cannot afford better food → health recovery slows.

Design goal: systems should interact, not exist in isolation.

---

## 11) Survival Stat Model (Starter)

### Primary Needs
- Hunger
- Thirst
- Energy
- Warmth
- Hygiene

### Secondary States
- Stress
- Mood
- Pain
- Illness
- Stamina

Start simple and layer depth later.

---

## 12) Zone-Based World Plan (Early Scope)

Starter zones:
- Home
- Nearby road
- Shop/gas station
- Forest/wild area
- Town center
- Clinic
- Work location
- Rental/motel/shelter
- Lake/river
- Hidden collectible zone

Each zone defines:
- Danger level
- Resource table
- Weather modifier
- NPC pool
- Event triggers
- Shop/services
- Hidden items

---

## 13) Economy Architecture

Economy systems:
- Wallet/bank
- Item values
- Rarity
- Store markups
- Supply-demand shifts
- Rent/mortgage
- Utilities
- Gas prices
- Property values
- Collectible resale
- Rare vendors/black market (later)

Pricing formula:

```text
FinalPrice =
  BasePrice
  × RegionModifier
  × ScarcityModifier
  × ReputationModifier
  × EventModifier
```

---

## 14) Housing Architecture

Housing data:
- Ownership type
- Safety
- Comfort
- Storage capacity
- Water access
- Cooking access
- Electricity
- Cleanliness
- Repair state
- Neighborhood rating

Housing influences:
- Sleep quality
- Health
- Item preservation
- Mood
- Social options
- Storage efficiency
- Security

---

## 15) Inventory Architecture

Item categories:
- Food
- Drink
- Medical
- Materials
- Tools
- Clothing
- Valuables
- Collectibles
- Documents
- Keys
- Furniture
- Seeds/farming
- Creature care/pet items (future)

Items should support:
- Stack count
- Condition
- Expiration
- Tags
- Use actions
- Ownership
- Rarity

---

## 16) NPC AI Layering

1. Identity (who they are)
2. Schedule (where they should be)
3. Needs (what they currently need)
4. Decision logic (what they choose)
5. Memory (how they remember player/events)

Example base schedule:
- 06:00 wake
- 07:00 breakfast
- 08:00 commute
- 09:00 work
- 12:00 lunch
- 17:00 leave work
- 18:00 errands
- 20:00 social/home
- 23:00 sleep

Disruptors:
- Weather
- Illness
- Relationship events
- Fear
- Emergencies
- Player choices

---

## 17) Dialogue Architecture

Use state-driven dialogue with context checks:
- Time of day
- Location
- NPC mood
- Relationship level
- Recent player actions
- Active quests
- Player appearance/status
- Rumors
- Trust level
- Current needs

Outcome: less repetitive, more believable social simulation.

---

## 18) Save System Design

Persist clean data models (not raw scene object graphs):
- Player state
- Location
- Time/day
- Inventory
- Quests
- Relationships
- Housing
- World events
- Simplified NPC states
- Shop inventories
- Economy values

---

## 19) Recommended Build Order

### Phase 1 — Foundation
- Player movement
- Interact system
- Time system
- Inventory
- Needs
- Save/load
- One small map

### Phase 2 — Survival
- Hunger/thirst/energy
- Consumables
- Gathering
- Simple crafting
- Weather
- Sleeping

### Phase 3 — Life Sim
- Money
- Shops
- Housing
- Routines
- Job/income
- Phone/journal

### Phase 4 — Social
- NPC schedules
- Relationship stats
- Dialogue states
- Gifts/favors
- Gossip/reputation

### Phase 5 — RPG
- Skills
- Traits
- Quests
- Perks
- Factions/story pathing

### Phase 6 — Depth
- Illnesses
- Seasons
- Collectibles
- Property values
- Rare encounters
- Long-memory NPCs
- Family systems
- Genetics/breeding systems (optional)
