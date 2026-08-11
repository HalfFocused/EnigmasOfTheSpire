using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class Blueprint() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override bool IsPlayable => CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("LastCardPlayedString", "")
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        //InventHoverTip()
    ];
    
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant() + TheManBehindTheSlaughter()}.png".CardImagePath();

    public override string? CustomPortraitPath
    {
        get
        {
            var name = Id.Entry.RemovePrefix().ToLowerInvariant() + TheManBehindTheSlaughter();
            var path = $"res://{MainFile.ModId}/images/card_portraits/big/{name}.png";
            return ResourceLoader.Exists(path) ? path : ArtRoller.Get(Id.Entry, Rarity);
        }
    }
    
    /*
     * how obvious is it that i know nothing about FNAF?
     * anyway, this checks if it's halloween for the FNAF version of the blueprint art instead.
     * it's bullshit like this which is why my mod will never come out
     */
    private String TheManBehindTheSlaughter()
    {
        return DateTime.Today.Month == 10 && DateTime.Today.Day == 31 ? "_scary" : "";
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel? copyOfLastPlayedCard = GetLastPlayedCard()?.CreateClone();
        if (copyOfLastPlayedCard == null) return;
        await EnigmaCmd.ChooseAndTransformInto(choiceContext, Owner, copyOfLastPlayedCard);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        ((StringVar)DynamicVars["LastCardPlayedString"]).StringValue = "\n(" + new LocString("cards", Id.Entry + ".lastCardPlayedLabel").GetFormattedText() + "[gold]" + cardPlay.Card.Title + "[/gold])";
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any())
        {
            ((StringVar)DynamicVars["LastCardPlayedString"]).StringValue = "\n(" + new LocString("cards", Id.Entry + ".lastCardPlayedLabel").GetFormattedText() + "[gold]" + GetLastPlayedCard().Title + "[/gold])";
        }
        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    private static CardModel? GetLastPlayedCard()
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().LastOrDefault().CardPlay.Card;
    }
}