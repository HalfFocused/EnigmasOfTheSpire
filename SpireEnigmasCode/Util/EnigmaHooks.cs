using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmaHooks() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel playedCard = cardPlay.Card;
        Player owner = playedCard.Owner;

        if (playedCard.Keywords.Contains(EnigmaEnums.TimeLoop)) return;

        foreach (CardModel card in owner.PlayerCombatState.AllCards.ToList())
        {
            if (card.Keywords.Contains(EnigmaEnums.TimeLoop) || card is TimeLoop)
            {
                CardModel result = playedCard.CreateClone();
                CardCmd.ApplyKeyword(result, EnigmaEnums.TimeLoop);

                await CardCmd.Transform(card, result, card.Pile.Type == PileType.Hand ? CardPreviewStyle.HorizontalLayout : CardPreviewStyle.None);
            }
        }
    }
}