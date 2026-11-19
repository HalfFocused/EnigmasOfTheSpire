#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.token;

[Pool(typeof(TokenCardPool))]
public class SelfEmbodiment() : SpireEnigmasCard.RenegadeCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<DodgePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, DynamicVars.Power<IntangiblePower>().BaseValue, Owner.Creature,
            this);
        await PowerCmd.Apply<DodgePower>(choiceContext, Owner.Creature, 1, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}