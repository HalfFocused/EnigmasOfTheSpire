#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class RummagePower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != Owner.Player ? count : count + Amount;
    }
}