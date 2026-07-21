using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class StrikeTheHeart() : SpireEnigmasCard.SacrificeCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(10M),
        new ExtraDamageVar(15M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => PlayedTabooThisTurn(card) ? 1 : 0)
    ];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Taboo>()
    ];
    
    protected override bool ShouldGlowGoldInternal => PlayedTabooThisTurn(this);
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage.Calculate(play.Target)).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(5);
    }

    public static bool PlayedTabooThisTurn(CardModel card)
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any(
            e => e.CardPlay.Card is Taboo && e.RoundNumber == card.CombatState.RoundNumber
        );
    }
}