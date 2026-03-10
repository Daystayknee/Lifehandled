using System;
using Lifehandled.Domain.NPC;

namespace Lifehandled.Application.Session.NPC
{
    [Serializable]
    public class NpcRuntimeState
    {
        public NpcProfile profile = new();
        public string currentReactionHint = string.Empty;
    }
}
