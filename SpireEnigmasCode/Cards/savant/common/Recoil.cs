using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.common;

public class Recoil() : SpireEnigmasCard.SavantCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await Cmd.CustomScaledWait(0.1f, 0.35f);
        CardModel? cardToDiscard = Owner.RunState.Rng.CombatCardSelection.NextItem(PileType.Hand.GetPile(Owner).Cards);
        if (cardToDiscard == null)
            return;
        await CardCmd.DiscardAndDraw(choiceContext, [cardToDiscard], DynamicVars.Cards.IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}