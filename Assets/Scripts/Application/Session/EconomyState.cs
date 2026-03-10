using System;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.Session
{
    [Serializable]
    public class EconomyState
    {
        public JobType currentJob = JobType.PartTimeShopHelper;
        public int dailyIncome = 12;

        public HousingTier housingTier = HousingTier.Basic;
        public int weeklyRentCost = 18;

        public float scarcityMultiplier = 1f;
        public float regionWealthMultiplier = 1f;
        public float reputationMultiplier = 1f;
        public float supplyDemandMultiplier = 1f;

        public int lastIncomePaidDay;
        public int lastRentPaidDay;
    }
}
