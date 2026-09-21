using System.Reflection;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using SpireEnigmas.SpireEnigmasCode.Character.chronicle;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Character.sacrifice;
using SpireEnigmas.SpireEnigmasCode.Character.savant;

namespace SpireEnigmas.SpireEnigmasCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "SpireEnigmas"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";
    
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);
    
    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        
        CustomCharacterUtils.TryOrderCustomCharacters(
            typeof(TheDisplaced),
            typeof(TheSacrifice),
            typeof(TheSavant),
            typeof(TheChronicle)
        );
    }
}
 