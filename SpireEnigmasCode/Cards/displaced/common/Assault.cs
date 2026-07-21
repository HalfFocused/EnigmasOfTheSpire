using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;

[Pool(typeof(TheDisplacedCardPool))]
public class Assault() : SpireEnigmasCard.DisplacedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move),
        new BlockVar(5M, ValueProp.Move)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ForgetHoverTip()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this)
            return;
        CardCmd.Preview(this, 0.85f, CardPreviewStyle.MessyLayout);
        await CreatureCmd.GainBlock(card.Owner.Creature, card.DynamicVars.Block, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars.Block.UpgradeValueBy(3M);
    }
}