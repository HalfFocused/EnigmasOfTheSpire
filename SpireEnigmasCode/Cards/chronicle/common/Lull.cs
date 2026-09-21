using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;

public class Lull() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy), IRenderStory
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SusceptiblePower>(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StoryHoverTip(),
        HoverTipFactory.FromPower<SusceptiblePower>()
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<SusceptiblePower>(choiceContext, play.Target!, DynamicVars.Power<SusceptiblePower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<SusceptiblePower>().UpgradeValueBy(1);
    }
}