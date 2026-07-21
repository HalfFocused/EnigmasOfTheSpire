using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

public class Savagery() : SpireEnigmasCard.DisplacedCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private int _attacksToAdd = 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9M, ValueProp.Move),
        new RepeatVar(2)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal,
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ForgetHoverTip()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars.Repeat.IntValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_flying_slash").Execute(choiceContext);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card != this)
            return;
        CardCmd.Preview(this, 0.85f, CardPreviewStyle.MessyLayout);

        if (causedByEthereal)
        {
            _attacksToAdd++;
        }
        else
        {
            var attack = CardFactory.GetDistinctForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).Where(c => c.Type == CardType.Attack), 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (attack == null)
                return;
            await CardPileCmd.AddGeneratedCardToCombat(attack, PileType.Hand, Owner);
        }
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {

        if (side != CombatSide.Player)
        {
            _attacksToAdd = 0;
        }
        else
        {
            if (_attacksToAdd > 0)
            {
                for (int i = 0; i < _attacksToAdd; i++)
                {
                    var attack = CardFactory.GetDistinctForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).Where(c => c.Type == CardType.Attack), 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                    if (attack == null)
                        return;
                    await CardPileCmd.AddGeneratedCardToCombat(attack, PileType.Hand, Owner);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}