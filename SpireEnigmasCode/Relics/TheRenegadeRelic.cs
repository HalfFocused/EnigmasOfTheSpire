#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpireEnigmas.SpireEnigmasCode.Character.renegade;
using SpireEnigmas.SpireEnigmasCode.Character.savant;
using SpireEnigmas.SpireEnigmasCode.Extensions;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics;

[Pool(typeof(TheRenegadeRelicPool))]
public abstract class TheRenegadeRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}