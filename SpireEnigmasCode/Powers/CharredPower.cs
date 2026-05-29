using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class CharredPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool hitDuringAttack = false;
    private int amountBeforeAttack = 0;
    
    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if(target != Owner) return 0;
        amountBeforeAttack = Amount;
        return !props.IsPoweredAttack() ? 0M : Amount;
    }
    
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

        if (!hitDuringAttack)
        {
            Flash();
            hitDuringAttack = true;
        }
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (hitDuringAttack)
        {
            if (amountBeforeAttack == Amount)
            {
                await PowerCmd.Remove(this);
            }
            else
            {
                await PowerCmd.ModifyAmount(choiceContext, this, -amountBeforeAttack, null, null);
                hitDuringAttack = false;
            }
            
        }
    }
    
}