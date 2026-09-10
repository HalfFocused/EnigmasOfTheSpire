using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.rare;

public class FourWoes() : SpireEnigmasCard.ChronicleCard(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy), IRenderStory
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FeeblePower>(2),
        new PowerVar<SusceptiblePower>(2),
        new PowerVar<InkPower>(2),
        new EnergyVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StoryHoverTip(),
        HoverTipFactory.FromPower<FeeblePower>(),
        HoverTipFactory.FromPower<SusceptiblePower>(),
        HoverTipFactory.FromPower<InkPower>(),
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
        await PowerCmd.Apply<FeeblePower>(choiceContext, play.Target!, DynamicVars["FeeblePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<SusceptiblePower>(choiceContext, play.Target!, DynamicVars["SusceptiblePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<InkPower>(choiceContext, Owner.Creature, DynamicVars["InkPower"].BaseValue, Owner.Creature, this);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}