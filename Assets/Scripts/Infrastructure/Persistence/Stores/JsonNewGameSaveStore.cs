using System.IO;
using Lifehandled.Application.Ports;
using Lifehandled.Infrastructure.Persistence.DTO;
using UnityEngine;

namespace Lifehandled.Infrastructure.Persistence.Stores
{
    /// <summary>
    /// Minimal file-backed save store used by New Game and VS01 startup readiness.
    /// </summary>
    public class JsonNewGameSaveStore : INewGameSaveStore, IGameSaveStore
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

        public SaveGameEnvelope Load()
        {
            if (!HasSave())
            {
                return new SaveGameEnvelope();
            }

            var json = File.ReadAllText(_savePath);
            var envelope = JsonUtility.FromJson<SaveGameEnvelope>(json);
            return envelope ?? new SaveGameEnvelope();
        }

        public bool HasSave()
        {
            return File.Exists(_savePath);
        }

        public string GetSavePath()
        {
            return _savePath;
        }
    }
}
