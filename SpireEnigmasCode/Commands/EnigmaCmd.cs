using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
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
    
    public static async Task<IEnumerable<CardModel>> DrawAndSelectFromDrawn(
        PlayerChoiceContext choiceContext,
        Player player,
        CardSelectorPrefs prefs,
        decimal amount,
        AbstractModel source
        )
    {
        IEnumerable<CardModel> drawnCards = await CardPileCmd.Draw(choiceContext, amount, player);

        return await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            c => drawnCards.Contains(c),
            source);
    }

    public static async Task InventGadget(Player owner, ICombatState combatState, IEnumerable<DynamicVar> dynamicVars, IEnumerable<CardKeyword>? keywords = null, int replay = 0)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;
        
        if (PileType.Hand.GetPile(owner).Cards.OfType<Gadget>().Any())
        {
            foreach (Gadget handGadget in PileType.Hand.GetPile(owner).Cards.OfType<Gadget>().ToList())
            {
                handGadget.TakeAttributesFrom(dynamicVars, keywords, replay);
                NCard? cardNode = NCard.FindOnTable(handGadget);
                cardNode?.AddChildSafely(NCardSmithVfx.Create(cardNode));
            }
        }
        else
        {
            Gadget gadgetBeingInvented = combatState.CreateCard<Gadget>(owner);
            gadgetBeingInvented.TakeAttributesFrom(dynamicVars, keywords, replay);
            await CardPileCmd.AddGeneratedCardsToCombat([gadgetBeingInvented], PileType.Hand, owner);
            NCard? cardNode = NCard.FindOnTable(gadgetBeingInvented);
            cardNode?.AddChildSafely(NCardSmithVfx.Create(cardNode));
        }
    }
}