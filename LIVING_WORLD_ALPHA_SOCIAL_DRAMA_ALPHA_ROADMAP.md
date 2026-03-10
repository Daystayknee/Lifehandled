# Living World Alpha + Social Drama Alpha

This is the **next milestone roadmap** built on the current runtime architecture:
- `GameSessionContext` + existing runtime state models
- `Application/UseCases/*` simulation/action flows
- `PrototypeWorldContentCatalog` as data/content source
- `Presentation/Session/*` controllers for player-facing feedback
- Existing save envelope + compatibility normalization

The goal is to make VS01 feel **truly alive** without major architecture rewrites.

---

## Scope and implementation style

- Incremental delivery in small vertical slices (1–2 weeks each).
- Every slice updates systems, content, UI feedback, and tuning.
- Preserve compatibility with current save/load shape.
- Add fields conservatively and backfill defaults in compatibility layer.

---

## Stage A (Weeks 1–3): Environmental Reactivity Foundation

### 1) Systems work
- Strengthen weather effects in `WorldSimulationTickUseCase` and related ticks:
  - weather intensity tiers (light/moderate/severe)
  - humidity/wind/chill proxies that influence needs/stress/travel risk
- Add environmental condition flags per active zone (muddy, slippery, low visibility, heat stress).
- Expand wildlife encounter resolver to include zone + time block + weather influence (not only static weights).
- Add lightweight “daily calendar state” in runtime context (day tag + event phase morning/day/evening).

### 2) Content/data work
- Add weather condition presets and environmental hazard tables by zone.
- Add first seasonal/cultural event catalog set (10–15 event templates) with tags:
  - holiday / civic / local-culture / emergency
- Add wildlife behavior tags per species:
  - diurnal/nocturnal, rain-averse/rain-active, seasonal migration, skittish/aggressive.
- Add baseline zone climate profiles (town, forest, lake, clinic, apartments, workplace).

### 3) UI/feedback work
- HUD: show weather intensity + practical impact text (e.g., “Storm: travel stress high”).
- Zone panel: show current environmental conditions and wildlife activity hint.
- Interaction feedback: append cause text (“Failed forage due to storm visibility penalty”).
- Add compact “Today in town” panel: active calendar event + weather warning.

### 4) Tuning/balancing work
- Keep weather penalties meaningful but not punishing (soft pressure, not hard lockout).
- Ensure at least 1–2 viable actions in every severe-weather state.
- Tune wildlife probability curves so weather changes are noticeable but not random chaos.

---

## Stage B (Weeks 3–6): NPC Routine + Social Response Alpha

### 1) Systems work
- Expand NPC routine logic in `NpcScheduleTickUseCase` with context-aware schedule modifiers:
  - weather, social event type, weekday/weekend-like pattern, work/rest phase.
- Add reaction drivers in social interaction use-cases:
  - recent weather hardships, event mood, household tension spillover.
- Introduce short-term social memory windows (today/recent days) and long-term memory weights.
- Add gossip propagation pass:
  - key interactions seed rumor tags
  - nearby/socially-linked NPCs receive diluted rumor impact.

### 2) Content/data work
- Add routine presets by archetype (shop owner, teacher, mechanic, nurse, etc.) for event/weather variants.
- Add response phrase banks by mood/event/reputation bucket.
- Add rumor/drama topic templates:
  - theft, kindness, rivalry, romance, household conflict, community support.
- Add social thresholds per archetype (forgiveness, suspicion, gossip susceptibility).

### 3) UI/feedback work
- NPC panel: display “current mood driver” (e.g., “Upset by protest crowd”).
- Conversation feedback: include which factor influenced outcome (reputation, rumor, memory, event).
- Add simple social feed/log section (recent gossip/drama headlines).
- Add daily summary line for “social climate” (calm / tense / celebratory).

### 4) Tuning/balancing work
- Cap rumor stacking and avoid runaway negativity spirals.
- Ensure positive recovery loops exist (helping actions can counter bad reputation).
- Tune routine shifts so NPC availability feels dynamic but still predictable enough for planning.

---

## Stage C (Weeks 6–9): Reputation + Economy/Social Consequence Layer

### 1) Systems work
- Deepen reputation effects across shops/jobs/NPC reactions:
  - shop price modifiers and stock availability
  - job income/opportunity gates
  - NPC openness/trust/fear baselines
- Add district/zone reputation sub-scores plus global reputation synthesis.
- Add household tension effects into day loop:
  - stress/mood decay modifiers
  - social action difficulty impacts
  - home-rest effectiveness reduction when tension is high.
- Extend `DailyEconomySettlementUseCase` and social use-cases to consume these modifiers consistently.

### 2) Content/data work
- Add reputation tier definitions (hostile/neutral/respected/admired) with effect tables.
- Add job/shop policy profiles by tier (discounts, refusals, bonus offers, suspicion checks).
- Add household social-life event templates:
  - arguments, reconciliation, shared meals, community invites.
- Add consequence event text packs for positive and negative arcs.

### 3) UI/feedback work
- HUD: expose reputation tier + trend arrow (rising/falling/stable).
- Shop/job interactions: preview current reputation effect before confirming action.
- Household panel: show tension meter + “what is driving it” hints.
- End-of-day report: include reputation movement, rumor heat, household climate, and resulting modifiers for next day.

### 4) Tuning/balancing work
- Avoid hard fail states from reputation alone; keep recovery paths visible.
- Tune price/job modifiers to be significant but not economy-breaking.
- Balance household tension impact to encourage care actions without forcing repetitive chores.

---

## Stage D (Weeks 9–12): Social Drama Alpha Cohesion Pass

### 1) Systems work
- Connect systems into coherent chains:
  - Weather/Calendar -> NPC Routine -> Social Interaction -> Gossip/Drama -> Reputation -> Economy/Household outcomes.
- Add lightweight scenario triggers for emergent “drama beats” when multiple pressures align.
- Ensure all new states persist and normalize through save compatibility.

### 2) Content/data work
- Add “living world packs” combining event + weather + wildlife + social consequences.
- Add relationship/drama arc templates (rival escalation, romance setbacks, community redemption).
- Expand cultural calendar with seasonal signature event weekends and consequences.

### 3) UI/feedback work
- Add “Why this happened” breakdown in key outcomes.
- Add contextual alerts for major world shifts (storm warning, festival prep, protest tension).
- Add a timeline strip for last 24h world/social events to improve perceived causality.

### 4) Tuning/balancing work
- Validate pacing so each in-game day has variety (environment + social + progression choices).
- Reduce noisy feedback; prioritize high-signal events.
- Run repeated full-day and multi-day simulations for stability and fun before feature lock.

---

## Cross-cutting implementation checklist (for each slice)

1. **Systems**
   - Implement in existing use-cases/session state.
   - Keep interfaces simple and backward-compatible.
2. **Content/Data**
   - Add definitions in catalog with deterministic fallback values.
3. **UI/Feedback**
   - Surface at least one actionable player-facing indicator for every new mechanic.
4. **Tuning/Balancing**
   - Add/adjust constants only after smoke simulation runs.

---

## Suggested first three implementation slices (immediate)

### Slice 1 (Week 1)
- Weather intensity + zone condition flags
- HUD weather impact line
- Wildlife weather/time weighting v1

### Slice 2 (Week 2)
- Calendar event phase + “Today in town” HUD widget
- NPC schedule modifiers by event/weather
- Conversation feedback includes one explicit reason tag

### Slice 3 (Week 3)
- Reputation tier effects on shop pricing
- Household tension impact on sleep/home comfort outcomes
- End-of-day summary with reputation + social climate + household tension

These slices deliver immediate “alive world” feel with minimal architectural risk.
