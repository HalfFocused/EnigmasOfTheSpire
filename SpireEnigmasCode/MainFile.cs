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
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Powers;

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
    }
}

class DireHPCostField
{
    public static readonly SpireField<CardModel, int> DireHPCost = new(()=>0);
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
        if (card.Keywords.Contains(EnigmaKeywords.Cherished))
        {
            card.RemoveKeyword(EnigmaKeywords.Cherished);
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
 *
 * This patch also handles Exhausting cards that were played via Dire.
 */
[HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
class CardPlayResultPilePatch
{
    [HarmonyPostfix]
    static void CancelExhaust(CardModel __instance, ref PileType __result)
    {
        if (__instance.Keywords.Contains(EnigmaKeywords.Dire))
        {
            if (DireHPCostField.DireHPCost.Get(__instance) > 0)
            {
                __result = PileType.Exhaust;
            }
        }

        if (__instance.Keywords.Contains(EnigmaKeywords.Cherished) && __result == PileType.Exhaust)
        {
            __instance.RemoveKeyword(EnigmaKeywords.Cherished);
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

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
class DirePlayablePatch
{
    [HarmonyPostfix]
    static void AllowPlayingIfHPSufficient(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
    {
        int calculatedEnergyCost = Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
        if (card.Keywords.Contains(EnigmaKeywords.Dire))
        {
            if (!__result && reason == UnplayableReason.EnergyCostTooHigh)
            {
                if (calculatedEnergyCost <= __instance.Energy + card.Owner.Creature._currentHp)
                {
                    //num1 represents the calculated energy cost of the card
                    DireHPCostField.DireHPCost.Set(card, calculatedEnergyCost - __instance.Energy);
                    __result = true;
                    reason = UnplayableReason.None;
                    return;
                }
            }
        }
        DireHPCostField.DireHPCost.Set(card, 0);
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendEnergy))]
class SpendHPCostPatch
{
    [HarmonyPrefix]
    static void SpendCalculatedHPCost(CardModel __instance)
    {

        if (__instance.Keywords.Contains(EnigmaKeywords.Dire))
        {
            int calculatedHPCost = DireHPCostField.DireHPCost.Get(__instance);
            if (calculatedHPCost > 0)
            {
                CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), __instance.Owner.Creature, calculatedHPCost, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, __instance);
            }
        }

    }
}

/*
 * Modify the description of the Foretold power if it is infinite.
 */
[HarmonyPatch(typeof(CardModel), "ShouldGlowRed", MethodType.Getter)]
class DireCardsGlowRedPatch
{
    [HarmonyPostfix]
    static void MakeGlowRed(CardModel __instance, ref bool __result)
    {
        if (__instance.Keywords.Contains(EnigmaKeywords.Dire))
        {
            int calculatedHPCost = DireHPCostField.DireHPCost.Get(__instance);
            
            if (calculatedHPCost > 0 && calculatedHPCost <= __instance.Owner.Creature._currentHp)
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
        MainFile.Logger.Info("Patch caught draw: " + count);
        if (player.HasPower<UnsustainableInconsolablePower>() && !fromHandDraw && count != 0)
        {
            MainFile.Logger.Info("Patch attempts modification");
            UnsustainableInconsolablePower power = player.Creature.GetPower<UnsustainableInconsolablePower>();
            power.Flash();
            count *= power.Amount;
        }
    }
}

[HarmonyDebug]
[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(Decimal),
    typeof(Player), typeof(bool))]
class ExhaustOverflowDrawPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchEndForward(
                CodeMatch.IsLdarg(0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CardPileCmd), "<hand>5__4")),
                CodeMatch.Calls(typeof(CardPile).PropertyGetter("Cards")),
                CodeMatch.Calls(typeof(IReadOnlyCollection<CardModel>).PropertyGetter("Count"))//,
                //CodeMatch.Calls(typeof(CardPile).PropertyGetter("MaxCardsInHand")),
                //new CodeMatch(OpCodes.Bge)
            )
            .ThrowIfInvalid("Could not find hand size check statement.")
            .Advance()
            .MatchEndForward(
                new CodeMatch(OpCodes.Bge)
            )
            .SetOpcodeAndAdvance(OpCodes.Brfalse)
            .Advance(-1)
            .InsertAndAdvance(
                //CodeInstruction.LoadArgument(1),
                //CodeInstruction.LoadArgument(3),
                CodeInstruction.Call(() => DoSomething(default, default/*, default, default*/))
            );

        return codeMatcher.Instructions();
    }
    
    static bool DoSomething(int cardsInHand, int maximumHandSize/*, PlayerChoiceContext choiceContext, Player player*/)
    {
        MainFile.Logger.Info("Did Something. Num1: " + cardsInHand + " Num2: " + maximumHandSize);

        return cardsInHand < maximumHandSize;
    }
}
