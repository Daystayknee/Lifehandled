using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.LifeSim
{
    public class PropertyUpgradeUseCase
    {
        public const int UpgradeCost = 30;

        public bool Execute(GameSessionContext context, out string message)
        {
            message = string.Empty;
            if (context == null || context.home == null)
            {
                message = "No active session.";
                return false;
            }

            if (context.wallet < UpgradeCost)
            {
                message = "Not enough money for upgrade.";
                return false;
            }

            context.wallet -= UpgradeCost;
            context.home.homeComfort = NeedsStatus.ClampToRange(context.home.homeComfort + 10f);
            context.home.storageCapacityBonus += 2;
            context.home.neighborhoodReputation = NeedsStatus.ClampToRange(context.home.neighborhoodReputation + 3f);
            message = "Property upgraded: comfort and storage increased.";
            return true;
        }
    }
}
