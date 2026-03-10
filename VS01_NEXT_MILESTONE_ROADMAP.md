# VS01 Next Milestone Roadmap (Dream Life-Sim Survival RPG)

This roadmap builds on the **current runtime architecture** (session context + use-cases + controllers + JSON save compatibility) and avoids big rewrites.

---

## Guiding constraints

- Keep the vertical slice playable at every milestone.
- Expand using existing seams:
  - `Application/UseCases/*`
  - `Application/Content/PrototypeWorldContentCatalog`
  - `Application/Session/*`
  - `Presentation/Session/*`
- Avoid major architecture churn until milestones prove design value.

---

## Milestone 1 (0–6 weeks): “Better Daily Life Loop”

**Goal:** Make moment-to-moment play clearer and more rewarding while staying inside current systems.

### 1) Character appearance/body feature content pipeline

#### Systems work
- Add lightweight appearance option catalogs (face presets, hair groups, body variants) as data lists in existing content catalog.
- Add deterministic selection helpers driven by seed + age stage + simple tags.

#### Content work
- Add first curated packs:
  - 20+ hairstyle IDs
  - 15+ face-shape/profile IDs
  - 10+ body silhouette IDs
  - age-stage visual tags (infant/toddler/teen/adult/elder-safe variants)

#### UI work
- Extend New Game setup panel with simple selector rows (next/prev) and randomize button.
- Add compact preview labels in HUD/context (selected style IDs).

#### Tuning work
- Ensure no option causes gameplay stat spikes (appearance is flavor first).
- Keep generation/inheritance output readable and not chaotic.

---

### 2) Food and cooking expansion

#### Systems work
- Expand food effect metadata in catalog (hunger/thirst/energy/mood/stress impacts).
- Add simple recipe composition rules (2–3 ingredient combos).
- Add cooking quality tiers based on existing skill progression.

#### Content work
- Add 30–40 additional food items grouped by:
  - survival staples
  - comfort meals
  - event foods
- Add 20+ recipe definitions tied to current item IDs.

#### UI work
- Add “Cookable now” and “Missing ingredients” text in interaction panel.
- Show meal quality and immediate expected effects before confirming cook action.

#### Tuning work
- Balance cheap survival foods vs higher-quality cooked meals.
- Avoid one optimal recipe dominating all choices.

---

### 3) Clothing/fashion systems

#### Systems work
- Add outfit slots and simple outfit score aggregation (warmth/social/work-eligibility).
- Link weather and social-event context to clothing outcomes (comfort, trust/reputation nudges).

#### Content work
- Add 40+ clothing entries across casual/work/formal/weather categories.
- Add starter outfit bundles by archetype.

#### UI work
- Add minimal “Current Outfit” summary panel and quick equip buttons.
- Show weather suitability hint (“Poor for rain”, “Good for formal event”).

#### Tuning work
- Keep clothing bonuses modest; avoid hard-locking progression.
- Ensure each category has viable low-cost choices.

---

### 4) Cultural/calendar systems

#### Systems work
- Expand social-event schedule into seasonal/event calendars (still deterministic).
- Add “today/tomorrow” event forecasting function for UI.

#### Content work
- Add recurring local traditions and holiday labels tied to existing events.
- Add event-specific reward variants (social gains, market discounts, rare drops).

#### UI work
- Add compact calendar strip in HUD (today + next event).
- Add event tooltip style text in interaction panel.

#### Tuning work
- Keep event cadence varied but predictable enough for planning.
- Prevent event overload that drowns core survival loop.

---

### 5) Wildlife ecosystem depth

#### Systems work
- Add simple animal behavior tags (diurnal/nocturnal/skittish/common-rare).
- Tie encounter chances to weather/time/zone danger.

#### Content work
- Add 15–20 new wildlife definitions with distinct outputs.
- Add zone-specific wildlife tables and rare encounter notes.

#### UI work
- Add “Likely wildlife now” hint in zone panel.
- Add encounter outcome log text with reason hints.

#### Tuning work
- Keep rare encounters exciting but not mandatory for economy balance.
- Ensure low-skill players still see occasional progress.

---

### 6) Housing and transportation expansion

#### Systems work
- Add housing tier progression metadata (space/comfort/upkeep modifiers).
- Add transport mode modifiers (travel stress/time/cost).

#### Content work
- Add 10+ housing upgrades and 5+ transport options.
- Add zone travel flavor hooks by transport type.

#### UI work
- Add “Travel mode” selector and travel preview (cost/stress/time).
- Add home-upgrade shortlist card in interaction panel.

#### Tuning work
- Balance home upgrades as mid-term goals (not day-1 unlocks).
- Keep transport choices meaningful (cheap/slow vs costly/comfortable).

---

## Milestone 2 (6–12 weeks): “Neighborhood Depth + Identity”

**Goal:** Turn systems into a believable social neighborhood with stronger player identity.

### Systems work (cross-domain)
- Add daily routine templates influenced by weather/event/calendar and age stage.
- Add lightweight faction/community tags that influence NPC relationship drift.
- Add household role logic (caregiver/worker/student) affecting daily needs and actions.

### Content work
- Expand NPC archetype packs, event scripts, and home neighborhood variants.
- Add themed recipe/clothing/event bundles by culture/season.

### UI work
- Add social map view (who likes/dislikes you, where they are likely to be).
- Add compact “today’s priorities” and “tomorrow forecast” panel.

### Tuning work
- Tune relationship deltas to avoid swingy extremes.
- Tune economy sink/source curves around mid-game upgrades.

---

## Milestone 3 (12–20 weeks): “Generational RPG Layer”

**Goal:** Make the long arc (legacy + survival + life fantasy) compelling over many cycles.

### Systems work
- Expand generation handoff consequences (traits, heir context, legacy bonuses/penalties).
- Add milestone quest chains tied to age stages and household growth.
- Add risk systems (illness/injury/economic shock) with recovery loops, not just failure.

### Content work
- Add multi-generation story arcs, heir traits, family events, and legacy collectibles.
- Add deeper housing districts + transportation hubs + wildlife regions.

### UI work
- Add legacy timeline screen and family tree progression summary.
- Add chapter-like progression cards (“Early adulthood”, “Family era”, “Late legacy”).

### Tuning work
- Ensure each generation feels different but fair.
- Keep survival pressure present without constant punishment.

---

## Delivery strategy (realistic)

- Ship in thin vertical slices every 1–2 weeks.
- Every slice must include:
  1) one systems increment,
  2) one content increment,
  3) one UX visibility increment,
  4) one balancing pass.
- No milestone closes without:
  - save/load compatibility verification,
  - full day-loop smoke run,
  - at least one “new player” usability check.

---

## Suggested immediate next 3 tasks

1. Add a tiny calendar forecast readout (`today` + `tomorrow`) to HUD using existing world event tags.
2. Add cooking preview text (“expected hunger + mood delta”) before action execution.
3. Add travel mode prototype (walk vs bus) with simple cost/stress/time tradeoff.

These are high-value and low-risk within current architecture.
