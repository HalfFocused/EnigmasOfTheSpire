using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.ancient;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.basic;

public class SlashAndBurn() : SacrificeCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)//, ITranscendenceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move),
        new PowerVar<CharredPower>( 4M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<CharredPower>()
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await PowerCmd.Apply<CharredPower>(choiceContext, play.Target, DynamicVars["CharredPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3M);
        this.DynamicVars.Power<CharredPower>().UpgradeValueBy(2M);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<AllSeeing>();
    }
}