using Lifehandled.Domain.Character;

namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Convenience accessor for systems that only need the selected player character.
    /// </summary>
    public static class SelectedPlayerCharacterAccessor
    {
        public static CharacterData GetPlayerCharacter()
        {
            return SessionContextRegistry.Current?.playerCharacter?.data;
        }
    }
}
