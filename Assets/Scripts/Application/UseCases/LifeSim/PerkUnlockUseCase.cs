using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class PerkUnlockUseCase
    {
        public bool TryUnlock(GameSessionContext context, string perkId, string requiredSkill, int requiredLevel, out string message)
        {
            message = string.Empty;
            if (context == null || string.IsNullOrWhiteSpace(perkId))
            {
                message = "Invalid perk request.";
                return false;
            }

            context.progression ??= new ProgressionState();
            if (context.progression.unlockedPerkIds.Contains(perkId))
            {
                message = $"Perk already unlocked: {perkId}.";
                return false;
            }

            var level = context.progression.GetSkillLevel(requiredSkill);
            if (level < requiredLevel)
            {
                message = $"Need {requiredSkill} level {requiredLevel}.";
                return false;
            }

            context.progression.unlockedPerkIds.Add(perkId);
            message = $"Unlocked perk: {perkId}.";
            return true;
        }
    }
}
