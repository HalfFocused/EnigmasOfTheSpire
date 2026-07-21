using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmaKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword TimeLoop; 
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Dire; 
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Improvise; 
}