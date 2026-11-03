#region

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Character.renegade;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

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
            RetainTrackingFields.RetainedLastTurn.Set(card, true);
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
                if (refineCard is IOnRefinementChanged onRefinementChangedCard)
                    onRefinementChangedCard.OnRefinementChanged(true);
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
                RetainTrackingFields.RetainedLastTurn.Set(card, false);
            }

            if (RefinementFields.IsRefined.Get(refineCard))
            {
                foreach (DynamicVar var in refineCard.DynamicVars.Values)
                {
                    var.BaseValue -= RefinementFields.RefineBy.Get(var);
                }

                RefinementFields.IsRefined.Set(refineCard, false);
                if (refineCard is IOnRefinementChanged onRefinementChangedCard)
                    onRefinementChangedCard.OnRefinementChanged(false);
            }
        }

        return Task.CompletedTask;
    }

    /*
     * Remove Retain Until Played from the card if it is played.
     * We do this in the before hook instead of the after so that Spectre Form's timing works.
     */
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (RetainTrackingFields.HasRetainUntilPlayed.Get(card))
        {
            RetainTrackingFields.HasRetainUntilPlayed.Set(card, false);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (RefinementFields.IsRefined.Get(card))
        {
            await EnigmaHooks.Dispatch<IAfterRefinedCardPlayed>(card.CombatState,
                m => m.AfterRefinedCardPlayed(choiceContext, card.Owner, card));
        }
    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Player?.Character is TheRenegade)
        {
            await AdversaryCmd.SummonAdversary(new ThrowingPlayerChoiceContext(), creature.Player);
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber == 1)
        {
            foreach (Creature participant in participants)
            {
                if (participant.Player?.Character is TheRenegade)
                {
                    await AdversaryCmd.SummonAdversary(new ThrowingPlayerChoiceContext(), participant.Player);
                }
            }
        }
    }

    public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            ICombatState combatState = participants.FirstOrDefault().CombatState;
            foreach (Player player in combatState!.Players)
            {
                if (EnigmasHelper.IsPlayerAdversaryAlive(player))
                {
                    Adversary? adversary = (Adversary?) EnigmasHelper.GetAdversaryFromPlayer(player)?.Monster;
                    if (adversary is not null) await adversary.PerformTurn();
                }
            }
        }
    }

    /*
     * Cache all calculated move damages on each target at the end of the player's turn
     */
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            foreach (Creature creature in participants)
            {
                if (creature.Player is not null)
                {
                    if (EnigmasHelper.IsPlayerAdversaryAlive(creature.Player))
                    {
                        Creature? adversary = EnigmasHelper.GetAdversaryFromPlayer(creature.Player);

                        if (adversary?.Monster is Adversary theAdversary)
                        {
                            theAdversary.CachePlayerIntentDamage();
                        }
                    }
                }
            }
        }
    }
}