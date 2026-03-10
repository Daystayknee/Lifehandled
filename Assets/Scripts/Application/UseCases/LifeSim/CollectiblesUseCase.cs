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
            message = $"Collected {collectibleId}.";
            return true;
        }
    }
}
