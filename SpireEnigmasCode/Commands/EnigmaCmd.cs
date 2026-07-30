using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpireEnigmas.SpireEnigmasCode.Cards.other;

namespace SpireEnigmas.SpireEnigmasCode.Commands;

public static class EnigmaCmd
{
    public static async Task<CardPileAddResult?> Invent(
        PlayerChoiceContext choiceContext,
        Player inventor,
        CardModel card)
    {
        CardModel? toTransform = (await CardSelectCmd.FromHand(choiceContext, inventor,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), null, card)).FirstOrDefault();

        if (toTransform is null) return null;
        return await CardCmd.Transform(toTransform, card, CardPreviewStyle.None);
    }
}