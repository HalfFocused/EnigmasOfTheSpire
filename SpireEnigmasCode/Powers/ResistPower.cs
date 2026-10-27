using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class ResistPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DisintegrationPower>()
    ];

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult _,
        ValueProp props,
        Creature? dealer,
        CardModel? __)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack())
            return;
        
        await CreatureCmd.Damage(choiceContext, dealer, Owner.GetPowerAmount<DisintegrationPower>() * Amount, ValueProp.Unpowered, Owner);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner.Side == side)
            return;
        await PowerCmd.Remove(this);
    }
}