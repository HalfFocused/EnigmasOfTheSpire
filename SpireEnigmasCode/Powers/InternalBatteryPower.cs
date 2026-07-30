using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class InternalBatteryPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override bool ShouldPlayVfx => false;
    
    public override int DisplayAmount => DynamicVars.Energy.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(0)
    ];

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature != Owner;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    public void GainReserve(decimal reserveAmount)
    {
        DynamicVars.Energy.BaseValue += reserveAmount;
        InvokeDisplayAmountChanged();
    }
    
    public void LoseReserve(decimal reserveAmount)
    {
        DynamicVars.Energy.BaseValue = Math.Max(DynamicVars.Energy.BaseValue - reserveAmount, 0);
        InvokeDisplayAmountChanged();
    }
}