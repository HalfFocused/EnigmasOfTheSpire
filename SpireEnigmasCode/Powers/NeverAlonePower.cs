#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class NeverAlonePower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    
    protected override object InitInternalData() => new Data();
    
    public class Data
    {
        public readonly List<Tuple<Creature, decimal>> doomConversion = new ();
    }

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
    public override Decimal ModifyHpLostAfterOstyLate(
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target.Player is null || dealer != Owner) return amount;
        GetInternalData<Data>().doomConversion.Add(new Tuple<Creature, decimal>(target, amount));


        return 0;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        foreach (var keyPair in GetInternalData<Data>().doomConversion)
        {
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), keyPair.Item1, keyPair.Item2, Owner, null, true);
        }
        GetInternalData<Data>().doomConversion.Clear();
    }
    */
    
}