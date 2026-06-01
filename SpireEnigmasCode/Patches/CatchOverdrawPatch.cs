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
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerChoiceContextField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, playerField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, cardField),
                CodeInstruction.Call(() => DoSomething(default, default, default, default, default))
            );

        return codeMatcher.Instructions();
    }
    
    static bool DoSomething(int cardsInHand, int maximumHandSize, PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        MainFile.Logger.Info("Attempted Draw: " + card.Title + ". Cards already in hand:" + cardsInHand);

        if (player.HasPower<UnsustainableInconsolablePower>())
        {
            MainFile.Logger.Info("Player has Unsustainable Inconsolable");
            if (cardsInHand >= maximumHandSize - 1)
            {
                MainFile.Logger.Info("Hand Full! Cannot draw " + card.Title + ". Attempting Exhaust.");
                CardCmd.Exhaust(choiceContext, card);
                MainFile.Logger.Info("Returning False");
                return false;
            }
            MainFile.Logger.Info("Hand not full. Allowing Draw:" + card.Title +  ". Returning True");
            return true;
        }
        
        return cardsInHand < maximumHandSize;
    }

}