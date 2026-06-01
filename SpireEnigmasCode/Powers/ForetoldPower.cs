using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class ForetoldPower: SpireEnigmaPower
{
    public const int INFINITY = Really.bigNumber;
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public virtual string InfiniteDescriptionLocKey => Id.Entry + ".infiniteDescription";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("DamageIncrease", 2.0M),
        new BoolVar("UsedThisTurn", false),
        new BoolVar("HitDuringCardPlay", false)
    ];
    
    /*
     * Double the damage on the creature if Foretold hasn't been used on it this turn.
     */
    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack() || ((BoolVar) DynamicVars["UsedThisTurn"]).BoolVal)
            return 1M;
        
        Decimal amount1 = DynamicVars["DamageIncrease"].BaseValue;
        if (dealer != null)
        {
            ProphesizePower? power = dealer.GetPower<ProphesizePower>();
            if (power != null)
                amount1 += power.Amount / 100M;
        }

        return amount1;
    }
    
    /*
     * If the creature is attacked and Foretold hasn't been used this turn, mark it to be used after card play.
     */
    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack() && !(cardSource is Omnislice))
            return;

        if (!((BoolVar)DynamicVars["UsedThisTurn"]).BoolVal)
        {
            Flash();
            ((BoolVar)DynamicVars["HitDuringCardPlay"]).BoolVal = true;
        }
    }
    
    /*
     * Mark Foretold as used after any card finishes playing if it is marked as having been used during the card play.
     */
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (((BoolVar)DynamicVars["HitDuringCardPlay"]).BoolVal)
        {
            if (!((BoolVar)DynamicVars["UsedThisTurn"]).BoolVal)
            {
                ((BoolVar)DynamicVars["UsedThisTurn"]).BoolVal = true;
                foreach(CardModel card in PileType.Discard.GetPile(cardPlay.Card.Owner).Cards.ToList<CardModel>()){
                    if (card is CarefulScheme)
                    {
                        await CardPileCmd.Add(card, PileType.Hand);
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
        ((BoolVar)DynamicVars["UsedThisTurn"]).BoolVal = false;
        ((BoolVar)DynamicVars["HitDuringCardPlay"]).BoolVal = false;
        if (side != CombatSide.Enemy)
            return;

        if (Amount != INFINITY)
            await PowerCmd.TickDownDuration(this);
        
    }

    public override Decimal ModifyPowerAmountGiven(
        PowerModel power,
        Creature giver,
        Decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        if (power == this && Amount == INFINITY)
        {
            return 0;
        }

        return amount;
    }
}