using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class InternalBleedingPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if(target is null) return 0;

        int uniqueDebuffs = target.Powers.Count(p => p.TypeForCurrentAmount == PowerType.Debuff && p is not ITemporaryPower);
        return Owner != dealer || !props.IsPoweredAttack() ? 0M : Amount * uniqueDebuffs;
    }
}