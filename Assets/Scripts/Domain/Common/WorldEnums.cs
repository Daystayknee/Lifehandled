using System;

namespace Lifehandled.Domain.Common
{
    [Serializable]
    public enum SeasonType
    {
        Spring = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 3
    }

    [Serializable]
    public enum WeatherType
    {
        Clear = 0,
        Cloudy = 1,
        Rain = 2,
        Storm = 3
    }

    [Serializable]
    public enum ZoneType
    {
        Home = 0,
        TownCenter = 1,
        Forest = 2,
        GasStation = 3,
        Clinic = 4,
        Apartments = 5,
        Lake = 6,
        Store = 7,
        Town = 8,
        Workplace = 9
    }

    [Serializable]
    public enum AnimalClassType
    {
        Wild = 0,
        Domestic = 1
    }

    [Serializable]
    public enum AnimalGameplayRoleType
    {
        HuntingResource = 0,
        Companionship = 1,
        FarmingProduct = 2,
        EcosystemBehavior = 3
    }
}
