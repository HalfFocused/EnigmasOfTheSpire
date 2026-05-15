using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using TheDisplaced.TheDisplacedCode.Character;
using TheDisplaced.TheDisplacedCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace TheDisplaced.TheDisplacedCode.Cards;

[Pool(typeof(TheDisplacedCardPool))]
public abstract class TheDisplacedCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    public static HoverTip GetStaticHoverTip(string locEntry)
    {
        const string locTable = "static_hover_tips";
        return new HoverTip(
            new LocString(locTable, locEntry + ".title"),
            new LocString(locTable, locEntry + ".description")
        );
    }

    public static HoverTip ForgetHoverTip()
    {
        return GetStaticHoverTip("THEDISPLACED-FORGET");
    }
    
    public static HoverTip FlashbackHoverTip()
    {
        return GetStaticHoverTip("THEDISPLACED-FLASHBACK");
    }
}