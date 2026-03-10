using System;

namespace Lifehandled.Domain.Common
{
    /// <summary>
    /// High-level role classification for character records.
    /// </summary>
    [Serializable]
    public enum CharacterRole
    {
        PlayerMain = 0,
        HouseholdMember = 1,
        Npc = 2
    }

    [Serializable]
    public enum RelationshipType
    {
        Family = 0,
        Friend = 1,
        Roommate = 2,
        Romantic = 3,
        Rival = 4
    }

    [Serializable]
    public enum HouseholdType
    {
        Solo = 0,
        Roommates = 1,
        Family = 2,
        Mixed = 3
    }

    [Serializable]
    public enum GeneSourceType
    {
        Randomized = 0,
        Template = 1,
        Inherited = 2
    }

    [Serializable]
    public enum FaceShapeType { Oval = 0, Round = 1, Square = 2, Heart = 3, Diamond = 4, Triangular = 5, LongRectangular = 6 }

    [Serializable]
    public enum JawlineType { Soft = 0, Sharp = 1, Square = 2, Narrow = 3, Wide = 4 }

    [Serializable]
    public enum ChinType { Pointed = 0, Rounded = 1, Cleft = 2, Wide = 3, Small = 4 }

    [Serializable]
    public enum EyeShapeType { Almond = 0, Round = 1, Hooded = 2, Monolid = 3, Upturned = 4, Downturned = 5, WideSet = 6, CloseSet = 7 }

    [Serializable]
    public enum EyeColorType { Brown = 0, Hazel = 1, Blue = 2, Gray = 3, Green = 4, Amber = 5, Mixed = 6 }

    [Serializable]
    public enum NoseShapeType { Straight = 0, Aquiline = 1, Button = 2, Bulbous = 3, Hooked = 4, WideBridge = 5, NarrowBridge = 6 }

    [Serializable]
    public enum LipShapeType { Thin = 0, Full = 1, HeartShaped = 2, Wide = 3, Small = 4, DefinedCupidsBow = 5 }

    [Serializable]
    public enum HairType { H1A = 0, H1B = 1, H1C = 2, H2A = 3, H2B = 4, H2C = 5, H3A = 6, H3B = 7, H3C = 8, H4A = 9, H4B = 10, H4C = 11 }

    [Serializable]
    public enum HairLengthType { Shaved = 0, Buzz = 1, Short = 2, Medium = 3, Long = 4, VeryLong = 5 }

    [Serializable]
    public enum HairStyleType { Ponytail = 0, Braids = 1, Bun = 2, Loose = 3, Fade = 4, Undercut = 5 }

    [Serializable]
    public enum HairColorType { Black = 0, Brown = 1, Blonde = 2, Red = 3, Gray = 4, Dyed = 5 }

    [Serializable]
    public enum ClothingCategoryType { UpperWear = 0, LowerWear = 1, Footwear = 2, Accessory = 3 }

    [Serializable]
    public enum FoodCategoryType { BasicSurvival = 0, Prepared = 1, Luxury = 2 }
}
