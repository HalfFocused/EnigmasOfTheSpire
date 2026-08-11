using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class WrenchStrike() : SpireEnigmasCard.SavantCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6M, ValueProp.Move),
        new RepeatVar(3)
    ];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        GadgetHoverTip()
    ];
    
    protected override bool ShouldGlowGoldInternal => PlayedGadgetThisTurn(this);
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitCount(PlayedGadgetThisTurn(this) ? DynamicVars.Repeat.IntValue : 1).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }

    public static bool PlayedGadgetThisTurn(CardModel card)
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any(
            e => e.CardPlay.Card is Gadget && e.RoundNumber == card.CombatState.RoundNumber
        );
    }
}