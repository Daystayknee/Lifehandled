using System;
using System.Collections.Generic;
using Lifehandled.Domain.Common;

namespace Lifehandled.Domain.Household
{
    /// <summary>
    /// Minimal household container for V1 (membership + basic shared settings).
    /// </summary>
    [Serializable]
    public class HouseholdData
    {
        public string householdId = string.Empty;
        public string householdName = "New Household";
        public string homeLocationId = "starter_home";

        public HouseholdType householdType = HouseholdType.Solo;
        public List<string> memberCharacterIds = new();

        public int sharedFunds;
        public bool usesSharedFunds;
        public bool usesSharedStorage;
        public bool autoPayRentFromSharedFunds = true;
    }
}
