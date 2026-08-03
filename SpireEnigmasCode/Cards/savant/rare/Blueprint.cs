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
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel? copyOfLastPlayedCard = GetLastPlayedCard()?.CreateClone();
        if (copyOfLastPlayedCard == null) return;
        await EnigmaCmd.Invent(choiceContext, Owner, copyOfLastPlayedCard);
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