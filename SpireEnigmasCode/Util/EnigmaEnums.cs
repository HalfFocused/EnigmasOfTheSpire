using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmaKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Command; 
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Refine; 
}

public class EnigmaTargetTypes
{
    [CustomEnum]
    public static TargetType ChirpOrAnyPlayer; 
}