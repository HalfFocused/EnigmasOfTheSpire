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

public class FreestylePower : SpireEnigmaPower
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
        return Owner != dealer || !props.IsPoweredAttack() || cardSource is null || !CardDrawnDuringThisTurn(cardSource) ? 0M : Amount;
    }

    public static bool CardDrawnDuringThisTurn(CardModel card)
    {
        return CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>()
            .Any(e => e.Card == card && !e.FromHandDraw && e.RoundNumber == card.CombatState.RoundNumber);
    }
}