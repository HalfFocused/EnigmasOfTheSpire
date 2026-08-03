using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Character.sacrifice;
using SpireEnigmas.SpireEnigmasCode.Character.savant;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards;

public abstract class SpireEnigmasCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{

    public Creature? GetChirp => ChirpCmd.GetChirpFromPlayer(Owner);
    
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
        return GetStaticHoverTip("SPIREENIGMAS-FORGET");
    }
    
    public static HoverTip FlashbackHoverTip()
    {
        return GetStaticHoverTip("SPIREENIGMAS-FLASHBACK");
    }
    
    public static HoverTip ChirpHoverTip()
    {
        return GetStaticHoverTip("SPIREENIGMAS-CHIRP");
    }
}