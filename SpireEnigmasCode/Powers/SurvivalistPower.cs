#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.other;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class SurvivalistPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Paranoia>()
    ];

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!(card is Paranoia) || card.Owner != Owner.Player)
            return Task.CompletedTask;

        card.DynamicVars.Cards.BaseValue += Amount;
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        foreach (CardModel card in Owner.Player.PlayerCombatState.AllCards.Where(c => c is Paranoia))
            card.DynamicVars.Cards.BaseValue += Amount;
        return Task.CompletedTask;
    }
}