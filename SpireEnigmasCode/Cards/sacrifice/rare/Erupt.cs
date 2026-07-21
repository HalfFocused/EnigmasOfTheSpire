using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class Erupt() : SpireEnigmasCard.SacrificeCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CharredPower>(6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CharredPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<CharredPower>(choiceContext, play.Target, DynamicVars["CharredPower"].BaseValue, Owner.Creature, this);
    }
    
    protected override void OnUpgrade() => DynamicVars["CharredPower"].UpgradeValueBy(3);
}