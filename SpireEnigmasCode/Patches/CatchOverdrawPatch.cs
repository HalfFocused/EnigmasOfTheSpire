using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Patches;

public class CatchOverdrawPatch
{
    private static Type? stateMachineType;

    public static void Patch(Harmony harmony)
    {
        MainFile.Logger.Info("Performing CatchOverdrawPatch");
        harmony.PatchAsyncMoveNext(AccessTools.Method(typeof(CardPileCmd), nameof(CardPileCmd.Draw),
            [
                typeof(PlayerChoiceContext), typeof(Decimal), typeof(Player), typeof(bool)
            ]),
            out stateMachineType,
            transpiler: AccessTools.Method(typeof(CatchOverdrawPatch), nameof(TranspilerPatch)));
    }
    
    static IEnumerable<CodeInstruction> TranspilerPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        FieldInfo playerChoiceContextField = stateMachineType.FindStateMachineField("choiceContext");
        FieldInfo playerField = stateMachineType.FindStateMachineField("player");
        FieldInfo cardField = stateMachineType.FindStateMachineField("card");
        //FieldInfo iteratorField = stateMachineType.FindStateMachineField("i");
        
        var codeMatcher = new CodeMatcher(instructions, generator);
        
        Label numReassignment = generator.DefineLabel();
        
        codeMatcher
            /*
             * First match: Find where num is assigned and potentially mess with that number.
             */
            .MatchEndForward(
                new CodeMatch(OpCodes.Sub),
                CodeMatch.Calls(AccessTools.Method(typeof(Math), nameof(Math.Max), [typeof(int), typeof(int)])),
                CodeMatch.StoresLocal("num")
            )
            .ThrowIfInvalid("Could not find num field setter.")
            .InsertAndAdvance(
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerChoiceContextField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerField),
                CodeInstruction.Call(() => ModifyPerceivedAllowedDraw(default, default, default))
            )
            /*
             * Second match inserts code before the break statement to override the logic of
             * hand.Cards.Count < CardPile.MaxCardsInHand
             * with the result from ReceiveCardDrawOrCardDrawAttempt instead
             */
            .MatchEndForward(
                CodeMatch.IsLdarg(0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CardPileCmd), "<hand>5__4")),
                CodeMatch.Calls(typeof(CardPile).PropertyGetter("Cards")),
                CodeMatch.Calls(typeof(IReadOnlyCollection<CardModel>).PropertyGetter("Count")) //,
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
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerChoiceContextField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, cardField),
                CodeInstruction.Call(() =>
                    ReceiveCardDrawOrCardDrawAttempt(default, default, default, default, default))//,
                //new CodeInstruction(OpCodes.Brtrue, numReassignment)
            )
            .InsertAfterAndAdvance(
            )
            /*
             * Third match: Essentially the exact same as the first.
             * The num setting logic happens twice, so we need to patch it twice.
             */
            .MatchEndForward(
                new CodeMatch(OpCodes.Sub),
                CodeMatch.Calls(AccessTools.Method(typeof(Math), nameof(Math.Max), [typeof(int), typeof(int)])),
                CodeMatch.StoresLocal("num")
            )
            .ThrowIfInvalid("Could not find num field setter.")
            .Advance()
            /*
             * save this spot for later cause we want to be able to jump to it after breaking everything
             */
            .AddLabels([numReassignment])
            /*
             * theres gotta be a better way of doing this but i dont care. i call it the matcher slide.
             */
            .Advance(-1)
            .InsertAndAdvance(
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerChoiceContextField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerField),
                CodeInstruction.Call(() => ModifyPerceivedAllowedDraw(default, default, default))
            );

        return codeMatcher.Instructions();
    }
    
    /*
     * Base game code prevents draws that it expects to exceed hand size before they happen.
     * It does this by tracking a number that is equal to the player's max cards - cards in hand.
     * Immediately before this hook is called, this number is updated. If it is equal to 0, the
     * draw ends early and isn't caught at all, even by the ReceiveCardDrawOrCardDrawAttempt hook.
     *
     * What does this mean in practice? It means you should set it to 1 if you want to catch
     * overdraws. Otherwise the game simply won't allow them.
     */
    static int ModifyPerceivedAllowedDraw(int oldExpectedDraw, PlayerChoiceContext choiceContext, Player player)
    {
        MainFile.Logger.Info("Modify perceived allowed draw called.");
        return 1;
    }
    
    /*
     * This method is called whenever the player attempts to draw a card, hand full or not.
     *
     * If the hand is not full:
     * Returning true draws the card
     * Returning false fails to draw the card
     *
     * If the hand is full:
     * Returning true causes future expected card draws to happen as well, even though they are also likely to get caught by this method
     * Return false to cut off the remaining card draws
     */
    static bool ReceiveCardDrawOrCardDrawAttempt(int cardsInHand, int maximumHandSize, PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        MainFile.Logger.Info("Attempted Draw: " + card.Title + ". Cards already in hand:" + cardsInHand);

        if (player.HasPower<UnsustainablePower>())
        {
            MainFile.Logger.Info("Player has Unsustainable Inconsolable");
            if (cardsInHand >= maximumHandSize)
            {
                MainFile.Logger.Info("Hand Full! Cannot draw " + card.Title + ". Attempting Exhaust.");
                CardCmd.Exhaust(choiceContext, card);
                return true;
            }
            MainFile.Logger.Info("Hand not full. Allowing Draw:" + card.Title +  ". Returning True");
            return true;
        }
        
        return cardsInHand < maximumHandSize;
    }

}