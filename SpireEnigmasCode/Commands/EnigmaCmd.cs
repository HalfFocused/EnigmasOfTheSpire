using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace SpireEnigmas.SpireEnigmasCode.Commands;

public static class EnigmaCmd
{
    public static async Task<CardPileAddResult?> Invent(
        PlayerChoiceContext choiceContext,
        Player inventor,
        CardModel card,
        PileType inventFromPile = PileType.Hand)
    {
        CardModel? toTransform;
        if (inventFromPile == PileType.Hand)
        {
            toTransform = (await CardSelectCmd.FromHand(choiceContext, inventor,
                new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), null, card)).FirstOrDefault();
        }
        else
        {
            CardPile pile = inventFromPile.GetPile(inventor);
            toTransform = (await CardSelectCmd.FromCombatPile(choiceContext, pile, inventor, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), null)).FirstOrDefault();
        }
        


        if (toTransform is null) return null;
        return await CardCmd.Transform(toTransform, card, inventFromPile == PileType.Hand ? CardPreviewStyle.None : CardPreviewStyle.HorizontalLayout);
    }
}