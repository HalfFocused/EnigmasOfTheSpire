using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireEnigmas.SpireEnigmasCode.Extensions;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public abstract class SpireEnigmaPower : CustomPowerModel
{
    //Loads from TheDisplaced/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    
    public static HoverTip GetStaticHoverTip(string locEntry)
    {
        const string locTable = "static_hover_tips";
        return new HoverTip(
            new LocString(locTable, locEntry + ".title"),
            new LocString(locTable, locEntry + ".description")
        );
    }
    
    public static HoverTip ChirpHoverTip()
    {
        return GetStaticHoverTip("SPIREENIGMAS-CHIRP");
    }
    
    public static HoverTip GadgetHoverTip()
    {
        return GetStaticHoverTip("SPIREENIGMAS-GADGET");
    }
    
    public static HoverTip InventHoverTip()
    {
        return GetStaticHoverTip("SPIREENIGMAS-INVENT");
    }
    
    public static HoverTip StoryHoverTip()
    {
        return GetStaticHoverTip("SPIREENIGMAS-STORY");
    }
}