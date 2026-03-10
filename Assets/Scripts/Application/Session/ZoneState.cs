using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.Session
{
    [Serializable]
    public class ZoneState
    {
        public ZoneType zoneType;
        public string zoneId = string.Empty;
        public string displayName = string.Empty;
        public List<string> npcPool = new();
        public List<string> resources = new();
        public List<string> events = new();
        public float dangerLevel;
    }
}
