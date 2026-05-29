using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.oblation;
using SpireEnigmas.SpireEnigmasCode.Extensions;

namespace SpireEnigmas.SpireEnigmasCode.Relics;

[Pool(typeof(TheOblationRelicPool))]
public abstract class TheOblationRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}