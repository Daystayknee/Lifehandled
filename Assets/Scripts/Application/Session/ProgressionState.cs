using System;
using System.Collections.Generic;
using System.Linq;

namespace Lifehandled.Application.Session
{
    [Serializable]
    public class ProgressionState
    {
        public List<SkillProgress> skills = new();
        public List<string> unlockedPerkIds = new();

        public int GetSkillLevel(string skillId)
        {
            var skill = skills.Find(s => s.skillId == skillId);
            return skill?.level ?? 1;
        }

        public float GetSkillXp(string skillId)
        {
            var skill = skills.Find(s => s.skillId == skillId);
            return skill?.xp ?? 0f;
        }

        public SkillProgress EnsureSkill(string skillId)
        {
            if (string.IsNullOrWhiteSpace(skillId))
            {
                return null;
            }

            var skill = skills.FirstOrDefault(s => s.skillId == skillId);
            if (skill != null)
            {
                return skill;
            }

            skill = new SkillProgress
            {
                skillId = skillId,
                level = 1,
                xp = 0f
            };
            skills.Add(skill);
            return skill;
        }

        public bool GainSkillXp(string skillId, float amount)
        {
            if (string.IsNullOrWhiteSpace(skillId) || amount <= 0f)
            {
                return false;
            }

            var skill = EnsureSkill(skillId);
            if (skill == null)
            {
                return false;
            }

            skill.xp += amount;
            while (skill.xp >= RequiredXp(skill.level))
            {
                skill.xp -= RequiredXp(skill.level);
                skill.level += 1;
            }

            return true;
        }

        public static float RequiredXp(int level)
        {
            return 20f + (level * 10f);
        }
    }

    [Serializable]
    public class SkillProgress
    {
        public string skillId = string.Empty;
        public int level = 1;
        public float xp;
    }
}
