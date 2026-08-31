using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Extensions;
using BaseLib.Patches.Features;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Character.savant;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;
using Label = System.Reflection.Emit.Label;

namespace SpireEnigmas.SpireEnigmasCode.Patches;

/*
 * This is stored in each card and updated whenever PlayerCombatState.HasEnoughResourcesFor is called on that card.
 * It represents the amount of energy Chirp should lose from it's battery when the card is played.
 * This will be 0 for X cost cards. The battery loss for them is handled in InternalBatteryPower.
 */
class ChirpBatteryCostField
{
    public static readonly SpireField<CardModel, int> ChirpBatteryCost = new(()=>0);
}

class StoryFields
{
    public static readonly SpireField<Player, List<CardModel>> PlayerStory = new(()=>[]);
    
    public static readonly SpireField<Player, int> PlayerStoryMaxLength = new(()=>5);
    public static readonly SpireField<Player, bool> RunEndPlayerStoryHooks = new(()=>false);
    public static readonly SpireField<Player, bool> RunStartPlayerStoryHooks = new(()=>false);
    
    public static readonly SpireField<CardModel, int> ChapterPlayedInto = new(()=>-1);

}

[HarmonyPatch(typeof(ScrollBoxes), nameof(ScrollBoxes.GenerateRandomBundles))]
class ScrollBoxesGenerateRandomBundlesPatch
{
    /*
     * Adds a 1/100 chance for either of the Scroll Boxes rewards to be replaced with 3 copies of "Recurring Dream."
     * It's the Displaced's version of a Claw Pack.
     */
    
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

/*
 * Doubles all card draw during the player's turn if they have the Unsustainable power.
 */
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

[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform), typeof(IEnumerable<CardTransformation>), typeof(Rng),
    typeof(CardPreviewStyle))]
class CardTransformationHookPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                new CodeMatch(OpCodes.Ldfld),
                CodeMatch.Calls(typeof(CardPile).Method(nameof(CardModel.AfterTransformedFrom))),
                CodeMatch.IsLdarg(0),
                new CodeMatch(OpCodes.Ldfld),
                CodeMatch.Calls(typeof(CardPile).Method(nameof(CardModel.AfterTransformedTo)))
            ).ThrowIfInvalid("Could not find Transform hooks");
            
        object originalField = codeMatcher.InstructionAt(1).operand;
        object replacementField = codeMatcher.InstructionAt(4).operand;

        codeMatcher.Advance(6);
            
        codeMatcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, replacementField),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, originalField),
            CodeInstruction.Call(() => CatchTransformation(default, default))
        );

        return codeMatcher.Instructions();
    }

    static void CatchTransformation(CardModel original, CardModel replacement)
    {
        if (replacement.Owner.PlayerCombatState != null)
        {
            if (original is Scrap)
            {
                replacement.EnergyCost.SetUntilPlayed(0);
            }
            
            if (original is FormAndFunction)
            {
                PlayerCmd.GainEnergy(original.DynamicVars.Energy.BaseValue, replacement.Owner);
                CardPileCmd.Draw(new BlockingPlayerChoiceContext(), original.DynamicVars.Cards.BaseValue, replacement.Owner);
            }

            Player player = replacement.Owner;

            MasterworkPower? masterworkPower = player.Creature.GetPower<MasterworkPower>();

            if (masterworkPower is not null)
            {
                masterworkPower.Flash();
                replacement.BaseReplayCount += masterworkPower.Amount;
            }
            
            ImprovisedShieldPower? improvisedShieldPower = player.Creature.GetPower<ImprovisedShieldPower>();

            if (improvisedShieldPower is not null)
            {
                //improvisedShieldPower.Flash();
                improvisedShieldPower.AfterCardTransformed();
            }
        }
    }
}

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
class CardGlowTweakPatch
{
    public static readonly Color chirpPlayableColor = new Color(0.95f, 0.45f, 0f, 0.98f);
    public static readonly Color madeEtherealColor = new Color(0.35f, 0.35f, 0.35f, 0.98f);
    
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0), //this
            new CodeMatch(OpCodes.Call), //get_CardNode
            new CodeMatch(OpCodes.Callvirt), //get_Model
            new CodeMatch(OpCodes.Callvirt), //CanPlay()
            new CodeMatch(OpCodes.Brtrue_S)

        ).ThrowIfInvalid("Could not find card model check");

        object getCardNodeMethod = codeMatcher.InstructionAt(1).operand;
        object getModelMethod = codeMatcher.InstructionAt(2).operand;

        //we advance late for the sake of setting those variables
        codeMatcher.Advance(5);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Callvirt),

            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldsfld), //
            /*
             * We want to sneak in our method here to intercept the loaded static color and potentially change it
             */
            new CodeMatch(OpCodes.Callvirt) //set_Modulate
        ).ThrowIfInvalid("Could not find glow color assignment");

        codeMatcher.Advance(8);

        codeMatcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0), //this
            new CodeInstruction(OpCodes.Call, getCardNodeMethod),
            new CodeInstruction(OpCodes.Callvirt, getModelMethod),
            CodeInstruction.Call(() => PossiblyTweakColor(default, default))
        );

        return codeMatcher.Instructions();
    }

    static Color PossiblyTweakColor(Color oldColor, CardModel cardModel)
    {
        int calculatedBatteryCost = ChirpBatteryCostField.ChirpBatteryCost.Get(cardModel);
        if (calculatedBatteryCost > 0)
        {
            return chirpPlayableColor;
        }

        if (cardModel.Owner.Character is TheDisplaced && cardModel.Keywords.Contains(CardKeyword.Ethereal) &&
            !cardModel.CanonicalInstance.Keywords.Contains(CardKeyword.Ethereal))
        {
            return madeEtherealColor;
        }

        return oldColor;
    }
}

