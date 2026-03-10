using Lifehandled.Application.Session;
using Lifehandled.Domain.NPC;
using Lifehandled.Domain.Social;

namespace Lifehandled.Application.UseCases.NPC
{
    public class NpcSocialInteractionUseCase
    {
        public bool TalkToNpc(GameSessionContext context, string npcId, out string reaction)
        {
            reaction = "";
            if (context?.npcs == null)
            {
                reaction = "No active session.";
                return false;
            }

            var npc = context.npcs.Find(n => n.profile.npcId == npcId) ?? context.npcs.Find(_ => true);
            if (npc == null)
            {
                reaction = "No NPC available.";
                return false;
            }

            if (!npc.profile.schedule.isAvailableForTalk)
            {
                reaction = $"{npc.profile.displayName} is busy right now.";
                return false;
            }

            var rel = npc.profile.relationshipToPlayer;
            var mood = npc.profile.mood;
            var time = context.hourOfDay;

            if (rel.fear > 50f || rel.resentment > 60f)
            {
                reaction = $"{npc.profile.displayName} avoids you.";
            }
            else if (rel.friendship > 60f && rel.trust > 50f && mood > 55f)
            {
                reaction = time < 12f
                    ? $"{npc.profile.displayName}: Morning! Good to see you."
                    : $"{npc.profile.displayName}: Nice to catch up.";
            }
            else if (mood < 35f)
            {
                reaction = $"{npc.profile.displayName}: Not a great time.";
            }
            else
            {
                reaction = $"{npc.profile.displayName}: Hey.";
            }

            rel.friendship = Clamp(rel.friendship + 2f);
            rel.trust = Clamp(rel.trust + 1f);
            npc.profile.mood = Clamp(npc.profile.mood + 1f);

            npc.profile.memory.Add(new NpcMemoryEntry
            {
                day = context.currentDay,
                hour = context.hourOfDay,
                interactionType = "Talk",
                outcome = reaction
            });
            if (npc.profile.memory.Count > 20)
            {
                npc.profile.memory.RemoveAt(0);
            }

            context.talkCountToday += 1;
            npc.currentReactionHint = reaction;
            return true;
        }

        private static float Clamp(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
