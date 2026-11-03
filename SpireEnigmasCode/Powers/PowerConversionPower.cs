#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class PowerConversionPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter  ;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(EnigmaHoverTips.Chirp),
        HoverTipFactory.ForEnergy(this)
    ];

    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        return player != Owner.Player ? amount : amount - Amount;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
            return;
        await ChirpCmd.GainEnergy(new ThrowingPlayerChoiceContext(), Owner.Player, Amount, this);
    }
}