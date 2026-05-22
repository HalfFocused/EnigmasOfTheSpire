using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace TheDisplaced.TheDisplacedCode.Powers;

public class TheSecondDreamPower : TheDisplacedPower
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