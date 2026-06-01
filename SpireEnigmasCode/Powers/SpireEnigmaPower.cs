using BaseLib.Abstracts;
using BaseLib.Extensions;
using SpireEnigmas.SpireEnigmasCode.Extensions;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public abstract class SpireEnigmaPower : CustomPowerModel
{
    //Loads from TheDisplaced/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}