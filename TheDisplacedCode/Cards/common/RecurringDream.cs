using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheDisplaced.TheDisplacedCode.Cards;

namespace TheDisplaced.TheDisplacedCode.Cards.common;

public class RecurringDream() : TheDisplacedCard(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(16M, ValueProp.Move)
    ];
    
    protected override bool ShouldGlowGoldInternal => this.PlayedThisTurnOrLast;


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4M);
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || !PlayedThisTurnOrLast)
            return Task.CompletedTask;
        ReduceCost();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!PlayedThisTurnOrLast)
            return Task.CompletedTask;
        ReduceCost();
        return Task.CompletedTask;
    }
    

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (!PlayedThisTurnOrLast)
            return Task.CompletedTask;
        ReduceCost();
        return Task.CompletedTask;
    }

    public void ReduceCost() => EnergyCost.SetThisTurn(0);
    
    public bool PlayedThisTurnOrLast
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any(
                e => e.CardPlay.Card is RecurringDream && (e.RoundNumber == CombatState.RoundNumber || e.RoundNumber == CombatState.RoundNumber - 1)
            );
        }
    }
}