#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.rare;

public class Portrait() : SpireEnigmasCard.RenegadeCard(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PaintedPower>(16)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PaintedPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,  
        CardPlay play)
    {
        await PowerCmd.Apply<PaintedPower>(choiceContext, play.Target, DynamicVars.Power<PaintedPower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<PaintedPower>().UpgradeValueBy(4);
    }
}