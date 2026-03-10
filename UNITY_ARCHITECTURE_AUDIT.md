# Unity Architecture Audit

## Scope

This audit reviews the current project skeleton described in `DREAM_GAME_ARCHITECTURE.md` and checks for:

- tightly coupled systems
- duplicate responsibilities
- missing interfaces
- bad dependency flow
- manager overreach
- unsafe singleton usage
- save/load risks

It then proposes a cleaner modular structure **without changing gameplay goals** (Life Sim × Survival × RPG).

---

## Executive Summary

The current skeleton is ambitious and directionally strong, but it has common early-architecture risks:

1. **Manager-centric coupling risk**: many global managers are likely to become cross-dependent.
2. **Boundary blur**: overlapping ownership between `GameManager`, `WorldManager`, and domain managers.
3. **Interface gaps**: architecture lists systems but not contracts, making substitution/testing hard.
4. **Dependency direction risk**: gameplay domains may end up depending on Unity scene/runtime details.
5. **Singleton hazards**: manager naming implies easy drift into static singleton anti-patterns.
6. **Save/load complexity risk**: wide world state with mutable references can cause fragile persistence.

A cleaner modular model is recommended: **Domain Modules + Application Orchestration + Infrastructure Adapters + Composition Root**.

---

## Detailed Audit Findings

## 1) Tightly Coupled Systems

### Observed risk in skeleton

The runtime map centralizes many high-level systems under `GameManager`:

- Time
- Save
- Event bus
- World/economy/weather/NPC/quest/relationship/UI

This can lead to fan-out dependencies where most systems depend (directly or indirectly) on shared global state.

### Why this is a problem

- Hard to test systems in isolation.
- Changes in one manager trigger regressions in unrelated features.
- Initialization order becomes brittle.

### Recommendation

- Convert manager-to-manager calls into **message-driven integration** (domain events + application commands).
- Keep direct dependencies only where deterministic ownership is required.
- Prefer feature modules with local state and explicit public APIs.

---

## 2) Duplicate Responsibilities

### Likely overlap zones

- `WorldManager` vs `WeatherManager` vs `SpawnManager` (world simulation ownership)
- `NPCManager` vs `RelationshipManager` vs `DialogueBrain` (social state ownership)
- `EconomyManager` vs shop systems vs quest reward logic (price/value authority)
- `UIManager` vs Phone/Journal systems (presentation vs feature orchestration)

### Why this is a problem

- Conflicting source of truth.
- Circular update logic.
- Difficult save/load restoration due to split ownership.

### Recommendation

Define strict ownership per bounded context:

- **World Simulation Module** owns time/weather/zone simulation clocks.
- **Social Module** owns relationship numbers + social memory.
- **Economy Module** owns pricing rules and transactions.
- **UI Layer** reads view models only; no gameplay mutations except through commands.

---

## 3) Missing Interfaces

### Current gap

The blueprint names classes/systems, but lacks interface contracts such as:

- `ITimeService`
- `ISaveRepository`
- `IInventoryService`
- `IEconomyService`
- `INPCScheduler`
- `IDialogueContextProvider`

### Why this matters

- No clean seam for mocking in tests.
- Infrastructure concerns leak into gameplay logic.
- Replacing JSON with SQLite later becomes expensive.

### Recommendation

Add interfaces at module boundaries and keep implementations behind adapters:

- Domain code depends on interfaces.
- Unity/IO code implements interfaces.
- Composition root wires concrete classes.

---

## 4) Bad Dependency Flow

### Common failure mode likely here

If managers are Unity `MonoBehaviour` singletons, dependency arrows often drift into:

`Gameplay domain -> Scene object -> Unity APIs -> other global manager`

instead of:

`Domain -> Application Services -> Infrastructure Adapters`

### Recommendation

Enforce dependency rule:

- **Inner layers never depend on outer layers.**
- Domain models contain pure rules/state transitions.
- Unity components should act as delivery and rendering adapters only.

---

## 5) Manager Overreach

### Risk indicators in current skeleton

- `GameManager` appears to coordinate nearly all major systems.
- Managers may become mixed orchestration + business logic + persistence access.

### Recommendation

Split responsibilities:

- **GameDirector (thin)**: startup and high-level mode transitions.
- **Use-case/Application services**: explicit actions (e.g., `SleepAction`, `PurchaseItemAction`).
- **Domain services**: rule computation only.
- **Infrastructure services**: storage, scene loading, audio, input adapters.

---

## 6) Unsafe Singleton Usage

### Likely issue

Unity projects often use `public static Instance` per manager. In a multi-scene, additive-load, or test context this causes:

- hidden runtime order dependencies
- stale references after scene reload
- duplicate instance race conditions

### Recommendation

- Use a **composition root** scene/context with explicit registration.
- Prefer dependency injection (manual or DI framework).
- Restrict singletons to true process-wide stateless utilities (rare).
- For shared runtime state, prefer scoped services bound to game session.

---

## 7) Save/Load Risks

### Risk profile in skeleton

State to persist is broad: player, world, NPC schedules/needs/relationships, inventories, economy, shop states, events.

Without schema and ownership discipline, likely failure points are:

- saving runtime object references
- version breaks when models evolve
- partial load inconsistencies across modules
- non-atomic writes causing corrupted saves

### Recommendation

1. **Save DTOs only** (no scene object refs).
2. Add `saveVersion` + migration pipeline.
3. Snapshot each module independently, then compose a world save envelope.
4. Use transactional write pattern:
   - write temp
   - checksum
   - atomic replace
   - keep rolling backups
5. Deterministic load order:
   - core clock/state
   - world modules
   - entities
   - UI restoration

---

## Proposed Cleaner Modular Structure

## A) Layered + Modular Target

### 1. Domain Layer (pure C#)

Modules:
- `Domain.Player`
- `Domain.Survival`
- `Domain.LifeSim`
- `Domain.RPG`
- `Domain.Social`
- `Domain.World`
- `Domain.Economy`

Contains:
- entities/value objects
- invariants/rules
- domain events

No Unity API usage.

### 2. Application Layer

Modules:
- `App.PlayerUseCases`
- `App.WorldSimulation`
- `App.SocialUseCases`
- `App.SaveLoad`

Contains:
- command handlers / use-cases
- orchestration across domain modules
- interface-based dependencies

### 3. Infrastructure Layer

Modules:
- `Infra.Persistence.Json`
- `Infra.Persistence.SQLite`
- `Infra.UnityScene`
- `Infra.Audio`
- `Infra.Input`
- `Infra.Time`

Contains concrete adapters for IO/Unity/platform.

### 4. Presentation Layer

Modules:
- `UI.HUD`
- `UI.Inventory`
- `UI.Dialogue`
- `UI.PhoneJournal`

Reads view models, issues commands through application services.

### 5. Composition Root

- Startup scene installs services and binds interfaces.
- No gameplay logic here.

---

## B) Dependency Direction

```text
Presentation -> Application -> Domain
       \             |
        \            v
         ------> Infrastructure
```

Rules:
- Domain depends on nothing external.
- Application depends on Domain + abstract ports.
- Infrastructure depends on ports and framework APIs.
- Presentation depends on Application contracts.

---

## C) Eventing Strategy

Use two event types:

1. **Domain Events** (inside module boundaries)
   - Example: `NeedDepleted`, `RelationshipChanged`, `WeatherShifted`

2. **Integration Events** (cross-module)
   - Example: `RainStarted` triggers Survival exposure and NPC schedule adaptation.

Use an event queue processed in deterministic tick order to avoid race conditions.

---

## D) Suggested Ownership Matrix

- `Time`: World module
- `Weather/Season`: World module
- `Needs/Health`: Player + Survival modules
- `Inventory`: Player module (with shared item catalog service)
- `Economy/Prices`: Economy module
- `Relationships/Social memory`: Social module
- `Quest state`: RPG module
- `Dialogue context assembly`: Social + RPG read models
- `Save snapshots`: App.SaveLoad per module serializer

No shared mutable ownership across modules.

---

## E) Safe Save/Load Blueprint

### Save Envelope

```text
SaveGame
- saveVersion
- createdAtUtc
- playtimeSeconds
- PlayerSnapshot
- WorldSnapshot
- EconomySnapshot
- SocialSnapshot
- QuestSnapshot
- InventorySnapshot
- ActiveEventSnapshot
```

### Protocol

1. Pause simulation tick.
2. Request immutable snapshot from each module.
3. Validate cross-module IDs.
4. Serialize with version.
5. Atomic write + backup.
6. Resume simulation.

### Load Protocol

1. Parse + migrate to current version.
2. Initialize module services.
3. Load snapshots in dependency order.
4. Rebuild runtime indices/cache.
5. Emit `GameLoaded` integration event.

---

## F) Minimal Refactor Path (Non-Disruptive)

1. Keep gameplay goals and existing planned systems unchanged.
2. Introduce interfaces for current manager APIs.
3. Move business rules out of managers into domain services.
4. Replace direct manager-to-manager calls with application use-cases/events.
5. Implement module snapshots for save/load.
6. Reduce `GameManager` to bootstrap + scene state transitions only.

This path preserves current design intent while lowering long-term architecture risk.

---

## Suggested Unity Folder Evolution

```text
Assets/
 ├── Scripts/
 │   ├── Domain/
 │   │   ├── Player/
 │   │   ├── Survival/
 │   │   ├── LifeSim/
 │   │   ├── RPG/
 │   │   ├── Social/
 │   │   ├── World/
 │   │   └── Economy/
 │   ├── Application/
 │   ├── Infrastructure/
 │   │   ├── Persistence/
 │   │   ├── UnityAdapters/
 │   │   └── Services/
 │   ├── Presentation/
 │   │   ├── UI/
 │   │   └── ViewModels/
 │   └── CompositionRoot/
 ├── ScriptableObjects/
 └── Scenes/
```

---

## Final Recommendation

Keep the gameplay vision exactly as defined, but enforce strict module ownership and dependency direction now, before implementation scale increases. This will substantially reduce rework as systems like economy, social memory, dynamic dialogue, and long-term persistence become more complex.
