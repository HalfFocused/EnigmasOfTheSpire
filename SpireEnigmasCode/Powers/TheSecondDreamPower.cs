using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class TheSecondDreamPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override bool ShouldTakeExtraTurn(Player player)
    {
        return true;
    }
    
    public override async Task AfterTakingExtraTurn(Player player)
    {
        if (player != Owner.Player)
            return;
        Flash();
        await PowerCmd.TickDownDuration(this);
    }
}