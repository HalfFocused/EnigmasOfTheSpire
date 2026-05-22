using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheDisplaced.TheDisplacedCode.Cards.rare;

public class RecklessAbandon() : TheDisplacedCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(14M, ValueProp.Move),
        new CalculationBaseVar(1M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedHits").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Keywords.Contains(CardKeyword.Ethereal))))
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount((int) ((CalculatedVar) DynamicVars["CalculatedHits"]).Calculate(play.Target)).FromCard(this).TargetingRandomOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "dagger_throw.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}