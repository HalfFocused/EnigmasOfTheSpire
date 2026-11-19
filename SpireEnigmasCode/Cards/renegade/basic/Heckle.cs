#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.basic;

public class Heckle() : SpireEnigmasCard.RenegadeCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7M, ValueProp.Move),
        new ("Provoke", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        //HoverTipFactory.Static(EnigmaHoverTips.Provoke, DynamicVars["Provoke"])
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await AdversaryCmd.TakeImmediateTurns(Owner, DynamicVars["Provoke"].IntValue);
    }
    
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3M);
}