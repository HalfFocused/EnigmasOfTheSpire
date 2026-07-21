using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
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
    
    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if(target != Owner) return 0;
        return !props.IsPoweredAttack() ? 0M : Amount;
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;

        if (Owner.HasPower<AfterburnPower>())
        {
            PowerModel afterburnPower = Owner.GetPower<AfterburnPower>();
            await PowerCmd.TickDownDuration(afterburnPower);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -Math.Ceiling(Amount / 2.0m), null, null);
        }
        
    }
    
}