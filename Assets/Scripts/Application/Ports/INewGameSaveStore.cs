using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.Ports
{
    /// <summary>
    /// Minimal persistence port for writing newly created game state.
    /// </summary>
    public interface INewGameSaveStore
    {
        void Save(SaveGameEnvelope envelope);
    }
}
