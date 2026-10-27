using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class InkPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Decimal ModifyBlockMultiplicative(
        Creature target,
        Decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource is null || cardSource.Owner.Creature != Owner || !ShouldActivate(cardSource) ? 1M : 1.5M;
    }
    
    public override async Task AfterModifyingBlockAmount(
        Decimal modifiedAmount,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (modifiedAmount <= 0M || cardSource == null)
            return;
        Flash();
        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, -1, Owner, null);
    }

    private bool ShouldActivate(CardModel card)
    {
        return card.GainsBlock;
    }
}