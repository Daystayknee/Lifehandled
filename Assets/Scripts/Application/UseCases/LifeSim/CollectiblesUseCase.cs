using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class CollectiblesUseCase
    {
        public bool Collect(GameSessionContext context, string collectibleId, out string message)
        {
            message = string.Empty;
            if (context == null || string.IsNullOrWhiteSpace(collectibleId))
            {
                message = "Invalid collectible.";
                return false;
            }

            context.collectibles ??= new System.Collections.Generic.List<string>();
            if (context.collectibles.Contains(collectibleId))
            {
                message = "Already collected.";
                return false;
            }

            context.collectibles.Add(collectibleId);

            context.progression ??= new ProgressionState();
            context.progression.GainSkillXp("survival", 2f);

            if (context.collectibles.Count % 5 == 0)
            {
                var milestonePerk = $"collector_tier_{context.collectibles.Count / 5}";
                if (!context.progression.unlockedPerkIds.Contains(milestonePerk))
                {
                    context.progression.unlockedPerkIds.Add(milestonePerk);
                }

                context.socialReputation = NeedsStatus.ClampToRange(context.socialReputation + 1.5f);
                message = $"Collected {collectibleId}. Milestone reached: {milestonePerk}.";
                return true;
            }

            message = $"Collected {collectibleId}.";
            return true;
        }
    }
}
