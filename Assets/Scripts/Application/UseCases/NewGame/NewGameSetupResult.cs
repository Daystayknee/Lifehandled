using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.UseCases.NewGame
{
    public class NewGameSetupResult
    {
        public SaveGameEnvelope Envelope { get; set; } = new();
        public string PlayerCharacterId { get; set; } = string.Empty;
        public string HouseholdId { get; set; } = string.Empty;
        public bool HasOptionalHouseholdMember { get; set; }
    }
}
