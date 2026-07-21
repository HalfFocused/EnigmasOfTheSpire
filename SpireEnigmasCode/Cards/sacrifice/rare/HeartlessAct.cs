using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class HeartlessAct() : SpireEnigmasCard.SacrificeCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("Taboos", 1),
        new PowerVar<HeartlessActPower>(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Taboo>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await Taboo.CreateInHand(Owner, DynamicVars["Taboos"].IntValue, CombatState);
        await PowerCmd.Apply<HeartlessActPower>(choiceContext, Owner.Creature, DynamicVars["HeartlessActPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HeartlessActPower"].UpgradeValueBy(1);
    }
}