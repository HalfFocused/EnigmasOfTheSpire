#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SpireEnigmas.SpireEnigmasCode.Commands;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class BarrierGenerator() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Transform),
        HoverTipFactory.FromCard<UltimateDefend>(IsUpgraded)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel ultimateDefend = CombatState.CreateCard<UltimateDefend>(Owner);
        if(IsUpgraded)
        {
            CardCmd.Upgrade(ultimateDefend);
        }
        await EnigmaCmd.ChooseAndTransformInto(choiceContext, Owner, ultimateDefend);
    }
    
    //protected override void OnUpgrade(){}
}