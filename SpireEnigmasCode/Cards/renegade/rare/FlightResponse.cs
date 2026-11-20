#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.rare;

public class FlightResponse() : SpireEnigmasCard.RenegadeCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FlightResponsePower>(10)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Paranoia>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<FlightResponsePower>(choiceContext, Owner.Creature, DynamicVars.Power<FlightResponsePower>().BaseValue, Owner.Creature, this);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Paranoia>(Owner), PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<FlightResponsePower>().UpgradeValueBy(2);
    }
}