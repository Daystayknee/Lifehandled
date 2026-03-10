using System;
using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class SkillProgressionUseCase
    {
        public bool GainXp(GameSessionContext context, string skillId, float amount, out string message)
        {
            message = string.Empty;
            if (context == null || string.IsNullOrWhiteSpace(skillId) || amount <= 0f)
            {
                message = "Invalid progression request.";
                return false;
            }

            context.progression ??= new ProgressionState();
            var previousLevel = context.progression.GetSkillLevel(skillId);
            context.progression.GainSkillXp(skillId, amount);
            var currentLevel = context.progression.GetSkillLevel(skillId);
            var leveled = currentLevel > previousLevel;

            message = leveled
                ? $"{skillId} leveled to {currentLevel}."
                : $"{skillId} gained {amount:0} XP.";
            return true;
        }
    }
}
