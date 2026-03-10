using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.NewGame
{
    /// <summary>
    /// Batch-2-compatible minimal new game setup request.
    /// </summary>
    public class NewGameSetupRequest
    {
        public string mainCharacterName = "Player";
        public bool includeHouseholdMember;
        public string householdMemberName = "Housemate";
        public RelationshipType optionalMemberRelationshipType = RelationshipType.Roommate;
    }
}
