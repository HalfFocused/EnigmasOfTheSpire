using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.ancient;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.basic;

[Pool(typeof(TheDisplacedCardPool))]
public class Gaze() : SpireEnigmasCard.DisplacedCard(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy),
    ITranscendenceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8M, ValueProp.Move),
        new PowerVar<ForetoldPower>( 2M)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        //CardKeyword.Ethereal
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<ForetoldPower>()
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await PowerCmd.Apply<ForetoldPower>(choiceContext, play.Target, DynamicVars["ForetoldPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3M);
        this.DynamicVars.Power<ForetoldPower>().UpgradeValueBy(1M);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<AllSeeing>();
    }
}