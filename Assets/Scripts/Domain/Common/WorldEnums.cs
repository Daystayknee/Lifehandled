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
}
