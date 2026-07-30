using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class PlanBPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource is null || (dealer != Owner && cardSource.Owner != Owner.Player) || !props.IsPoweredAttack() || !CardCreatedThisCombat(cardSource) ? 0M : Amount;
    }
    
    public override Decimal ModifyBlockAdditive(
        Creature target,
        Decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource is null || cardSource.Owner.Creature != Owner || !CardCreatedThisCombat(cardSource) ? 0M : Amount;
    }

    private bool CardCreatedThisCombat(CardModel card)
    {
        return CombatManager.Instance.History.Entries.OfType<CardGeneratedEntry>().Any(e => e.Card == card);
    }
}