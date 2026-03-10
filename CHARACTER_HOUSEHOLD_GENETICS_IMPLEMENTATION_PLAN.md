# Character / Household / Genetics: Actionable Implementation Plan (V1-safe)

This plan turns the foundation pass into buildable steps for the current Unity project while preserving Vertical Slice 01 (VS01).

Primary goals:
- keep prototype playable first
- keep architecture modular (Domain/Application/Infrastructure/Presentation)
- avoid big-bang rewrites
- keep inheritance/multigenerational support forward-compatible, but lightweight for now

---

## 0) Guardrails (Before Coding)

- Keep all new logic behind interfaces and use-cases where possible.
- Keep current gameplay loop unchanged: wake → needs → eat/drink → travel → gather/buy → talk → money → sleep → save.
- Add only minimal new runtime behavior for household/genetics in V1.
- Use DTO/save-safe IDs only (no scene object references in persisted state).

---

## 1) Exact Implementation Order

Implement in this order to minimize risk:

1. **Data Contracts First**
   - Add Character/Household/Relationship/Appearance/Genetic DTO/domain models.
2. **Save Envelope Extension**
   - Add minimal save schema fields and migration-safe defaults.
3. **Creation Services**
   - Implement New Game creation use-case/services (main character + optional household templates).
4. **UI (Minimal Setup Panels)**
   - Add compact New Game setup UI (no advanced creator).
5. **Runtime Integration (Lightweight)**
   - Wire created data into session start.
6. **Genetics Modifiers (Narrow Scope)**
   - Add 2–3 conservative gameplay modifiers to existing systems.
7. **Household V1 Behavior**
   - Implement passive/limited household behavior.
8. **Validation + Fallbacks**
   - Add null-safe/default-safe loading and test passes.

---

## 2) First Scripts / Files / Data Objects to Create

Use these as first concrete additions (names can be adjusted to repo conventions):

## Domain models

- `Assets/Scripts/Domain/Character/Character.cs`
- `Assets/Scripts/Domain/Character/IdentityProfile.cs`
- `Assets/Scripts/Domain/Character/AppearanceProfile.cs`
- `Assets/Scripts/Domain/Character/GeneticProfile.cs`
- `Assets/Scripts/Domain/Character/LineageProfile.cs`
- `Assets/Scripts/Domain/Household/Household.cs`
- `Assets/Scripts/Domain/Social/RelationshipLink.cs`
- `Assets/Scripts/Domain/Social/RelationshipStats.cs`

## Application ports/services

- `Assets/Scripts/Application/Ports/ICharacterRepository.cs`
- `Assets/Scripts/Application/Ports/IHouseholdRepository.cs`
- `Assets/Scripts/Application/Ports/IRelationshipRepository.cs`
- `Assets/Scripts/Application/Ports/IGeneticService.cs`
- `Assets/Scripts/Application/Ports/ICharacterCreationService.cs`
- `Assets/Scripts/Application/UseCases/NewGame/InitializeNewGameUseCase.cs`

## Infrastructure (initial)

- `Assets/Scripts/Infrastructure/Persistence/DTO/CharacterDto.cs`
- `Assets/Scripts/Infrastructure/Persistence/DTO/HouseholdDto.cs`
- `Assets/Scripts/Infrastructure/Persistence/DTO/RelationshipLinkDto.cs`
- `Assets/Scripts/Infrastructure/Persistence/DTO/GeneticProfileDto.cs`
- `Assets/Scripts/Infrastructure/Persistence/Mappers/CharacterMapper.cs`

## ScriptableObject data seeds (minimal)

- `Assets/ScriptableObjects/NewGame/CharacterPreset.asset`
- `Assets/ScriptableObjects/NewGame/HouseholdTemplate.asset`
- `Assets/ScriptableObjects/Genetics/GeneModifierConfig.asset`

---

## 3) Minimum Save Schema Updates Required

Add only these fields now:

```text
SaveGameEnvelope
- saveVersion
- playerCharacterId
- characters[]
- households[]
- relationshipLinks[]
- geneticSchemaVersion
```

### Required defaults/migration behavior

- If `characters[]` missing: construct single fallback character from old player state mapping.
- If `households[]` missing: create implicit solo household for player.
- If `relationshipLinks[]` missing: empty list.
- If `geneticSchemaVersion` missing: assume version `1` and initialize default genes.

### Minimum integrity checks on load

- `playerCharacterId` must exist in `characters[]`.
- Each `Household.MemberCharacterIds` must reference existing characters.
- Each relationship endpoint must reference existing characters.

If validation fails, degrade gracefully to solo-player fallback, log warning, and keep session playable.

---

## 4) Minimum New Game Flow Needed for V1

Keep flow short (3 steps):

1. **Main Character Setup**
   - Name input
   - Choose one preset appearance/background
2. **Household Setup (Optional)**
   - Mode: Solo / +1 member / +2 members
   - Pick templates for optional members (friend/roommate/family)
3. **Confirm & Start**
   - Generate genetics for all created characters
   - Create relationship links and household
   - Spawn at starter home, begin VS01 day

No deep trait slider editing, no lineage editor, no advanced family tree screen.

---

## 5) Minimum UI Screens / Panels for Character + Household Setup

Add only these panels (single scene or stacked modal flow):

1. `NewGameLandingPanel`
   - New Game / Back
2. `MainCharacterPanel`
   - Name field
   - Preset selector (appearance + starter archetype)
3. `HouseholdPanel`
   - Household mode dropdown (Solo, +1, +2)
   - Template pickers for optional members
   - Basic relationship type picker (Friend/Roommate/Family)
4. `NewGameSummaryPanel`
   - Read-only summary
   - Confirm Start

### UI constraints for V1

- Reuse existing UI components/style system.
- No body morph sliders.
- Max 2 optional members.
- No custom member-by-member deep editing yet.

---

## 6) First Genetics Gameplay Modifiers to Implement

Implement exactly these first (small, deterministic, low risk):

1. **MetabolismRate**
   - modifies hunger/thirst decay (`~0.9x to 1.1x`)
2. **SleepRecoveryEfficiency**
   - modifies energy restored on sleep (`~0.9x to 1.1x`)
3. **StressSensitivity**
   - modifies stress gain / mood penalty sensitivity (`~0.9x to 1.1x`)

### Implementation note

- Store as normalized gene values (0..1).
- Convert to clamped multipliers through `IGeneticModifierProvider`.
- Keep effects subtle so player behavior remains dominant.

---

## 7) Optional Household Members: V1 Behavior

In V1, optional members should be **lightweight companions**, not full simulation actors.

### Required V1 behavior

- Exist in household data and save/load.
- Have basic identity/appearance/genetics/relationship state.
- Be available for simple social interactions (dialogue stubs/short interactions).
- Contribute optional household flavor events (non-blocking).

### Not required in V1

- Full autonomous AI loops with complete needs simulation.
- Complex pathfinding/schedules equivalent to major NPC systems.
- Deep interpersonal drama engine.

### Practical runtime mode

- "Presence + schedule stub" approach:
  - home presence windows
  - occasional interaction availability
  - low update frequency

---

## 8) Stubs / Placeholders for Later

Create placeholders now to preserve architecture seams:

- `ILineageService` (stub)
- `IInheritanceResolver` (stub)
- `FamilyTreeReadModel` (stub)
- advanced genetics calculators (stub)
- reproduction/offspring pipeline (stub)
- multi-generation event systems (stub)

Each stub should have clear TODO notes and no runtime dependency in VS01 path.

---

## 9) Done Criteria for Each Step

## Step 1 — Data Contracts
- [ ] Domain models compile and are serializable/mappable.
- [ ] IDs are stable and unique.
- [ ] No Unity scene refs in persisted models.

## Step 2 — Save Schema Update
- [ ] Envelope fields added.
- [ ] Load defaults for missing fields implemented.
- [ ] Integrity checks and solo fallback implemented.

## Step 3 — Creation Services
- [ ] New game use-case can create player + optional members + household + relationships.
- [ ] Founder genetics generated for all created characters.

## Step 4 — Minimal UI
- [ ] 4 panels implemented (Landing/MainCharacter/Household/Summary).
- [ ] Flow supports Solo/+1/+2 and starts game successfully.

## Step 5 — Runtime Integration
- [ ] Session starts from generated new-game data.
- [ ] VS01 loop remains playable end-to-end.

## Step 6 — Genetics Modifiers
- [ ] 3 modifiers live (metabolism, sleep recovery, stress sensitivity).
- [ ] Modifiers clamped and tested for safe ranges.

## Step 7 — Household V1 Behavior
- [ ] Members persist and can be interacted with minimally.
- [ ] No heavy AI/schedule dependency required.

## Step 8 — Forward-compat Stubs
- [ ] Lineage/inheritance interfaces exist as non-blocking stubs.
- [ ] Save schema supports future extension without breaking V1.

---

## 10) PR-by-PR / Batch-by-Batch Build Sequence

## PR-1: Data + Interfaces + DTOs

**Includes**
- Domain models
- repositories/ports interfaces
- DTOs + mappers

**Excludes**
- gameplay behavior/UI

**Done**
- compile pass
- serialization sanity pass

---

## PR-2: Save Envelope Minimal Extension

**Includes**
- envelope fields
- migration/default logic
- integrity validation + fallback strategy

**Done**
- old-save compatibility path tested
- new-save roundtrip tested

---

## PR-3: New Game Use-Case + Service Layer

**Includes**
- `InitializeNewGameUseCase`
- founder gene generation
- household + relationship bootstrap

**Done**
- headless/service-level new-game creation test passes

---

## PR-4: Minimal New Game UI Panels

**Includes**
- 4 compact setup panels
- data binding into new-game use-case

**Done**
- manual flow works for Solo/+1/+2

---

## PR-5: Genetics Gameplay Modifiers (3 only)

**Includes**
- metabolism/sleep/stress modifier hooks
- conservative clamped tuning config

**Done**
- modifiers visibly affect metrics but do not destabilize loop

---

## PR-6: Optional Household Runtime Behavior (Light)

**Includes**
- member presence stubs
- simple interaction enablement

**Done**
- members appear in household context and are save/load stable

---

## PR-7: Forward-Compat Stubs + Polish

**Includes**
- lineage/inheritance placeholder interfaces + docs
- cleanup, null-safe guards, telemetry/logging

**Done**
- VS01 remains fully playable
- no dependency on unimplemented advanced genetics systems

---

## Final V1 Acceptance Criteria

- [ ] Main character creation works.
- [ ] Optional household members (0..2) supported.
- [ ] Genetics exists and impacts exactly 3 core mechanics in subtle ways.
- [ ] Save/load supports new data with backward-safe defaults.
- [ ] VS01 gameplay loop remains intact and stable.
- [ ] Architecture remains modular and ready for future inheritance/multi-generation expansion.
