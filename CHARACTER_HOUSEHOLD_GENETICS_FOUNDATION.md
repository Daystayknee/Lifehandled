# Character / Household / Genetics Foundation Pass (Pre-VS01)

This document extends the current architecture with a **minimal, modular foundation** for:

- one player-created main character
- optional starting household members (friend/roommate/family)
- genetics as a meaningful but lightweight gameplay layer
- future inherited-trait and multi-generation support

The goal is to stay incremental and compatible with Vertical Slice 01.

---

## Design Principles

1. **Single source of truth per domain** (Character, Household, Genetics).
2. **Data-first modeling** (DTOs + IDs, not scene object references).
3. **Prototype-safe genetics** (small number of gameplay hooks, no simulation explosion).
4. **Forward compatibility** (fields and IDs ready for inheritance and generations later).
5. **No giant creator UI in V1** (simple flow, expandable later).

---

## 1) Data Models

All models should use stable IDs (GUID/string IDs) and be serializable as plain save DTOs.

## 1.1 Character

```csharp
Character
- CharacterId : string
- IsPlayerControlled : bool
- Role : CharacterRole            // PlayerMain, HouseholdMember, NPC
- Identity : IdentityProfile
- Appearance : AppearanceProfile
- Genetics : GeneticProfile
- CoreStats : CoreStatBlock       // health/stamina/mood baseline container
- NeedsState : NeedsStateSnapshot // hunger, thirst, energy, hygiene, warmth
- TraitIds : List<string>         // gameplay traits (non-genetic + derived)
- SkillStates : List<SkillState>
- HouseholdId : string?            // null for non-household NPC
- RelationshipLinkIds : List<string>
- Lifecycle : LifecycleState       // age stage + fertility flags (later-ready)
- Metadata : CharacterMetadata     // createdAt, sourceArchetype, etc.
```

### IdentityProfile

```csharp
IdentityProfile
- DisplayName : string
- AgeYears : int
- Sex : SexType                    // design-facing enum; keep inclusive/extensible
- Pronouns : string
- OccupationTag : string?
- CultureTag : string?
```

---

## 1.2 Household

```csharp
Household
- HouseholdId : string
- HouseholdName : string
- HomeLocationId : string
- MemberCharacterIds : List<string>
- HouseholdType : HouseholdType    // Solo, Roommates, Family, Mixed
- SharedFunds : CurrencyAmount
- SharedInventoryId : string?
- RentObligation : RecurringCost
- UtilityObligation : RecurringCost
- Rules : HouseholdRuleSet         // sleeping, chores, access permissions
- ReputationTag : string?
- CreatedAtDay : int
```

### HouseholdRuleSet (V1-minimal)

```csharp
HouseholdRuleSet
- UsesSharedFunds : bool
- UsesSharedStorage : bool
- AutoPayRentFromSharedFunds : bool
```

---

## 1.3 RelationshipLink

```csharp
RelationshipLink
- RelationshipLinkId : string
- CharacterAId : string
- CharacterBId : string
- LinkType : RelationshipType      // Family, Friend, Roommate, Rival, Romantic, etc.
- Stats : RelationshipStats
- KnownSinceDay : int
- IsHouseholdBond : bool           // true if same household context
- MemoryTagIds : List<string>      // short references, not full logs in V1
```

### RelationshipStats (starter)

```csharp
RelationshipStats
- Familiarity : float   // 0..100
- Trust : float         // 0..100
- Affection : float     // 0..100
- Tension : float       // 0..100
- Respect : float       // 0..100
```

---

## 1.4 AppearanceProfile

Use deterministic, seed-driven appearance so future inheritance can derive from parents.

```csharp
AppearanceProfile
- AppearanceSeed : int
- BodyFrame : BodyFrameType
- SkinToneIndex : int
- HairStyleId : string
- HairColorId : string
- EyeColorId : string
- DistinctiveFeatureIds : List<string>
- OutfitPresetId : string
```

**Rule:** In V1, appearance is mostly cosmetic except limited social modifiers (see genetics/gameplay section).

---

## 1.5 GeneticProfile

Keep genetics lightweight and tunable.

```csharp
GeneticProfile
- GenomeVersion : int
- Lineage : LineageProfile
- GeneValues : Dictionary<string, float>   // normalized 0..1 scalar genes
- GeneticTraitIds : List<string>           // derived labels, e.g., "FastMetabolism"
- HealthPredispositionIds : List<string>   // low-intensity probabilities in V1
- InheritanceMetadata : InheritanceMetadata
```

### LineageProfile

```csharp
LineageProfile
- ParentACharacterId : string?
- ParentBCharacterId : string?
- AncestorCharacterIds : List<string>      // optional in V1
```

### InheritanceMetadata

```csharp
InheritanceMetadata
- IsFounder : bool                         // true for created characters in V1
- GenerationIndex : int                    // founder=0
- GeneSource : GeneSourceType              // Randomized, Template, Inherited
```

---

## 2) What to Include Now vs Later

## Include Now (Pre-VS01)

1. Data schemas above as save-compatible DTOs.
2. New game support for:
   - one main character
   - 0..2 optional household members (template-driven)
3. Relationship links between starter household members.
4. Minimal genetics system:
   - 4–6 gene scalars that map to existing needs/stats modifiers.
5. Save/load support for Character + Household + RelationshipLink + Genetics.
6. Simple UI inputs (name + preset + household option), **not full creator**.

## Defer to Later (Post-VS01)

1. Full visual character creator (sliders/morphs/extensive cosmetics).
2. Pregnancy/childbirth/aging pipelines.
3. Advanced Mendelian/polygenic simulation with mutation events.
4. Deep family tree UI and genealogy exploration tools.
5. Complex inheritance of skills/assets/legal systems.
6. High-fidelity social memory and long-form relationship histories.

---

## 3) Safest Minimal Implementation for V1

Implement as a thin foundation layer with strict boundaries:

### V1 Foundation Package

- `Domain/Character/` for Character, AppearanceProfile, GeneticProfile models.
- `Domain/Household/` for Household and RelationshipLink aggregates.
- `Application/UseCases/NewGame/` for creation orchestration.
- `Infrastructure/Persistence/DTO/` for save DTOs.

### Minimal runtime behavior

- Household members are simulation-light entities in V1 (schedule stubs acceptable).
- Genetics modify only a small subset of existing systems (needs decay/recovery and mood resilience).
- Relationship links initialize state and provide lightweight dialogue/context modifiers.

### Why this is safest

- No deep AI rewrite required.
- No UI-heavy creator required.
- Keeps VS01 loop intact while adding forward-compatible foundations.

---

## 4) Genetics Gameplay Impact (Prototype-Safe)

Use a tiny gene set (example):

- `MetabolismRate` (needs decay multiplier)
- `StressSensitivity` (stress gain/mood drop sensitivity)
- `SleepRecoveryEfficiency` (energy restoration during sleep)
- `ThermalTolerance` (warmth decay / exposure sensitivity)
- `ImmuneRobustness` (minor illness chance modifier placeholder)

## Mapping strategy

- Convert genes (0..1) into conservative multipliers, e.g. `0.9x .. 1.1x`.
- Clamp all gameplay modifiers to narrow ranges.
- Surface as trait labels for readability (e.g., `Fast Metabolism`).

## Prototype rule

Genetics should **influence** outcomes, not dominate them. Player choices remain primary.

---

## 5) New Game Creation Flow (Incremental)

## Step 1 — Main Character Basics

- Enter name
- Select one of a few appearance presets
- Select one starter background/archetype (optional)

## Step 2 — Household Setup (Optional)

- Choose household mode:
  - Solo
  - +1 roommate/friend/family
  - +2 members
- Select member templates (name/preset/relationship type)

## Step 3 — Genetics Initialization

- For founders (all V1 starters), generate gene values by template + randomized variance.
- Derive initial genetic trait labels.

## Step 4 — Relationship Bootstrap

- Create `RelationshipLink` records among household members and player.
- Apply starting stat bands by relationship type.

## Step 5 — World Placement + Save Seed

- Assign starter home/location.
- Create initial household finances.
- Emit initial save snapshot as first checkpoint.

No advanced creator screens required in V1; this can be implemented as compact setup panels.

---

## 6) Compatibility with Vertical Slice 01

This foundation should plug into VS01 without changing its core loop.

## VS01 loop mapping

- **Wake/check needs/eat/drink:** genetics can lightly affect needs decay/recovery.
- **Travel/gather/buy/manage money:** unchanged; household can optionally share funds.
- **Talk to NPCs:** household relationship links provide immediate social context.
- **Sleep/save:** genetics modify sleep recovery; save envelope persists new structures.

## Non-breaking rule

If household/genetics data is absent (older/newer save mismatch), fallback defaults keep VS01 playable.

---

## 7) Recommended Interfaces, Events, and Save Data Updates

## 7.1 Interfaces (create first)

- `ICharacterRepository`
  - Load/store `Character` aggregate data
- `IHouseholdRepository`
  - Load/store `Household` data
- `IRelationshipRepository`
  - Load/store `RelationshipLink` data
- `IGeneticService`
  - generate founder genes
  - derive trait labels/modifiers
- `ICharacterCreationService`
  - orchestrate new game character + household setup
- `IGeneticModifierProvider`
  - exposes gameplay multipliers for needs/sleep/stress systems

Optional (next pass):
- `ILineageService`
- `IInheritanceResolver`

---

## 7.2 Integration Events

Add a small event set:

- `MainCharacterCreated`
- `HouseholdCreated`
- `HouseholdMemberAdded`
- `RelationshipLinkCreated`
- `GeneticProfileGenerated`
- `NewGameInitialized`

Use events for cross-module initialization only; avoid over-eventing every stat tick.

---

## 7.3 Save Data Envelope Updates

Extend save envelope with new sections:

```text
SaveGameEnvelope
- saveVersion
- playerCharacterId
- characters[]
- households[]
- relationshipLinks[]
- geneticSchemaVersion
```

### Save safety notes

1. Keep all references by IDs.
2. Validate referential integrity on load:
   - all household members must exist in `characters[]`
   - relationship endpoints must exist
3. Include migration stubs for absent genetics fields.
4. Default unsupported fields to safe values.

---

## Suggested Incremental PR Sequence

1. **PR-A:** Add data models + DTOs + save envelope extension (no gameplay changes).
2. **PR-B:** Add `IGeneticService` + founder generation + trait derivation.
3. **PR-C:** Add New Game setup flow (main character + optional household templates).
4. **PR-D:** Apply limited genetics modifiers to needs/sleep/stress systems.
5. **PR-E:** Add relationship bootstrap integration and VS01 validation.

Each PR should keep the project playable and reversible.

---

## Done Criteria (Foundation Pass)

- [ ] New game can create 1 main character.
- [ ] Optional 0..2 household members can be created from templates.
- [ ] Relationship links are created and saved.
- [ ] Genetic profiles are generated for all starter characters.
- [ ] At least three gameplay systems consume lightweight genetic modifiers.
- [ ] Save/load roundtrip preserves character-household-relationship-genetics integrity.
- [ ] VS01 loop remains functional end-to-end.
