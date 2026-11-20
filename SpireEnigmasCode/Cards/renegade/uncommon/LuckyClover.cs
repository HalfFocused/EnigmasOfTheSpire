#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class LuckyClover() : SpireEnigmasCard.RenegadeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DodgePower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<DodgePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<DodgePower>(choiceContext, Owner.Creature, DynamicVars.Power<DodgePower>().BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}