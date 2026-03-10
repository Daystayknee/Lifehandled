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

            var activeZone = context.zones?.Find(z => z.zoneType == context.currentZone);
            if (activeZone != null && activeZone.npcPool.Count > 0 && !activeZone.npcPool.Contains(npc.profile.npcId))
            {
                reaction = $"{npc.profile.displayName} is not in this zone right now.";
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
            var charismaLevel = context.progression?.GetSkillLevel("charisma") ?? 1;
            var negotiationLevel = context.progression?.GetSkillLevel("negotiation") ?? 1;

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

            var opener = BuildSpeechPrefix(npc.profile.voiceType, npc.profile.speechStyle);
            if (!string.IsNullOrWhiteSpace(opener))
            {
                reaction = $"{npc.profile.displayName} ({opener}) {reaction}";
            }

            var friendshipDelta = 1.5f + ((charismaLevel - 1) * 0.4f);
            var trustDelta = 1f + ((negotiationLevel - 1) * 0.35f);
            var sharedHobbyBonus = ResolveSharedHobbyBonus(context, npc.profile.preferences?.hobbyIds);
            friendshipDelta += sharedHobbyBonus;
            if (npc.profile.socialTraits.Contains(Domain.Common.SocialTraitType.Introverted))
            {
                friendshipDelta -= 0.4f;
            }
            if (npc.profile.socialTraits.Contains(Domain.Common.SocialTraitType.Awkward))
            {
                trustDelta -= 0.2f;
            }
            if (npc.profile.emotionalTraits.Contains(Domain.Common.EmotionalTraitType.Forgiving))
            {
                friendshipDelta += 0.35f;
            }

            if (npc.profile.preferences?.favoriteFoodItemIds != null && npc.profile.preferences.favoriteFoodItemIds.Exists(id => context.inventory.GetCount(id) > 0))
            {
                friendshipDelta += 0.4f;
                reaction += " They seem happy you remembered their favorite food.";
            }

            if (npc.profile.preferences != null && npc.profile.preferences.preferredClothingStyleId == npc.profile.clothingStyleId)
            {
                trustDelta += 0.2f;
            }

            rel.friendship = Clamp(rel.friendship + friendshipDelta);
            rel.trust = Clamp(rel.trust + trustDelta);
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

        private static string BuildSpeechPrefix(Domain.Common.VoiceType voice, Domain.Common.SpeechStyleType speech)
        {
            return $"{voice.ToString().ToLowerInvariant()} / {speech.ToString().ToLowerInvariant()}";
        }

        private static float ResolveSharedHobbyBonus(GameSessionContext context, System.Collections.Generic.List<string> hobbies)
        {
            if (hobbies == null || hobbies.Count == 0)
            {
                return 0f;
            }

            float bonus = 0f;
            if (hobbies.Contains("fishing") && context.progression?.GetSkillLevel("fishing") > 1) bonus += 0.3f;
            if (hobbies.Contains("cooking") && context.progression?.GetSkillLevel("cooking") > 1) bonus += 0.3f;
            if (hobbies.Contains("working_out") && context.progression?.GetSkillLevel("fitness") > 1) bonus += 0.25f;
            if (hobbies.Contains("painting") && context.progression?.GetSkillLevel("crafting") > 1) bonus += 0.2f;
            if (hobbies.Contains("reading") && context.progression?.GetSkillLevel("medicine") > 1) bonus += 0.2f;
            return bonus;
        }
    }
}
