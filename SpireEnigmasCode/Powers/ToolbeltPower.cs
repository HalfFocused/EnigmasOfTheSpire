using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.other;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class ToolbeltPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Scrap>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!(card is Scrap) || card.Owner != Owner.Player)
            return Task.CompletedTask;
        CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        foreach (CardModel card in Owner.Player.PlayerCombatState.AllCards.Where(c => c is Scrap))
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        return Task.CompletedTask;
    }
}