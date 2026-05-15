using BaseLib.Abstracts;
using BaseLib.Extensions;
using TheDisplaced.TheDisplacedCode.Extensions;
using Godot;

namespace TheDisplaced.TheDisplacedCode.Powers;

public abstract class TheDisplacedPower : CustomPowerModel
{
    //Loads from TheDisplaced/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}