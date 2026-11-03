#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireEnigmas.SpireEnigmasCode.Character.chronicle;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Character.renegade;
using SpireEnigmas.SpireEnigmasCode.Character.sacrifice;
using SpireEnigmas.SpireEnigmasCode.Character.savant;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards;

public abstract class SpireEnigmasCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    protected Creature? GetChirp => EnigmasHelper.GetChirpFromPlayer(Owner);
    
    protected Creature? GetAdversary => EnigmasHelper.GetAdversaryFromPlayer(Owner);

    
    /*
    private LocString? _flavorTextTitleLocString;
    
    public LocString FlavorTextTitleLocString
    {
        get
        {
            return _flavorTextTitleLocString ??= new LocString("cards", Id.Entry + ".flavorTitle");
        }
    }
    
    private LocString? _flavorTextBodyLocString;
    
    public LocString FlavorTextBodyLocString
    {
        get
        {
            return _flavorTextBodyLocString ??= new LocString("cards", Id.Entry + ".flavorBody");
        }
    }
    */
    
    public override string? CustomPortraitPath
    {
        get
        {
            var name = Id.Entry.RemovePrefix().ToLowerInvariant();
            var path = $"res://{MainFile.ModId}/images/card_portraits/big/{name}.png";
            return ResourceLoader.Exists(path) ? path : ArtRoller.Get(Id.Entry, Rarity);
        }
    }

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    
    [Pool(typeof(TheDisplacedCardPool))]
    public abstract class DisplacedCard(int cost, CardType type, CardRarity rarity, TargetType target) :
        SpireEnigmasCard(cost, type, rarity, target);
    
    [Pool(typeof(TheSacrificeCardPool))]
    public abstract class SacrificeCard(int cost, CardType type, CardRarity rarity, TargetType target) :
        SpireEnigmasCard(cost, type, rarity, target);
    
    [Pool(typeof(TheSavantCardPool))]
    public abstract class SavantCard(int cost, CardType type, CardRarity rarity, TargetType target) :
        SpireEnigmasCard(cost, type, rarity, target);
    
    [Pool(typeof(TheChronicleCardPool))]
    public abstract class ChronicleCard(int cost, CardType type, CardRarity rarity, TargetType target) :
        SpireEnigmasCard(cost, type, rarity, target);
    
    [Pool(typeof(TheRenegadeCardPool))]
    public abstract class RenegadeCard(int cost, CardType type, CardRarity rarity, TargetType target) :
        SpireEnigmasCard(cost, type, rarity, target);
    
    public static HoverTip GetStaticHoverTip(string locEntry)
    {
        const string locTable = "static_hover_tips";
        return new HoverTip(
            new LocString(locTable, locEntry + ".title"),
            new LocString(locTable, locEntry + ".description")
        );
    }

    protected bool IsInChapter(int chapter, StoryManager.CardInChapterTimings timing)
    {
        return StoryManager.InChapter(this, chapter, timing);
    }
    
    protected bool IsRefined => RefinementFields.IsRefined.Get(this);
}