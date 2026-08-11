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

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class BuiltToLastPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override int DisplayAmount
    {
        get => Math.Max(0, Amount - GetInternalData<Data>().gadgetsPlayed);
    }
    
    protected override object InitInternalData() => (object) new Data();
    
    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        if (card.Owner.Creature != Owner || card is not Gadget || card.IsDupe || GetInternalData<Data>().gadgetsPlayed >= this.Amount)
            return location;
        location.pileType = PileType.Hand;
        location.position = CardPilePosition.Top;
        location.player = Owner.Player;
        return location;
    }

    public override Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation location)
    {
        Flash();
        SetGadgetsPlayed(GetInternalData<Data>().gadgetsPlayed + 1);
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
        SetGadgetsPlayed(0);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    
    public void SetGadgetsPlayed(int value)
    {
        GetInternalData<Data>().gadgetsPlayed = value;
        InvokeDisplayAmountChanged();
    }

    public class Data
    {
        public int gadgetsPlayed;
    }
}