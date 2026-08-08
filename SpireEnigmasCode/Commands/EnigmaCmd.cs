using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;

namespace SpireEnigmas.SpireEnigmasCode.Commands;

public static class EnigmaCmd
{
    public static async Task<CardPileAddResult?> ChooseAndTransformInto(
        PlayerChoiceContext choiceContext,
        Player inventor,
        CardModel? result,
        PileType inventFromPile = PileType.Hand)
    {
        CardModel? toTransform;
        if (inventFromPile == PileType.Hand)
        {
            toTransform = (await CardSelectCmd.FromHand(choiceContext, inventor,
                new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), null, result)).FirstOrDefault();
        }
        else
        {
            CardPile pile = inventFromPile.GetPile(inventor);
            toTransform = (await CardSelectCmd.FromCombatPile(choiceContext, pile, inventor, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), null)).FirstOrDefault();
        }

        if (toTransform is null || result is null) return null;
        return await CardCmd.Transform(toTransform, result, inventFromPile == PileType.Hand ? CardPreviewStyle.None : CardPreviewStyle.HorizontalLayout);
    }

    public static async Task InventGadget<T>(Player owner, ICombatState combatState) where T : AbstractGadget
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        ;
        
        if (PileType.Hand.GetPile(owner).Cards.OfType<AbstractGadget>().Any())
        {
            T gadgetBeingInvented = ModelDb.Get<T>();
            
            foreach (AbstractGadget handGadget in PileType.Hand.GetPile(owner).Cards.OfType<AbstractGadget>().ToList())
            {
                handGadget.TakeAttributesFrom(gadgetBeingInvented);
                NCard? cardNode = NCard.FindOnTable(handGadget);
                cardNode?.AddChildSafely(NCardSmithVfx.Create(cardNode));
            }
        }
        else
        {
            T gadgetBeingInvented = combatState.CreateCard<T>(owner);
            await CardPileCmd.AddGeneratedCardsToCombat([gadgetBeingInvented], PileType.Hand, owner);
        }
    }
}