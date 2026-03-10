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
        Town = 1,
        Store = 2,
        Forest = 3,
        Lake = 4,
        Clinic = 5,
        Workplace = 6
    }
}
