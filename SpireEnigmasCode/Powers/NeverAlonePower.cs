#region

using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class NeverAlonePower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override bool ShouldPlayVfx => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature != Owner;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    /*
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer == Owner && target.Monster is not null ? 2 : 1;
    }
    */
}