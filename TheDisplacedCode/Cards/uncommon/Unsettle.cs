using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheDisplaced.TheDisplacedCode.Cards;
using TheDisplaced.TheDisplacedCode.Powers;

namespace TheDisplaced.TheDisplacedCode.Cards.uncommon;

public class Unsettle() : TheDisplacedCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>(1),
        new PowerVar<ForetoldPower>(1),
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<ForetoldPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    
    protected override bool ShouldGlowGoldInternal => CardExhaustedThisTurn();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<ForetoldPower>(choiceContext, play.Target, DynamicVars["ForetoldPower"].BaseValue, Owner.Creature, this);
        if (CardExhaustedThisTurn())
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }

    private bool CardExhaustedThisTurn()
    {
        return CombatManager.Instance.History.Entries.OfType<CardExhaustedEntry>().Any(e =>
            e.RoundNumber == CombatState.RoundNumber);
    }

}