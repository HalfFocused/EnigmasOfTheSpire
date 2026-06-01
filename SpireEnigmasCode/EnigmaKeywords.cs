using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpireEnigmas.SpireEnigmasCode;

public class EnigmaKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Cherished; 
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Dire; 
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Improvise; 
}