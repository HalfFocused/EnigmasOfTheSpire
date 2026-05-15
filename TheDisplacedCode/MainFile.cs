using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace TheDisplaced.TheDisplacedCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "TheDisplaced"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}
/*
 * Patch #1 required to make Thinking Of You work.
 * This patch just straight-up cancels any attempt to Exhaust the card, making it do nothing instead.
 */
[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]
class CardCmdExhaustPatch
{
    [HarmonyPrefix]
    static bool CancelExhaust(ref Task __result, PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal, bool skipVisuals)
    {
        if (card.Keywords.Contains(DisplacedKeywords.Cherished))
        {
            __result = Task.CompletedTask;
            return false;
        }

        return true;
    }
}
/*
 * Patch #2 required to make Thinking Of You work.
 * This patch checks if a card wants to go to the Exhaust pile after being played but can't, redirecting it to the discard pile.
 * Without this, a card that can't Exhaust but wants to Exhaust when played simply goes nowhere and stays on screen.
 */
[HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
class CardPlayResultPilePatch
{
    [HarmonyPostfix]
    static void CancelExhaust(CardModel __instance, ref PileType __result)
    {
        if (__instance.Keywords.Contains(DisplacedKeywords.Cherished) && __result == PileType.Exhaust)
        {
            __result = PileType.Discard;
        }
    }
}
