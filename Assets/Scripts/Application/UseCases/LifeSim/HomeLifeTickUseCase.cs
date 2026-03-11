using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    /// <summary>
    /// Small life-sim tick for home cleanliness, mood pressure, and neighborhood drift.
    /// </summary>
    public class HomeLifeTickUseCase
    {
        public void Execute(GameSessionContext context, float inGameMinutes)
        {
            if (context == null || context.home == null || context.playerCharacter?.needsStatus == null)
            {
                return;
            }

            var home = context.home;
            var needs = context.playerCharacter.needsStatus;

            var hours = inGameMinutes / 60f;
            home.cleanliness = NeedsStatus.ClampToRange(home.cleanliness - (0.4f * hours));

            if (home.cleanliness < 35f)
            {
                needs.mood = NeedsStatus.ClampToRange(needs.mood - (0.8f * hours));
            }

            // Better-kept homes slightly improve neighborhood reputation over time.
            if (home.cleanliness > 70f)
            {
                home.neighborhoodReputation = NeedsStatus.ClampToRange(home.neighborhoodReputation + (0.15f * hours));
            }
            else if (home.cleanliness < 30f)
            {
                home.neighborhoodReputation = NeedsStatus.ClampToRange(home.neighborhoodReputation - (0.2f * hours));
            }

            ApplyHouseholdTension(context, hours);
            ApplySharedResourcePressure(context, hours);
            ApplyHobbyRecovery(context, hours);
        }


        private static void ApplyHouseholdTension(GameSessionContext context, float hours)
        {
            var memberCount = context.householdMembers?.Count ?? 0;
            if (memberCount <= 0)
            {
                context.lastHouseholdEvent = "Household: living solo.";
                return;
            }

            var needs = context.playerCharacter.needsStatus;
            var pressure = 0.12f * hours * memberCount;
            if (context.home.cleanliness < 40f) pressure += 0.18f * hours;
            if (needs.stress > 60f) pressure += 0.22f * hours;
            if (context.isWeekend) pressure += 0.08f * hours;

            if (context.currentZone == Domain.Common.ZoneType.Home && context.hourOfDay >= 18f)
            {
                context.familyTension = NeedsStatus.ClampToRange(context.familyTension + pressure);
            }
            else
            {
                context.familyTension = NeedsStatus.ClampToRange(context.familyTension - (0.1f * hours));
            }

            if (context.familyTension >= 70f)
            {
                needs.mood = NeedsStatus.ClampToRange(needs.mood - (0.9f * hours));
                needs.stress = NeedsStatus.ClampToRange(needs.stress + (0.8f * hours));
                context.lastHouseholdEvent = "Household conflict spike: everyone is tense.";
            }
            else if (context.familyTension >= 45f)
            {
                needs.mood = NeedsStatus.ClampToRange(needs.mood - (0.35f * hours));
                context.lastHouseholdEvent = "Household friction is building.";
            }
            else
            {
                context.lastHouseholdEvent = "Household feels stable.";
            }
        }

        private static void ApplySharedResourcePressure(GameSessionContext context, float hours)
        {
            if (context.inventory == null || context.home == null)
            {
                return;
            }

            var memberCount = context.householdMembers?.Count ?? 0;
            var totalPeople = 1 + memberCount;
            var used = context.inventory.GetTotalItemCount();
            var capacity = context.home.GetStorageCapacity();
            var freeSlots = capacity - used;

            if (freeSlots <= totalPeople)
            {
                context.familyTension = NeedsStatus.ClampToRange(context.familyTension + (0.45f * hours));
                context.playerCharacter.needsStatus.stress = NeedsStatus.ClampToRange(context.playerCharacter.needsStatus.stress + (0.35f * hours));
                context.lastHouseholdEvent = $"Shared storage strain: {used}/{capacity} slots in use.";
            }
        }

        private static void ApplyHobbyRecovery(GameSessionContext context, float hours)
        {
            if (context.currentZone != Domain.Common.ZoneType.Home || context.hourOfDay < 18f)
            {
                return;
            }

            var hobbySkill = ResolvePrimaryHobbySkill(context.progression);
            if (string.IsNullOrWhiteSpace(hobbySkill))
            {
                return;
            }

            var skill = context.progression.skills.Find(s => s.skillId == hobbySkill);
            if (skill == null)
            {
                return;
            }

            ReduceStressFromHobby(context, hours, skill.level);
            context.progression.GainSkillXp(hobbySkill, 1.5f * hours);
        }

        private static void ReduceStressFromHobby(GameSessionContext context, float hours, int level)
        {
            var reduction = (0.6f + (level * 0.08f)) * hours;
            context.playerCharacter.needsStatus.stress = NeedsStatus.ClampToRange(context.playerCharacter.needsStatus.stress - reduction);
            context.playerCharacter.needsStatus.mood = NeedsStatus.ClampToRange(context.playerCharacter.needsStatus.mood + (0.3f * hours));
        }

        private static string ResolvePrimaryHobbySkill(ProgressionState progression)
        {
            if (progression?.skills == null || progression.skills.Count == 0)
            {
                return string.Empty;
            }

            var candidateSkills = new[] { "cooking", "fishing", "crafting", "fitness", "medicine" };
            SkillProgress best = null;
            foreach (var id in candidateSkills)
            {
                var found = progression.skills.Find(s => s.skillId == id);
                if (found == null)
                {
                    continue;
                }

                if (best == null || found.level > best.level)
                {
                    best = found;
                }
            }

            return best?.skillId ?? string.Empty;
        }
    }
}
