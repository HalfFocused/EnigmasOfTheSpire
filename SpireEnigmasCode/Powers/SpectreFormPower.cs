using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Patches;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class SpectreFormPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override int DisplayAmount
    {
        get => Math.Max(0, Amount - GetInternalData<Data>().cardsPlayed);
    }
    
    protected override object InitInternalData() => new Data();
    
    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        if (card.Owner.Creature != Owner || !ShouldAffect(card) || card.IsDupe || GetInternalData<Data>().cardsPlayed >= Amount)
            return location;
        location.pileType = PileType.Hand;
        location.position = CardPilePosition.Top;
        location.player = Owner.Player;
        return location;
    }

    public override Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation location)
    {
        Flash();
        SetCardsPlayed(GetInternalData<Data>().cardsPlayed + 1);
        GetInternalData<Data>().cardsReturnedThisTurn.Add(card);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        SetCardsPlayed(0);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (GetInternalData<Data>().cardsReturnedThisTurn.Contains(card))
        {
            EnigmaCmd.GiveRetainUntilPlayed(card);
            GetInternalData<Data>().cardsReturnedThisTurn.Remove(card);
        }

        return Task.CompletedTask;
    }
    
    private void SetCardsPlayed(int value)
    {
        GetInternalData<Data>().cardsPlayed = value;
        InvokeDisplayAmountChanged();
    }

    /*
     * A list of cards that were returned each turn.
     * Used to add Retain Until Played after the card resolves
     * (because the aftermodifying hook for card location happens before either card play hook)
     */
    public class Data
    {
        public int cardsPlayed;
        public List<CardModel> cardsReturnedThisTurn = new ();
    }

    private bool ShouldAffect(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }
}