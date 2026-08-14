using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public sealed class ChirpDoubleBlockPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ChirpHoverTip(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public override Decimal ModifyBlockMultiplicative(
        Creature target,
        Decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return target != Owner || !props.IsCardOrMonsterMove() ? 1M : 2M;
    }
    
    public override async Task AfterModifyingBlockAmount(
        Decimal modifiedAmount,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (modifiedAmount <= 0M)
            return;
        Flash();

        await PowerCmd.Decrement(this);
    }
}