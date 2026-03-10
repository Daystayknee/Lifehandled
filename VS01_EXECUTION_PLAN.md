# Vertical Slice 01 (VS01) Execution Plan

This plan continues from the current Character/Household/Genetics foundation and existing new-game/session bootstrap work.

Scope goal: deliver the first fully playable daily loop with save/reload continuity, without architecture rewrites.

---

## 1) Exact Next Implementation Order for VS01

Implement in this exact order:

1. **Session Entry + Status Readout**
   - In playable scene, consume `SessionContextRegistry.Current` and render minimal player status/needs panel.
2. **Needs Runtime Core (Minimal)**
   - Add runtime needs state (hunger/thirst/energy/stress) for selected player and basic tick/update.
3. **Consume Item Action**
   - Add one consumable item action path that modifies needs.
4. **Single World Interaction Path**
   - Implement one path only: either (A) travel to one location OR (B) interact with one shop/resource point.
5. **Buy or Gather One Item**
   - Add one item acquisition action and inventory update (shop purchase OR gather node).
6. **Optional Simple Social Interaction**
   - One interaction button with household member or one NPC to prove social hook works.
7. **Sleep / End Day Action**
   - Add end-day action that advances day block and updates energy/needs.
8. **Save + Reload Continuity**
   - Persist runtime state and ensure reload restores same player/session context and key values.

---

## 2) Smallest Possible Playable Checkpoint

**Checkpoint definition (MVP-of-VS01):**

- Scene loads with selected player from session context.
- HUD shows 4 needs values.
- Player can click one consumable action.
- Player can perform one acquisition action (buy OR gather).
- Player can click Sleep.
- Save file updates.
- Reload returns with same character and updated values.

If this works, the core loop is proven and can be expanded safely.

---

## 3) PR-by-PR / Batch-by-Batch Sequence

## PR-A: VS01 Runtime State + HUD

**Add**
- `PlayerNeedsRuntimeState` (runtime-only container)
- `Vs01StatusPanelController` (simple text bars/labels)
- binding to `SelectedPlayerCharacterAccessor`

**Done when**
- scene shows selected player + baseline needs
- no save changes yet

---

## PR-B: Consume Item + Minimal Inventory Slice

**Add**
- `ConsumeStarterItemUseCase` (one item type)
- tiny runtime inventory list (or single item counter)
- UI button: "Consume"

**Done when**
- pressing consume updates needs deterministically

---

## PR-C: One Acquisition Path (choose one)

Option 1 (fastest): **Gather point**
- `GatherResourceUseCase`
- one scene interaction point
- grants one item

Option 2: **Shop point**
- `BuyItemUseCase`
- minimal wallet integer + one buy button

**Done when**
- one action adds item to inventory path

---

## PR-D: Sleep/End Day + Optional Social Ping

**Add**
- `SleepUseCase` (advance day segment, recover energy)
- optional `SimpleTalkUseCase` (single relationship delta or message)

**Done when**
- day can end and player can perform one social interaction (optional but wired)

---

## PR-E: Save/Reload Integration for VS01 State

**Add**
- extend save envelope with minimal runtime needs/inventory/day-block payload
- loader writes back into session runtime state
- reload check path in playable scene

**Done when**
- save + reload returns to same character, needs, and inventory/day state

---

## 4) Systems to Implement First vs Stub

## Implement first (real behavior)

- Session context consumption in playable scene
- Needs runtime state + display
- Consume action
- One acquisition action (buy or gather)
- Sleep action
- Save/reload for above state

## Stub for now

- Advanced economy formulas
- Multi-location travel graph
- Deep NPC schedules
- Full dialogue context engine
- Crafting trees
- advanced genetics/inheritance behaviors

Keep stubs as interfaces/placeholders where needed, but no heavy implementation.

---

## 5) Minimum UI Needed

In playable scene only:

1. **Status Panel**
   - Name
   - Hunger / Thirst / Energy / Stress
2. **Action Buttons**
   - Consume Item
   - Gather/Buy Item (one action)
   - Sleep / End Day
   - Save
3. **Optional Social Button**
   - Talk (single interaction)
4. **Feedback Text**
   - short result messages (e.g., "Consumed Water", "Saved")

No advanced menus required.

---

## 6) Minimum Data Needed

Add only these runtime/save fields for VS01:

- `playerCharacterId` (already present)
- `currentDay` (int)
- `daySegment` (enum/int; Morning/Evening or 0/1)
- `needs` for selected player:
  - hunger
  - thirst
  - energy
  - stress
- `starterInventory` (simple itemId->count)
- `wallet` (if using buy path)
- `currentLocationId` (if using travel or location interaction)

Keep data intentionally flat and migration-safe.

---

## 7) Testing Steps for Each Batch

## PR-A tests

- Load playable scene after New Game.
- Verify selected player name appears from session context.
- Verify needs values are visible and initialized.

## PR-B tests

- Click Consume once.
- Verify needs changed as expected.
- Repeat for boundary checks (no item available).

## PR-C tests

- Trigger gather/buy once.
- Verify item count increments.
- If buy path: verify wallet decrements and cannot go below zero.

## PR-D tests

- Click Sleep.
- Verify day segment/day increments and energy/needs update.
- (Optional social) click Talk and confirm one relationship/log effect.

## PR-E tests

- Perform loop actions.
- Save.
- Reload scene/app.
- Verify player ID, needs, inventory, day state, and household context are preserved.

---

## 8) Common Mistakes to Avoid

1. **Bypassing session context**
   - Don’t read/write player data from random scene objects directly.
2. **Overbuilding inventory/economy too early**
   - Keep one item/action path first.
3. **Coupling UI to infrastructure**
   - UI should call use-cases, not file IO directly.
4. **Skipping save defaults/migration safety**
   - Keep compatibility helpers in the load path.
5. **Expanding genetics beyond subtle multipliers**
   - Maintain prototype-safe impact ranges.
6. **Trying to implement all social systems now**
   - One interaction hook is enough for VS01.
7. **Adding multiple travel/shop/resource systems at once**
   - Pick one path first, ship, then expand.

---

## Recommended First Commit After This Plan

Start with **PR-A (runtime status + HUD)** because it creates immediate visibility into session correctness and all later loop actions depend on it.
