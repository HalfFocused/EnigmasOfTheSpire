#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Monsters;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class HauntedPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.None;
    
    protected override object InitInternalData() => new Data();
    
    public class Data
    {
        public readonly List<decimal> doomConversion = new ();
    }

    public override bool ShouldPlayVfx => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];

    
    public override Decimal ModifyHpLostAfterOstyLate(
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || (!props.IsPoweredAttack() && dealer?.Monster is not Adversary)) return amount;
        GetInternalData<Data>().doomConversion.Add(amount);

        return 0;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        foreach (var amount in GetInternalData<Data>().doomConversion)
        {
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), Owner, amount, Owner, null, true);
        }
        GetInternalData<Data>().doomConversion.Clear();
    }
}