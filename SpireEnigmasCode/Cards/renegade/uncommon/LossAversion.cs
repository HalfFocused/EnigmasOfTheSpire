#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class LossAversion() : SpireEnigmasCard.RenegadeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    //public override bool GainsBlock => true; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(4)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        //EnigmaKeywords.Fixation
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromCard<Paranoia>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<LossAversionPower>(choiceContext, CombatState.HittableEnemies, -DynamicVars.Strength.BaseValue, Owner.Creature, this);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Paranoia>(Owner), PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(2);
    }
}