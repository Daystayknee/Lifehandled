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

            ApplyHobbyRecovery(context, hours);
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
            skill.xp += 1.5f * hours;
            while (skill.xp >= (20f + (skill.level * 10f)))
            {
                skill.xp -= 20f + (skill.level * 10f);
                skill.level += 1;
            }
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
