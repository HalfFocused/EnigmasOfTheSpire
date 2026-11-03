#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class ContingencyPlan() : SpireEnigmasCard.SavantCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ContingencyPlanPower>(8)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
       HoverTipFactory.ForEnergy(this),
       HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<ContingencyPlanPower>(choiceContext, Owner.Creature, DynamicVars["ContingencyPlanPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ContingencyPlanPower"].UpgradeValueBy(2);
    }
}