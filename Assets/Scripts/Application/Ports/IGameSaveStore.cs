using Lifehandled.Infrastructure.Persistence.DTO;

namespace Lifehandled.Application.Ports
{
    /// <summary>
    /// Generic game save persistence contract for save/load readiness.
    /// </summary>
    public interface IGameSaveStore
    {
        void Save(SaveGameEnvelope envelope);
        SaveGameEnvelope Load();
        bool HasSave();
    }
}
