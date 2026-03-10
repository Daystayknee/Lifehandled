using System;
using Lifehandled.Domain.Character;

namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Lightweight runtime projection of character data for session start.
    /// </summary>
    [Serializable]
    public class RuntimeCharacterState
    {
        public CharacterData data = new();
        public GeneticModifierProfile geneticModifiers = new();
        public bool isLightSimulatedHouseholdMember;
    }
}
