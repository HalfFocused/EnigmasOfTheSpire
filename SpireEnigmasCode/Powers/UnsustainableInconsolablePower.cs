using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class UnsustainableInconsolablePower : SpireEnigmaPower, IMaxHandSizeModifier
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int ModifyMaxHandSizeLate(Player player, int currentMaxHandSize)
    {
        return currentMaxHandSize + 1;
    }
}