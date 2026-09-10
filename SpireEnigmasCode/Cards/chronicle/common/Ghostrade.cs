using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;

public class Ghostrade() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy), IRenderStory
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FeeblePower>(1),
        new PowerVar<SusceptiblePower>(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StoryHoverTip(),
        HoverTipFactory.FromPower<FeeblePower>(),
        HoverTipFactory.FromPower<SusceptiblePower>()
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<FeeblePower>(choiceContext, play.Target!, DynamicVars["FeeblePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<SusceptiblePower>(choiceContext, play.Target!, DynamicVars["SusceptiblePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}