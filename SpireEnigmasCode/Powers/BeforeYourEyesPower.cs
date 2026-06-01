using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class BeforeYourEyesPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => new Data();

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Keywords.Contains(CardKeyword.Ethereal))
        {
            if (causedByEthereal)
            {
                GetInternalData<Data>().queuedCards += Amount;
            }
            else{
                for (int i = 0; i < Amount; i++)
                {
                    Flash();
                    CardModel newCard = CardFactory.GetDistinctForCombat(Owner.Player, Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint), 1, Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                    if (newCard != null)
                    {
                        await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, Owner.Player);
                    }
                }
            }
        }
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (GetInternalData<Data>().queuedCards > 0)
        {
            Flash();
            for (int i = 0; i < GetInternalData<Data>().queuedCards; i++)
            {
                CardModel newCard = CardFactory.GetDistinctForCombat(Owner.Player, Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint), 1, Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                if (newCard != null)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, Owner.Player);
                }
            }
            
            GetInternalData<Data>().queuedCards = 0;
        }
    }
    
    public class Data
    {
        public int queuedCards;
    }
}