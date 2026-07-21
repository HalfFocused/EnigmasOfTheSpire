using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class TheGoldenMeanPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int ActivationsThisTurn = 0;
    private bool JustCreated = true;
    
    CardModel? CommonCard = null;
    CardModel? UncommonCard = null;
    CardModel? RareCard = null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CommonCard"),
        new StringVar("UncommonCard"),
        new StringVar("RareCard")
    ];
    
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel cardPlayed = cardPlay.Card;
        
        if (cardPlayed.Owner != Owner.Player || !CombatManager.Instance.IsInProgress || cardPlayed.IsDupe || ActivationsThisTurn >= Amount)
            return;
        
        if (CommonCard is null && RarityHelper.GetModifiedRarity(cardPlayed) is CardRarity.Common)
        {
            SetCommonCard(cardPlayed);
        }
        
        if (UncommonCard is null && RarityHelper.GetModifiedRarity(cardPlayed) is CardRarity.Uncommon)
        {
            SetUncommonCard(cardPlayed);
        }
        
        if (RareCard is null && RarityHelper.GetModifiedRarity(cardPlayed) is CardRarity.Rare)
        {
            SetRareCard(cardPlayed);
        }

        /*
         * Lol. Lmao even. I can't wait to see exactly how this doesn't work.
         */
        if (JustCreated && RareCard is TheGoldenMean)
        {
            SetRareCard(null);
            JustCreated = false;
        }

        if (CommonCard is not null && UncommonCard is not null && RareCard is not null && ActivationsThisTurn < Amount)
        {
            Flash();
            await CardCmd.AutoPlay(choiceContext, CommonCard.CreateDupe(Owner.Player), null);
            await Cmd.CustomScaledWait(0.2f, 0.65f);
            await CardCmd.AutoPlay(choiceContext, UncommonCard.CreateDupe(Owner.Player), null);
            await Cmd.CustomScaledWait(0.2f, 0.65f);
            await CardCmd.AutoPlay(choiceContext, RareCard.CreateDupe(Owner.Player), null);
            SetCommonCard(null);
            SetUncommonCard(null);
            SetRareCard(null);
            ActivationsThisTurn++;
        }
    }
    
    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        SetCommonCard(null);
        SetUncommonCard(null);
        SetRareCard(null);
        ActivationsThisTurn = 0;
        return Task.CompletedTask;
    }

    private void SetCommonCard(CardModel? card)
    {
        CommonCard = card;
        ((StringVar) DynamicVars["CommonCard"]).StringValue = CommonCard is null ? "" : CommonCard.Title;
    }
    
    private void SetUncommonCard(CardModel? card)
    {
        UncommonCard = card;
        ((StringVar) DynamicVars["UncommonCard"]).StringValue = UncommonCard is null ? "" : UncommonCard.Title;
    }
    
    private void SetRareCard(CardModel? card)
    {
        RareCard = card;
        ((StringVar) DynamicVars["RareCard"]).StringValue = RareCard is null ? "" : RareCard.Title;
    }
}