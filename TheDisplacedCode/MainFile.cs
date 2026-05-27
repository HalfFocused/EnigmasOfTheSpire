using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Random;
using TheDisplaced.TheDisplacedCode.Cards;
using TheDisplaced.TheDisplacedCode.Cards.common;
using TheDisplaced.TheDisplacedCode.Powers;

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
            card.RemoveKeyword(DisplacedKeywords.Cherished);
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
            __instance.RemoveKeyword(DisplacedKeywords.Cherished);
            __result = PileType.Discard;
        }
    }
}

/*
 * Adds a 1/100 chance for either of the Scroll Boxes rewards to be replaced with 3 copies of "Recurring Dream."
 * It's the Displaced's version of a Claw Pack.
 */
[HarmonyPatch(typeof(ScrollBoxes), nameof(ScrollBoxes.GenerateRandomBundles))]
class ScrollBoxesGenerateRandomBundlesPatch
{
    [HarmonyPostfix]
    static void ChanceForNightmarePack(Player player, ref List<IReadOnlyList<CardModel>> __result)
    {
        Rng rewardsRng = player.PlayerRng.Rewards;
        if (player.Character is Character.TheDisplaced)
        {
            for (int i = 0; i < __result.Count; i++)
            {
                if (rewardsRng.NextInt(100) < 1)
                {
                    CardModel recurringDream = ModelDb.Card<RecurringDream>();
                    __result[i] =
                    [
                        recurringDream, recurringDream, recurringDream
                    ];
                }
            }
        }
    }
}
/*
[HarmonyPatch(typeof(CardModel), "HoverTips", MethodType.Getter)]
internal static class AddFlavorTextPatch
{
    [HarmonyPostfix]
    static void AddRareCardFlavorText(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is TheDisplacedCard && __instance.Rarity == CardRarity.Rare)
        {
            TheDisplacedCard card = ((TheDisplacedCard)__instance);
            HoverTip flavorText = new HoverTip(card.FlavorTextTitleLocString, card.FlavorTextBodyLocString);
            __result.AddItem(flavorText);
        }
    }
}
*/

/*
 * Modify the power icon of the Foretold power to display an infinity instead of a number under certain conditions.
 */
[HarmonyPatch(typeof(NPower), nameof(NPower.RefreshAmount))]
class PowerNodeInfinityDisplayPatch
{
    [HarmonyPostfix]
    static void ChangeLabelToInfinity(NPower __instance)
    {
        if (__instance._model is ForetoldPower)
        {
            if (__instance._model.Amount == ForetoldPower.INFINITY)
            {
                __instance._amountLabel.SetTextAutoSize("∞");
            }
        }
    }
}

/*
 * Modify the description of the Foretold power if it is infinite.
 */
[HarmonyPatch(typeof(PowerModel), "SmartDescription", MethodType.Getter)]
class ForetoldPowerDescriptionPatch
{
    [HarmonyPostfix]
    static void ChangeDescriptionToInfinity(PowerModel __instance, ref LocString __result)
    {
        if (__instance is ForetoldPower)
        {
            if (__instance.Amount == ForetoldPower.INFINITY)
            {
                __result = new LocString("powers", ((ForetoldPower)__instance).InfiniteDescriptionLocKey);
            }
        }
    }
}

/*
 * Stop debuff durations from decreasing if the creature has Stasis
 */
[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.TickDownDuration))]
class StasisDurationPatch
{
    [HarmonyPrefix]
    static bool CancelTickDown(ref Task __result, PowerModel power)
    {
        if (power.TypeForCurrentAmount == PowerType.Debuff && power is not StasisPower)
        {
            if (power.Owner.HasPower<StasisPower>())
            {
                __result = Task.CompletedTask;
                return false;
            }
        }

        return true;
    }
}
