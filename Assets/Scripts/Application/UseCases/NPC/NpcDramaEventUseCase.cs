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

            var activeZone = context.zones?.FirstOrDefault(z => z.zoneType == context.currentZone);
            if (activeZone != null && activeZone.npcPool.Count > 0 && !activeZone.npcPool.Contains(npc.profile.npcId))
            {
                message = $"{npc.profile.displayName} is not in this zone right now.";
                return false;
            }

            context.socialReputation = Clamp(context.socialReputation);
            context.familyTension = Clamp(context.familyTension);
            npc.profile.drama ??= new NpcDramaState();
            var drama = npc.profile.drama;
            var rel = npc.profile.relationshipToPlayer;
            var negotiationLevel = context.progression?.GetSkillLevel("negotiation") ?? 1;
            var charismaLevel = context.progression?.GetSkillLevel("charisma") ?? 1;

            var rumorBaseImpact = 0f;
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
                    rumorBaseImpact = -1f;
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
                    rumorBaseImpact = 3f;
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
                    rumorBaseImpact = 6f;
                    message = "Your theft became a rumor. Reputation dropped.";
                    break;

                case DramaEventType.Gossiped:
                    drama.gossipHeat = Clamp(drama.gossipHeat + 10f);
                    drama.rumorBelief = Clamp(drama.rumorBelief + 6f);
                    rel.trust = Clamp(rel.trust - 2f);
                    context.socialReputation = Clamp(context.socialReputation - 1f);
                    AddMemory(context, npc, "Gossip", "You gossiped about someone.");
                    rumorBaseImpact = 4f;
                    message = "Gossip spreads quickly in town.";
                    break;

                default:
                    message = "No drama event applied.";
                    return false;
            }

            if (npc.profile.emotionalTraits.Contains(EmotionalTraitType.Jealous))
            {
                drama.rivalryWithPlayer = Clamp(drama.rivalryWithPlayer + 1f);
            }

            if (npc.profile.emotionalTraits.Contains(EmotionalTraitType.Forgiving))
            {
                rel.resentment = Clamp(rel.resentment - 0.8f);
            }

            if (npc.profile.socialTraits.Contains(SocialTraitType.Manipulative))
            {
                drama.gossipHeat = Clamp(drama.gossipHeat + 1f);
            }

            var deescalation = ((negotiationLevel - 1) * 0.25f) + ((charismaLevel - 1) * 0.2f);
            drama.familyTensionWithPlayer = Clamp(drama.familyTensionWithPlayer - deescalation);
            context.familyTension = Clamp(context.familyTension - (deescalation * 0.5f));

            var rumorSpreadCount = SpreadRumor(context, npc, rumorBaseImpact, $"{eventType} rumor spread.");
            if (rumorSpreadCount > 0)
            {
                message += $" Rumor reached {rumorSpreadCount} NPCs.";
            }

            RecomputeGlobalReputationFromNpcBeliefs(context);
            npc.currentReactionHint = message;
            context.lastNpcReaction = message;
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

        private static int SpreadRumor(GameSessionContext context, NpcRuntimeState sourceNpc, float reputationImpact, string rumorText)
        {
            var targets = context.npcs
                .Where(n => n.profile.npcId != sourceNpc.profile.npcId)
                .Take(4)
                .ToList();

            if (targets.Count == 0)
            {
                return 0;
            }

            var gossipHeatStep = 5f;
            var rumorBeliefStep = 7f;
            var spreadFactor = 1f;
            var spreadCount = 0;

            foreach (var target in targets)
            {
                target.profile.drama ??= new NpcDramaState();
                target.profile.drama.gossipHeat = Clamp(target.profile.drama.gossipHeat + (gossipHeatStep * spreadFactor));
                target.profile.drama.rumorBelief = Clamp(target.profile.drama.rumorBelief + (rumorBeliefStep * spreadFactor));
                target.profile.drama.socialReputationOfPlayer = Clamp(target.profile.drama.socialReputationOfPlayer - (reputationImpact * spreadFactor));

                target.profile.memory.Add(new NpcMemoryEntry
                {
                    day = context.currentDay,
                    hour = context.hourOfDay,
                    interactionType = "Rumor",
                    outcome = rumorText
                });
                if (target.profile.memory.Count > 30)
                {
                    target.profile.memory.RemoveAt(0);
                }

                spreadCount++;
                spreadFactor *= 0.72f;
            }

            return spreadCount;
        }

        private static void RecomputeGlobalReputationFromNpcBeliefs(GameSessionContext context)
        {
            if (context.npcs == null || context.npcs.Count == 0)
            {
                return;
            }

            var avgNpcRep = context.npcs.Average(n => n.profile.drama?.socialReputationOfPlayer ?? 50f);
            context.socialReputation = Clamp((context.socialReputation * 0.65f) + (avgNpcRep * 0.35f));
        }

        private static float Clamp(float value)
        {
            if (value < 0f) return 0f;
            if (value > 100f) return 100f;
            return value;
        }
    }
}
