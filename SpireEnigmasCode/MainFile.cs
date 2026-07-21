using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Extensions;
using BaseLib.Utils;
using BaseLib.Utils.Patching;
using Godot;
using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

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
        Harmony.DEBUG = true;
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
        if (player.Character is TheDisplaced)
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
            if (__instance._model.Amount >= ForetoldPower.INFINITY)
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
            if (__instance.Amount >= ForetoldPower.INFINITY)
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

[HarmonyPatch(typeof(CardModel), "ShouldGlowGold", MethodType.Getter)]
class FreestyleGlowGoldPatch
{
    [HarmonyPostfix]
    static void MakeGlowGold(CardModel __instance, ref bool __result)
    {
        if (__instance.Owner.HasPower<FreestylePower>())
        {
            if (FreestylePower.CardDrawnDuringThisTurn(__instance) && __instance.Type is CardType.Attack)
            {
                __result = true;
            }
        }
    }
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), 
    typeof(PlayerChoiceContext), typeof(Decimal), typeof(Player), typeof(bool))]
class DrawIncreasePatch
{
    [HarmonyPrefix]
    static void IncreaseCardDraw(PlayerChoiceContext choiceContext,
        ref Decimal count,
        Player player,
        bool fromHandDraw)
    {
        if (player.HasPower<UnsustainablePower>() && !fromHandDraw && count != 0)
        {
            UnsustainablePower? power = player.Creature.GetPower<UnsustainablePower>();
            power.Flash();
            count *= power.Amount;
        }
    }
}
