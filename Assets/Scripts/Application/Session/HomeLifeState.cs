using System;

namespace Lifehandled.Application.Session
{
    [Serializable]
    public class HomeLifeState
    {
        public bool isHomeOwned = true;
        public int furnitureCount = 1;
        public float homeComfort = 55f;
        public float cleanliness = 65f;
        public int storageCapacityBase = 6;
        public int storageCapacityBonus = 0;
        public float neighborhoodReputation = 50f;

        public int GetStorageCapacity()
        {
            var capacity = storageCapacityBase + storageCapacityBonus;
            return capacity < 1 ? 1 : capacity;
        }
    }
}
