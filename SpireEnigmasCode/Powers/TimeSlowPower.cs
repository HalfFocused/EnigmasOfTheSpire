using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class TimeSlowPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override Decimal ModifyHpLostAfterOsty(
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return !CombatManager.Instance.IsInProgress || target != Owner ? amount : Math.Min(GetDamageCap(target, dealer), amount);
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override Decimal ModifyDamageCap(
        Creature? target,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return target != Owner ? Decimal.MaxValue : GetDamageCap(target, dealer);
    }

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;
        await PowerCmd.TickDownDuration(this);
    }

    private int GetDamageCap(Creature target, Creature? dealer)
    {
        Player player = dealer?.Player ?? dealer?.PetOwner;
        return player == null || !player.Relics.Any(r => r is TheBoot) || target == player.Creature ? 0 : 5;
    }
}