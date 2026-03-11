# Lifehandled VS01 — How to Run (For Dummies)

This guide is intentionally simple. If you can click buttons in Unity, you can run this.

---

## 1) What you need

- **Unity Editor** (use the project’s expected version from your team setup/Unity Hub).
- This repo checked out locally.
- Basic ability to open a Unity scene and press Play.

If Unity says packages need updating, let it finish before trying to play.

---

## 2) Open the project

1. Open **Unity Hub**.
2. Click **Add project from disk**.
3. Select this repo folder (`Lifehandled`).
4. Open the project.
5. Wait for import/compilation to finish.

You are ready when the Console is not spamming compile errors.

---

## 3) Find the VS01 scene

1. In Unity, open the **Project** window.
2. Find the scene used for the VS01 prototype loop.
3. Open it.

If you’re not sure which scene is correct, ask your teammate for the one that contains:
- `Vs01SessionBootstrap`
- `Vs01NeedsRuntimeController`
- `Vs01InteractionPanelController`
- `Vs01SessionHudController`

Those are the core runtime drivers/controllers.

---

## 4) Wire required references (one-time scene setup check)

Select objects with these components and verify references are assigned in Inspector:

### A) `Vs01SessionBootstrap`
- This is the startup composition point that initializes session state.

### B) `Vs01NeedsRuntimeController`
- Check tick values:
  - `realSecondsPerTick` (how often runtime updates)
  - `inGameMinutesPerTick` (how fast game time moves)
- Leave defaults unless testing pace.

### C) `Vs01InteractionPanelController`
- Ensure UI button/text references are assigned.
- If references are null, button clicks won’t do anything.

### D) `Vs01SessionHudController`
- Ensure HUD text/slider references are assigned.

---

## 5) Press Play and verify baseline loop

1. Press **Play**.
2. Confirm HUD shows a live session (not “missing”).
3. Click a few core actions:
   - Consume
   - Buy
   - Travel
   - Talk
   - Sleep
4. Confirm feedback text updates.

If this works, VS01 runtime is alive.

---

## 6) Save/Load basics

The prototype uses a JSON save adapter (`JsonNewGameSaveStore`).

- Use **Save** button from interaction panel.
- Use **Reload Validate** button to confirm save can be read/validated.
- Use **Sleep** to auto-advance day and trigger additional progression messages.

If reload fails, check Console for compatibility/default warnings from save compatibility layer.

---

## 7) What to test manually every time (quick smoke checklist)

- [ ] Session boots without null errors.
- [ ] HUD updates each tick.
- [ ] Travel changes zone text and context.
- [ ] Social interaction returns reaction text.
- [ ] Sleep advances day and applies economy/needs changes.
- [ ] Save + reload validation succeeds.

Optional now that age timeline exists:
- [ ] Day progression eventually reports age timeline milestones.

---

## 8) Most common issues + fixes

### “Buttons do nothing”
- Usually missing UI references on `Vs01InteractionPanelController`.

### “Session context missing” in HUD
- Bootstrap object/component not active in scene.

### Repeated null errors in Update
- A controller reference is missing (text/slider/button list).

### Save/reload warnings
- Usually legacy/partial save payloads being normalized by compatibility logic.

---

## 9) How to tune pace quickly (designer-friendly)

In `Vs01NeedsRuntimeController`:
- Increase `inGameMinutesPerTick` to speed up day flow.
- Decrease it to make decisions less rushed.

In world/economy/survival use-cases:
- Tweak deltas gradually (small increments first).

---

## 10) Golden rule for this prototype

Do not bypass use-cases from UI scripts.

Keep behavior changes inside the existing runtime architecture:
- Session state in `GameSessionContext`
- Runtime tick in `Vs01NeedsRuntimeController`
- Behavior in `Application/UseCases/*`
- Persistence via save store + envelope compatibility

That keeps the vertical slice stable as features expand.


## Paper Doll (2D AAA pivot) quick setup

1. Create a `Player` GameObject.
2. Add child layer GameObjects with `SpriteRenderer` components:
   - `BaseBody` (sorting order 0)
   - `Eyes` (sorting order 1)
   - `Nose` (sorting order 2)
   - `Lips` (sorting order 3)
   - `Makeup` (sorting order 4)
   - `Details_Freckles` (sorting order 5)
   - `Details_BeautyMarks` (sorting order 6)
3. Add `CharacterRenderer` to `Player` and map each `LayerSlot` to its child renderer.
4. Create a `CharacterIdentity` ScriptableObject and assign initial sprites/stats/trait.
5. Add `CharacterCreatorUI` to your creator canvas and wire its button events (`CycleEyes`, `CycleNose`, `ToggleFreckles`, `ToggleBeautyMarks`).
6. Create one `EnvironmentProfile` asset per area (Mall/Park/Hospital), set background + stat modifiers.
7. Add `EnvironmentManager` + `SurvivalSystem` in the scene, assign `CharacterIdentity` and profiles.

This keeps visuals decoupled from survival mechanics and supports scene changes without losing identity data.
