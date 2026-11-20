#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.common;

public class NervousStrike() : SpireEnigmasCard.RenegadeCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Paranoia>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        TransformIntoAfterPlay = CombatState.CreateCard<Paranoia>(Owner);
    }
    
    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation locationForCardPlay = base.GetResultLocationForCardPlay();
        if (locationForCardPlay.pileType == PileType.Discard)
            locationForCardPlay.pileType = PileType.Hand;
        return locationForCardPlay;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}