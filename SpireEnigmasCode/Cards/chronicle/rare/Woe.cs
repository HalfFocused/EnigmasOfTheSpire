using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.rare;

public class Woe() : SpireEnigmasCard.ChronicleCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new EnergyVar(2),
        new PowerVar<DisintegrationPower>(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DisintegrationPower>(),
        HoverTipFactory.ForEnergy(this)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<DisintegrationPower>(choiceContext, Owner.Creature, DynamicVars["InkPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}