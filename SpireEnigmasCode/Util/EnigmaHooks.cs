using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmaHooks() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (!card.Keywords.Contains(EnigmaKeywords.Improvise) || fromHandDraw || card.CombatState.CurrentSide != card.Owner.Creature.Side)
            return;
        if (CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>()
                .Count(e => e.Card == card && !e.FromHandDraw && e.RoundNumber == card.CombatState.RoundNumber) != 1)
        {
            card.EnergyCost.SetThisTurn(0);
        }
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel playedCard = cardPlay.Card;
        Player owner = playedCard.Owner;

        if (playedCard.Keywords.Contains(EnigmaKeywords.TimeLoop)) return;

        foreach (CardModel card in owner.PlayerCombatState.AllCards.ToList())
        {
            if (card.Keywords.Contains(EnigmaKeywords.TimeLoop) || card is TimeLoop)
            {
                CardModel result = playedCard.CreateClone();
                CardCmd.ApplyKeyword(result, EnigmaKeywords.TimeLoop);

                await CardCmd.Transform(card, result, card.Pile.Type == PileType.Hand ? CardPreviewStyle.HorizontalLayout : CardPreviewStyle.None);
            }
        }
    }
}