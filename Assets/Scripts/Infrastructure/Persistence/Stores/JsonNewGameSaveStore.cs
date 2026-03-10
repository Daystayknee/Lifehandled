using System.IO;
using Lifehandled.Application.Ports;
using Lifehandled.Infrastructure.Persistence.DTO;
using UnityEngine;

namespace Lifehandled.Infrastructure.Persistence.Stores
{
    /// <summary>
    /// Minimal file-backed save writer for new game creation flow.
    /// </summary>
    public class JsonNewGameSaveStore : INewGameSaveStore
    {
        private readonly string _savePath;

        public JsonNewGameSaveStore(string fileName = "savegame.json")
        {
            _savePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public void Save(SaveGameEnvelope envelope)
        {
            envelope ??= new SaveGameEnvelope();
            var json = JsonUtility.ToJson(envelope, prettyPrint: true);
            File.WriteAllText(_savePath, json);
        }

        public string GetSavePath()
        {
            return _savePath;
        }
    }
}
