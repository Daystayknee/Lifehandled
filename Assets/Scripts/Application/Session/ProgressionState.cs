using System;
using System.Collections.Generic;

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
    }

    [Serializable]
    public class SkillProgress
    {
        public string skillId = string.Empty;
        public int level = 1;
        public float xp;
    }
}
