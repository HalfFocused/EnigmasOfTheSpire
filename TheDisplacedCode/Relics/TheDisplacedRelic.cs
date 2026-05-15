using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using TheDisplaced.TheDisplacedCode.Character;
using TheDisplaced.TheDisplacedCode.Extensions;
using Godot;

namespace TheDisplaced.TheDisplacedCode.Relics;

[Pool(typeof(TheDisplacedRelicPool))]
public abstract class TheDisplacedRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}