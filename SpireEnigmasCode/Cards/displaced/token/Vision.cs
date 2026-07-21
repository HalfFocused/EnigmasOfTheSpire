using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.token;
[Pool(typeof(TokenCardPool))]
public class Vision() : SpireEnigmasCard.DisplacedCard(1,
    CardType.Attack, CardRarity.Token,
    TargetType.AnyEnemy)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(16M, ValueProp.Move),
        new CalculationBaseVar(1M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedCards").WithMultiplier((card, _) => card.Pile.Type is PileType.Hand or PileType.Play ? TrueSightAmount(card.Owner) : 0),
        new ("TurnsLeft", 3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, ((CalculatedVar) DynamicVars["CalculatedCards"]).Calculate(play.Target), Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4M);
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            DynamicVars["TurnsLeft"].BaseValue--;
            if (DynamicVars["TurnsLeft"].BaseValue == 0)
            {
                await CardCmd.Exhaust(choiceContext, this);
            }
        }
    }

    public static int TrueSightAmount(Player owner)
    {
        return owner.Creature.Powers.Count(p => p is TrueSightPower);
    }
}