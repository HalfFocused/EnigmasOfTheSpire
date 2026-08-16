using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmaEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword TimeLoop; 
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Command; 
    
    [CustomEnum]
    public static TargetType ChirpOrAnyPlayer; 
}