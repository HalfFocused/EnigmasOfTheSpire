using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class FeeblePower : SpireEnigmaPower, IAfterStoryEnd
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("DamageDecrease", 0.75M)
    ];
    
    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (props.IsPoweredAttack() && dealer == Owner)
        {
            return DynamicVars["DamageDecrease"].BaseValue;;
        }

        return 1;
    }

    public async Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.TickDownDuration(this);
    }
}