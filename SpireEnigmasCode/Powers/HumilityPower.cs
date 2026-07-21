using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class HumilityPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => (object) new Data();
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || RarityHelper.GetModifiedRarity(cardPlay.Card) is not CardRarity.Common)
            return Task.CompletedTask;
        GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, Amount);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount;
        if (cardPlay.Card.Owner.Creature != Owner || !GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out amount) || amount <= 0)
            return;
        await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null, true);
    }

    public class Data
    {
        /// <summary>
        /// Keep track of the cards we've seen played and the power amount at the time they were played.
        /// This lets After Image avoid triggering on cards that started play before it was applied, and avoid gaining
        /// extra block on multiple plays of After Image.
        /// </summary>
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new ();
    }
}