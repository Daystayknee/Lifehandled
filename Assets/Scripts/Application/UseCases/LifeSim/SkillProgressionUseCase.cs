using System;
using System.Linq;
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
            var skill = context.progression.skills.FirstOrDefault(s => s.skillId == skillId);
            if (skill == null)
            {
                skill = new SkillProgress { skillId = skillId, level = 1, xp = 0f };
                context.progression.skills.Add(skill);
            }

            skill.xp += amount;
            var leveled = false;
            while (skill.xp >= RequiredXp(skill.level))
            {
                skill.xp -= RequiredXp(skill.level);
                skill.level += 1;
                leveled = true;
            }

            message = leveled
                ? $"{skillId} leveled to {skill.level}."
                : $"{skillId} gained {amount:0} XP.";
            return true;
        }

        private static float RequiredXp(int level)
        {
            return 20f + (level * 10f);
        }
    }
}
