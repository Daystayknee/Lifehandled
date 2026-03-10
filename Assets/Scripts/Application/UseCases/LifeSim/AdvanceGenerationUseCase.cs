using System;
using System.Linq;
using Lifehandled.Application.Content;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;

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
            var parentA = context.playerCharacter.data;
            var parentB = context.householdMembers.Select(m => m.data).FirstOrDefault() ?? parentA;

            var generationIndex = context.familyLineage.generationIndex + 1;
            var heirId = Guid.NewGuid().ToString("N");
            var seed = heirId.GetHashCode();

            var inheritedGenetics = PrototypeWorldContentCatalog.BlendGeneticsForOffspring(
                parentA.genetics,
                parentB.genetics,
                parentA.characterId,
                parentB.characterId,
                generationIndex,
                seed);

            var inheritedAppearance = PrototypeWorldContentCatalog.CreateInheritedAppearance(
                parentA.appearance,
                parentB.appearance,
                seed);
            PrototypeWorldContentCatalog.ApplyGeneticInfluenceToAppearance(inheritedAppearance, inheritedGenetics);

            var heir = new CharacterData
            {
                characterId = heirId,
                displayName = $"{parentA.displayName} Jr.",
                role = CharacterRole.PlayerMain,
                isPlayerControlled = true,
                householdId = parentA.householdId,
                ageYears = 18,
                genetics = inheritedGenetics,
                appearance = inheritedAppearance
            };

            parentA.isPlayerControlled = false;
            context.playerCharacter.data = heir;
            context.playerCharacterId = heir.characterId;

            context.familyLineage.generationIndex = generationIndex;
            context.familyLineage.generationCharacterIds.Add(heir.characterId);
            context.familyLineage.legacyReputation = NeedsStatus.ClampToRange(
                (context.familyLineage.legacyReputation + context.socialReputation) * 0.5f);

            message = $"Generation advanced to {context.familyLineage.generationIndex}.";
            return true;
        }
    }
}
