using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class Freestyle() : SpireEnigmasCard.SacrificeCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FreestylePower>(5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<FreestylePower>(choiceContext, Owner.Creature, DynamicVars["FreestylePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FreestylePower"].UpgradeValueBy(3M);
    }
}