using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Events;

public class EnigmaSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        foreach (CardModel card in retainedCards)
        {
            RetainTrackingField.RetainedLastTurn.Set(card, true);
        }
        
        List<CardModel> refinedCards = retainedCards.Where(c => c.Keywords.Contains(EnigmaKeywords.Refine)).ToList();

        List<CardModel> cardsJustRefined = new List<CardModel>();
        
        foreach (CardModel refineCard in refinedCards)
        {
            if (!RefinementFields.IsRefined.Get(refineCard))
            {
                foreach (DynamicVar var in refineCard.DynamicVars.Values)
                {
                    var.BaseValue += RefinementFields.RefineBy.Get(var);
                }
                cardsJustRefined.Add(refineCard);
                RefinementFields.IsRefined.Set(refineCard, true);
                if(refineCard is IOnRefinementChanged onRefinementChangedCard) onRefinementChangedCard.OnRefinementChanged(true);
            }
        }

        /*
         * Unrefine all Refine cards that weren't just Refined
         */
        foreach (CardModel refineCard in player.PlayerCombatState!.AllCards.Except(cardsJustRefined)
                     .Where(c => c.Keywords.Contains(EnigmaKeywords.Refine)))
        {
            foreach (CardModel card in flushedCards)
            {
                RetainTrackingField.RetainedLastTurn.Set(card, false);
            }
            
            if (RefinementFields.IsRefined.Get(refineCard))
            {
                foreach (DynamicVar var in refineCard.DynamicVars.Values)
                {
                    var.BaseValue -= RefinementFields.RefineBy.Get(var);
                }
                
                RefinementFields.IsRefined.Set(refineCard, false);
                if(refineCard is IOnRefinementChanged onRefinementChangedCard) onRefinementChangedCard.OnRefinementChanged(false);
            }
        }

        return Task.CompletedTask;
    }
}