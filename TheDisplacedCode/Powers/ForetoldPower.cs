using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheDisplaced.TheDisplacedCode.Cards.uncommon;

namespace TheDisplaced.TheDisplacedCode.Powers;

public class ForetoldPower: TheDisplacedPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageIncrease", 2.0M),
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
        if (target != this.Owner || !props.IsPoweredAttack() || ((BoolVar) DynamicVars["UsedThisTurn"]).BoolVal)
            return 1M;
        Decimal amount1 = this.DynamicVars["DamageIncrease"].BaseValue;
        
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
            ((BoolVar)DynamicVars["UsedThisTurn"]).BoolVal = true;
            foreach(CardModel card in PileType.Discard.GetPile(cardPlay.Card.Owner).Cards.ToList<CardModel>()){
                if (card is CarefulScheme)
                {
                    await CardPileCmd.Add(card, PileType.Hand);
                }
            }
        }
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Enemy)
            return;
        
        ((BoolVar)DynamicVars["UsedThisTurn"]).BoolVal = false;
        ((BoolVar)DynamicVars["HitDuringCardPlay"]).BoolVal = false;
        
        await PowerCmd.TickDownDuration(this);
    }
}