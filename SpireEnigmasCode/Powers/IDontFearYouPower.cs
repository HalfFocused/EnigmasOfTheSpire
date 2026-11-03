#region

using MegaCrit.Sts2.Core.Entities.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class IDontFearYouPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
}