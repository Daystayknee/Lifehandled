using System;
using System.Collections.Generic;

namespace Lifehandled.Application.Session
{
    [Serializable]
    public class FamilyLineageState
    {
        public int generationIndex;
        public string legacyFamilyName = "Founders";
        public List<string> generationCharacterIds = new();
        public float legacyReputation = 50f;
    }
}
