using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class StreamOfConsciousnessPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || RarityHelper.GetModifiedRarity(cardPlay.Card) is not CardRarity.Rare)
            return;
        CardModel card = CardFactory.GetDistinctForCombat(Owner.Player, Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint).Where<CardModel>((Func<CardModel, bool>) (c => RarityHelper.GetModifiedRarity(c) is CardRarity.Rare)), 1, Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player);
    }
}