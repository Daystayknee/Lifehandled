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
            npc.profile.drama ??= new NpcDramaState();
            var drama = npc.profile.drama;

            var remembersTheft = npc.profile.memory.Exists(m => m.outcome.Contains("stole from the store"));
            var remembersHelp = npc.profile.memory.Exists(m => m.outcome.Contains("helped"));
            var remembersInsult = npc.profile.memory.Exists(m => m.outcome.Contains("insulted"));

            if (remembersTheft || drama.rivalryWithPlayer > 60f || rel.fear > 50f || rel.resentment > 60f)
            {
                reaction = $"{npc.profile.displayName} avoids you.";
            }
            else if (drama.romanceInterest > 65f && rel.attraction > 30f && mood > 55f && time >= 18f)
            {
                reaction = $"{npc.profile.displayName}: Want to spend some time together tonight?";
            }
            else if (rel.friendship > 60f && rel.trust > 50f && mood > 55f)
            {
                reaction = time < 12f
                    ? $"{npc.profile.displayName}: Morning! Good to see you."
                    : $"{npc.profile.displayName}: Nice to catch up.";
            }
            else if (drama.familyTensionWithPlayer > 50f || context.familyTension > 60f)
            {
                reaction = $"{npc.profile.displayName}: Family drama again?";
            }
            else if (remembersInsult)
            {
                reaction = $"{npc.profile.displayName}: I'm still not over what you said.";
            }
            else if (remembersHelp)
            {
                reaction = $"{npc.profile.displayName}: I remember your help.";
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
            drama.gossipHeat = Clamp(drama.gossipHeat - 0.5f);

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
