# Unity Refactor & Implementation Plan (Incremental)

This plan converts `UNITY_ARCHITECTURE_AUDIT.md` into an actionable, low-risk roadmap for a Unity project, without rewriting everything.

Design constraints:
- Keep gameplay goals unchanged.
- Keep scope incremental and team-practical.
- Avoid big-bang rewrites.
- Preserve current momentum for Vertical Slice 01.

---

## 1) Top 5 Architecture Issues to Fix First

## Issue 1 — Manager coupling and cross-calls

**Symptom**
Managers call each other directly (often globally), creating hidden runtime dependencies.

**Risk**
Regression blast radius and brittle initialization order.

**Fix intent**
Introduce application-level use-cases and event dispatch for cross-module interactions.

---

## Issue 2 — Unclear ownership / duplicate responsibilities

**Symptom**
Overlaps between world, economy, NPC, relationship, and UI orchestration responsibilities.

**Risk**
Conflicting source of truth and save/load ambiguity.

**Fix intent**
Define bounded ownership matrix and enforce one owner per mutable state domain.

---

## Issue 3 — Missing interface boundaries

**Symptom**
Systems are class-named but not contract-driven.

**Risk**
Hard to test and hard to swap persistence/input/time implementations later.

**Fix intent**
Create ports/interfaces at module boundaries, then adapt existing managers behind them.

---

## Issue 4 — Save/load fragility

**Symptom**
Potentially broad state persistence without strict DTO/version strategy.

**Risk**
Corrupt/incompatible saves, partial reload failures.

**Fix intent**
Introduce modular snapshots + envelope + versioning + deterministic load sequence.

---

## Issue 5 — Singleton overuse and startup overreach

**Symptom**
`GameManager`/global instances likely carry bootstrap + logic + integration.

**Risk**
Hard scene lifecycle behavior, duplicate instances, testing barriers.

**Fix intent**
Keep a thin composition root and narrow singleton scope to session bootstrap only.

---

## 2) Fixes in Safest Order (Recommended Sequence)

## Step A — Stabilize ownership and dependency rules (documentation + guardrails)

**Action**
- Add ownership matrix and allowed dependency direction to project docs.
- Define simple architecture rules for PR review.

**Why first**
No runtime risk; prevents wrong changes before code refactor begins.

**Done criteria**
- Ownership table exists and is approved.
- Team can answer "who owns this state?" for each key system.
- New feature PRs reference ownership and dependency rule.

---

## Step B — Introduce interfaces/ports around current managers

**Action**
- Create first batch of interfaces (see section 5).
- Keep existing manager implementations, but access via interface where touched.

**Why second**
Low-risk seam creation without changing behavior.

**Done criteria**
- At least one active feature path resolves dependencies through interfaces, not concrete manager classes.
- Unit tests can mock time/save/inventory dependencies for that feature path.

---

## Step C — Extract use-cases for high-value actions

**Action**
- Move cross-system game actions into application services:
  - Sleep
  - Consume item
  - Purchase item
  - Travel zone

**Why third**
Gives immediate decoupling value while preserving current runtime systems.

**Done criteria**
- These actions are invoked through use-case classes.
- UI and input call use-cases, not manager internals.
- Manager-to-manager direct calls reduced on these paths.

---

## Step D — Implement save envelope + modular snapshots

**Action**
- Add save envelope (`saveVersion`, per-module snapshots).
- Serialize DTOs only.
- Introduce deterministic load order and migration hook.

**Why fourth**
Once interfaces and ownership are clearer, persistence can be made safe without churn.

**Done criteria**
- Save file includes version and modular sections.
- Load path validates IDs/references and rebuilds runtime indexes.
- At least one backward-compatible migration path exists (even v1→v1 no-op scaffold).

---

## Step E — Thin composition root and singleton reduction

**Action**
- Create startup/composition class that wires dependencies explicitly.
- Restrict globals to boot/session references only.

**Why fifth**
Safer once interfaces/use-cases exist; avoids breaking scene lifecycle early.

**Done criteria**
- `GameManager` (or equivalent) no longer contains business rules.
- Runtime services registered in one place.
- Duplicate-instance guard and lifecycle policy documented and enforced.

---

## 3) What Can Stay As-Is for Now

Keep these unchanged during initial refactor to minimize disruption:

- Existing gameplay goals, loops, and system feature list.
- Current ScriptableObject-based data authoring for items/traits/skills.
- Zone-based world approach (scene-oriented) for VS01.
- Existing manager class names (temporarily), as adapters behind interfaces.
- Existing UI stack (Canvas/UI Toolkit), as long as it calls use-cases gradually.
- JSON save backend (SQLite can remain later-phase).

**Condition**
As-is is acceptable only if new work follows ownership and interface rules.

---

## 4) Files/Folders to Move or Rename First

Do this in minimal, practical increments (no massive moves in one PR).

## First move set (safe, high value)

1. Create top-level architecture folders:

```text
Assets/Scripts/
  Application/
  Domain/
  Infrastructure/
  Presentation/
  CompositionRoot/
```

2. Add `*.asmdef` files aligned to these folders (if not already used):
- `Game.Domain.asmdef`
- `Game.Application.asmdef`
- `Game.Infrastructure.asmdef`
- `Game.Presentation.asmdef`
- `Game.CompositionRoot.asmdef`

3. Move only new/refactored code first (leave old paths temporarily):
- New interfaces → `Application/Ports/`
- New use-cases → `Application/UseCases/`
- Save DTOs/envelope → `Infrastructure/Persistence/DTO/`
- UI presenters/view models → `Presentation/UI/`
- Bootstrap wiring → `CompositionRoot/`

## Rename policy

- Do **not** mass-rename existing managers immediately.
- When touching a manager, optionally rename toward intent:
  - `XManager` used for orchestration → `XService` / `XCoordinator`
  - state owner classes stay explicit (`WorldStateStore`, `RelationshipStateStore`)

**Done criteria**
- Folder skeleton exists.
- First 2–3 incremental PRs place only new code into target folders.
- No project-wide path churn breaking prefabs/scenes.

---

## 5) Interfaces to Create First (Priority Set)

Start with ports needed by VS01 gameplay loop and save safety.

## Tier 1 (create first)

- `ITimeService`
  - `CurrentGameTime`, `AdvanceMinutes(int)`, `OnTimeTick`
- `IPlayerNeedsService`
  - query/modify hunger thirst energy hygiene warmth
- `IInventoryService`
  - add/remove/query item stacks, weight checks
- `IEconomyService`
  - price calculation, wallet debit/credit, affordability check
- `ISaveRepository`
  - `Save(SaveGameEnvelope)`, `Load()`, `HasSave()`
- `IZoneTravelService`
  - validate route, travel cost/time, apply relocation

## Tier 2 (next)

- `IWeatherService`
- `INPCScheduleService`
- `IRelationshipService`
- `IQuestStateService`
- `IEventBus` (or integration event dispatcher)

**Done criteria**
- Tier 1 interfaces compiled and used by at least one use-case each.
- Concrete adapters exist for current managers/systems.
- Tests can mock Tier 1 interfaces in application-layer unit tests.

---

## 6) Vertical Slice 01 (VS01) Implementation Plan

Target loop (unchanged intent): wake → check needs → eat/drink → travel → gather/buy → talk to 2–3 NPCs → manage money → sleep → save.

## VS01-Phase 0: Planning + Contracts (1 short sprint)

**Implement**
- Ownership matrix doc.
- Tier 1 interfaces.
- Minimal composition root registration.

**Done**
- Build compiles.
- One smoke test scene boots via composition root.

---

## VS01-Phase 1: Needs + Consumables (core survival loop)

**Implement**
- `ConsumeItemUseCase` using `IInventoryService`, `IPlayerNeedsService`.
- Item effect mapping from existing ScriptableObject data.
- UI panel reads needs via view model.

**Done**
- Player can consume valid food/drink.
- Needs update deterministically.
- Invalid consume attempts return clear reason codes.

---

## VS01-Phase 2: Travel + Time progression

**Implement**
- `TravelToZoneUseCase` with `IZoneTravelService` + `ITimeService`.
- Route config per starter zones.
- On travel, time advances and dependent systems tick.

**Done**
- Travel changes location and advances game time.
- Blocked travel reasons surfaced (cost, locked zone, etc.).

---

## VS01-Phase 3: Buy/Gather + Money loop

**Implement**
- `PurchaseItemUseCase` using `IEconomyService`, `IInventoryService`.
- Basic gather action grants resource items with constraints.
- Wallet and price calculations routed through economy service.

**Done**
- Purchase success/failure deterministic.
- Wallet cannot go negative.
- Inventory respects stack/weight constraints.

---

## VS01-Phase 4: NPC interaction (light social)

**Implement**
- `TalkToNPCUseCase` with minimal context checks (time/location/relationship stub).
- Hook to existing dialogue content path.

**Done**
- Player can talk to at least 2–3 NPCs.
- Dialogue changes by at least one state factor (e.g., time or relationship band).

---

## VS01-Phase 5: Sleep + Daily rollover

**Implement**
- `SleepUseCase` updates time + energy + selected needs recovery/decay.
- Daily rollover hook for bills/reputation placeholder events.

**Done**
- Sleep transitions to next time block/day.
- Recovery/decay values are data-driven and test-covered.

---

## VS01-Phase 6: Save/Load integration

**Implement**
- `SaveGameEnvelope` with module snapshots (player/world/economy/inventory/social-lite).
- JSON repository adapter via `ISaveRepository`.
- Deterministic load sequence restoring VS01 loop state.

**Done**
- Save and reload returns user to same loop state reliably.
- Corrupt/invalid save handling shows recoverable error path.

---

## 7) Clear “Done” Criteria Checklist by Step

Use this as release gate criteria.

## Architecture gate

- [ ] Ownership matrix approved.
- [ ] Dependency direction rule documented and referenced in PR template.
- [ ] No new direct cross-manager calls for refactored paths.

## Interface gate

- [ ] Tier 1 interfaces implemented.
- [ ] Use-cases depend only on interfaces.
- [ ] Existing managers reachable via adapter implementations.

## Use-case gate

- [ ] Consume, Travel, Purchase, Talk, Sleep each implemented as separate use-case class.
- [ ] UI/input invoke use-cases, not manager internals.
- [ ] Failure reason codes standardized and surfaced to UI.

## Save/load gate

- [ ] Save envelope has `saveVersion`.
- [ ] DTO-only serialization (no scene object refs).
- [ ] Load order deterministic and documented.
- [ ] Backup/atomic write strategy in place.

## VS01 gameplay gate

- [ ] Full VS01 loop playable end-to-end.
- [ ] At least one automated test per core use-case happy path.
- [ ] At least one automated test per critical failure path (insufficient funds, full inventory, invalid travel).
- [ ] Manual playtest checklist passed in one session without reset.

---

## Suggested PR Breakdown (Incremental)

1. **PR-01:** Architecture guardrails + folder skeleton + asmdefs + Tier 1 interfaces.
2. **PR-02:** Consume + Travel use-cases wired to current managers via adapters.
3. **PR-03:** Purchase + wallet integration and inventory constraints.
4. **PR-04:** Talk + Sleep use-cases + daily rollover hook.
5. **PR-05:** Save envelope + JSON repository + load sequence.
6. **PR-06:** VS01 polish + tests + bug fixes.

Each PR should be independently shippable and keep existing scenes functional.
