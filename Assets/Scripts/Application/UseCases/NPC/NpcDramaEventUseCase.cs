using System.Linq;
using Lifehandled.Application.Session;
using Lifehandled.Application.Session.NPC;
using Lifehandled.Domain.Common;
using Lifehandled.Domain.NPC;

namespace Lifehandled.Application.UseCases.NPC
{
    /// <summary>
    /// Lightweight drama engine event entrypoint for secrets/gossip/rivalry/romance/family tension/reputation.
    /// </summary>
    public class NpcDramaEventUseCase
    {
        public bool Execute(GameSessionContext context, string npcId, DramaEventType eventType, out string message)
        {
            message = string.Empty;
            if (context?.npcs == null)
            {
                message = "No active session.";
                return false;
            }

            var npc = context.npcs.FirstOrDefault(n => n.profile.npcId == npcId) ?? context.npcs.FirstOrDefault();
            if (npc == null)
            {
                message = "No NPC available.";
                return false;
            }

            context.socialReputation = Clamp(context.socialReputation);
            context.familyTension = Clamp(context.familyTension);
            npc.profile.drama ??= new NpcDramaState();
            var drama = npc.profile.drama;
            var rel = npc.profile.relationshipToPlayer;

            switch (eventType)
            {
                case DramaEventType.HelpedNpc:
                    rel.friendship = Clamp(rel.friendship + 4f);
                    rel.trust = Clamp(rel.trust + 3f);
                    drama.romanceInterest = Clamp(drama.romanceInterest + 1.5f);
                    drama.socialReputationOfPlayer = Clamp(drama.socialReputationOfPlayer + 3f);
                    context.socialReputation = Clamp(context.socialReputation + 2f);
                    AddMemory(context, npc, "Help", "You helped them today.");
                    message = $"You helped {npc.profile.displayName}. Trust and reputation improved.";
                    break;

                case DramaEventType.InsultedNpc:
                    rel.friendship = Clamp(rel.friendship - 5f);
                    rel.respect = Clamp(rel.respect - 4f);
                    rel.resentment = Clamp(rel.resentment + 5f);
                    drama.rivalryWithPlayer = Clamp(drama.rivalryWithPlayer + 4f);
                    drama.familyTensionWithPlayer = Clamp(drama.familyTensionWithPlayer + 2f);
                    context.familyTension = Clamp(context.familyTension + 2f);
                    context.socialReputation = Clamp(context.socialReputation - 1f);
                    AddMemory(context, npc, "Insult", "You insulted them in public.");
                    SpreadRumor(context, npc, "Insult rumor spread.");
                    message = $"{npc.profile.displayName} took offense. Rivalry increased.";
                    break;

                case DramaEventType.StoleFromShop:
                    rel.trust = Clamp(rel.trust - 7f);
                    rel.fear = Clamp(rel.fear + 4f);
                    rel.resentment = Clamp(rel.resentment + 6f);
                    drama.knownSecretsCount += 1;
                    drama.gossipHeat = Clamp(drama.gossipHeat + 8f);
                    drama.rumorBelief = Clamp(drama.rumorBelief + 10f);
                    context.socialReputation = Clamp(context.socialReputation - 6f);
                    AddMemory(context, npc, "Secret", "They remember you stole from the store.");
                    SpreadRumor(context, npc, "Theft rumor spread.");
                    message = "Your theft became a rumor. Reputation dropped.";
                    break;

                case DramaEventType.Gossiped:
                    drama.gossipHeat = Clamp(drama.gossipHeat + 10f);
                    drama.rumorBelief = Clamp(drama.rumorBelief + 6f);
                    rel.trust = Clamp(rel.trust - 2f);
                    context.socialReputation = Clamp(context.socialReputation - 1f);
                    AddMemory(context, npc, "Gossip", "You gossiped about someone.");
                    SpreadRumor(context, npc, "Gossip spread.");
                    message = "Gossip spreads quickly in town.";
                    break;

                default:
                    message = "No drama event applied.";
                    return false;
            }

            npc.currentReactionHint = message;
            return true;
        }

        private static void AddMemory(GameSessionContext context, NpcRuntimeState npc, string interactionType, string outcome)
        {
            npc.profile.memory.Add(new NpcMemoryEntry
            {
                day = context.currentDay,
                hour = context.hourOfDay,
                interactionType = interactionType,
                outcome = outcome
            });

            if (npc.profile.memory.Count > 30)
            {
                npc.profile.memory.RemoveAt(0);
            }
        }

        private static void SpreadRumor(GameSessionContext context, NpcRuntimeState sourceNpc, string rumorText)
        {
            var target = context.npcs.FirstOrDefault(n => n.profile.npcId != sourceNpc.profile.npcId);
            if (target == null)
            {
                return;
            }

            target.profile.drama ??= new NpcDramaState();
            target.profile.drama.gossipHeat = Clamp(target.profile.drama.gossipHeat + 5f);
            target.profile.drama.rumorBelief = Clamp(target.profile.drama.rumorBelief + 7f);
            target.profile.drama.socialReputationOfPlayer = Clamp(target.profile.drama.socialReputationOfPlayer - 2f);

            target.profile.memory.Add(new NpcMemoryEntry
            {
                day = context.currentDay,
                hour = context.hourOfDay,
                interactionType = "Rumor",
                outcome = rumorText
            });
        }

        private static float Clamp(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
