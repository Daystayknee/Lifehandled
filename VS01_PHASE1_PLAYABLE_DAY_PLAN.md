# VS01 Phase 1 Plan — Complete Playable Day Loop

This plan continues from the current implemented foundation (session bootstrap, needs runtime/HUD, consume/buy/save/reload, sleep/end-day).

Target: a complete first playable day loop:
1) wake/start
2) see needs
3) use item
4) go somewhere
5) buy/gather item
6) talk to someone
7) sleep
8) save + reload

---

## 1) Next Implementation Batches

## Batch A — Location/Travel Stub (Go Somewhere)

**Goal**
Add one lightweight location change interaction so the player can “go somewhere” in-loop.

**Implement**
- `currentLocationId` in runtime session + save payload.
- `TravelToLocationUseCase` with one destination (`shop_point` or `gather_point`).
- One UI button: `Go To Shop` (or `Go To Gather Point`).

**Done criteria**
- Location text updates in HUD.
- Save/reload restores same location.

---

## Batch B — One Social Interaction (Talk)

**Goal**
Add one minimal social action to satisfy “talk to someone.”

**Implement**
- `SimpleTalkUseCase` with one target:
  - optional household member if exists, else fallback NPC label.
- Track one tiny social marker in runtime (`lastTalkDay` / `talkCountToday`).
- One UI button: `Talk`.

**Done criteria**
- Pressing Talk gives feedback and updates simple social marker.
- Save/reload keeps social marker.

---

## Batch C — Loop Polish + Validation Pass

**Goal**
Ensure full day loop is coherent and robust using existing use-cases.

**Implement**
- Wire all existing actions into one panel flow:
  - consume
  - buy/gather
  - talk
  - sleep (autosave)
  - reload validation
- Add minimal completion checklist text in HUD (optional).

**Done criteria**
- Full loop can be completed in one run without manual data editing.

---

## 2) Smallest Playable Checkpoint

Smallest acceptable “playable day” checkpoint:

- Scene starts with valid session and selected player.
- Needs visible and changing.
- Consume item works.
- Travel/location change works (one destination).
- Buy or gather works.
- Talk works (one simple interaction).
- Sleep advances day and autosaves.
- Reload restores player/session state (needs/inventory/wallet/location/social marker/day).

---

## 3) Minimal UI Required

Use one compact `VS01` panel + existing HUD:

- Existing:
  - player name
  - needs text/bars
  - wallet
  - inventory count
  - day
- Add:
  - current location text
  - `Go To Shop` (or gather point) button
  - `Talk` button
  - feedback text output

No additional scenes or advanced menus required.

---

## 4) Minimal Gameplay Systems Required

Only minimal systems needed to finish VS01 Phase 1:

1. **Session bootstrap** (already present)
2. **Needs runtime** (already present)
3. **Item consume** (already present)
4. **One acquisition path** (already present via buy; gather optional)
5. **Location travel stub** (new)
6. **Simple social interaction stub** (new)
7. **Sleep/end-day** (already present)
8. **Save/reload persistence** (already present; extend with location/social marker)

Keep all advanced systems stubbed.

---

## 5) How to Test Each Batch

## Batch A tests (Travel/Location)

1. Start from new game.
2. Press `Go To Shop`.
3. Confirm location label changes.
4. Save, reload validate.
5. Confirm location remains `shop_point`.

Pass condition: location persists through reload.

---

## Batch B tests (Talk)

1. Start/load session.
2. Press `Talk` once.
3. Confirm feedback appears and social marker increments/updates.
4. Save and reload.
5. Confirm marker state is retained.

Pass condition: one social interaction can be performed and persisted.

---

## Batch C tests (Full Day Loop)

1. Start/wake in scene.
2. Check needs visible.
3. Consume one item.
4. Travel to location.
5. Buy/gather one item.
6. Talk once.
7. Sleep/end day.
8. Reload validation.
9. Confirm all key state preserved:
   - player identity
   - household count/context
   - genetics loaded flag
   - needs
   - wallet/inventory
   - location
   - social marker
   - day count

Pass condition: complete loop works end-to-end with no missing step.

---

## Notes to Keep Architecture Intact

- Continue using use-cases from presentation controllers.
- Keep persistence via `IGameSaveStore` path only.
- Keep session state in `GameSessionContext` and `SessionContextRegistry`.
- Add only minimal new fields for location/social persistence.
- Avoid introducing manager singletons or cross-layer shortcuts.

## Age Timeline Journey (Incremental, No Major Architecture)

- **Now (implemented):** life stages + sub-stages are visible in VS01 HUD/interaction panel and automatically progress on a prototype in-game year cadence.
- **Next:** add small age-milestone gameplay nudges (e.g., mood/stress modifiers and social flavor text) without changing data architecture.
- **Then:** attach age-stage hints to NPC interactions to improve roleplay readability.
- **Later:** tune year length and milestone rewards based on playtest pacing feedback.

### Prototype cadence
- 30 in-game days = 1 in-game year.
- At each in-game year boundary, player and household members age up.
- Stage/sub-stage transitions are announced in sleep/day-end feedback.

### Extended social-event and hobby variety (content expansion)
Added additional event/hobby breadth to keep days feeling different:
- New social events: concert, sports tournament, book fair, art show, harvest fair, science expo, charity drive, block party, talent show, night market.
- New hobbies: woodworking, photography, birdwatching, knitting, music production, dance, baking, chess, volunteering, writing, astronomy, hiking.
