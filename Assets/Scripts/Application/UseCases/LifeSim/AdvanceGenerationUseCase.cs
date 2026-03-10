using System;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class AdvanceGenerationUseCase
    {
        public bool Execute(GameSessionContext context, out string message)
        {
            message = string.Empty;
            if (context?.playerCharacter?.data == null)
            {
                message = "No active session.";
                return false;
            }

            context.familyLineage ??= new FamilyLineageState();
            var oldPlayer = context.playerCharacter.data;

            var heir = new CharacterData
            {
                characterId = Guid.NewGuid().ToString("N"),
                displayName = $"{oldPlayer.displayName} Jr.",
                role = oldPlayer.role,
                isPlayerControlled = true,
                householdId = oldPlayer.householdId,
                ageYears = 18,
                genetics = oldPlayer.genetics,
                appearance = oldPlayer.appearance
            };

            oldPlayer.isPlayerControlled = false;
            context.playerCharacter.data = heir;
            context.playerCharacterId = heir.characterId;

            context.familyLineage.generationIndex += 1;
            context.familyLineage.generationCharacterIds.Add(heir.characterId);
            context.familyLineage.legacyReputation = NeedsStatus.ClampToRange(
                (context.familyLineage.legacyReputation + context.socialReputation) * 0.5f);

            message = $"Generation advanced to {context.familyLineage.generationIndex}.";
            return true;
        }
    }
}
