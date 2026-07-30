using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
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

public sealed class ChirpBlockNextTurnPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ChirpHoverTip(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature != Owner.PetOwner.Creature)
            return;
        await ChirpCmd.GiveBlockToOwner(Owner.PetOwner, Amount, ValueProp.Unpowered, null);
        await PowerCmd.Remove(this);
    }
}