#region

using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

#endregion

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

public class EnigmaHoverTips
{
    [CustomEnum]
    public static StaticHoverTip Forget; 
    
    [CustomEnum]
    public static StaticHoverTip Flashback; 
    
    [CustomEnum]
    public static StaticHoverTip Chirp;
    
    [CustomEnum]
    public static StaticHoverTip Gadget;
    
    [CustomEnum]
    public static StaticHoverTip Invent;
    
    [CustomEnum]
    public static StaticHoverTip Story;
    
    [CustomEnum]
    public static StaticHoverTip Unwell;
}