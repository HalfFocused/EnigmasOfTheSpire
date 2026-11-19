#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class Downtrodden() : SpireEnigmasCard.RenegadeCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),
        new PowerVar<DexterityPower>(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, -DynamicVars.Strength.BaseValue, Owner.Creature,
            this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars.Dexterity.BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Dexterity.UpgradeValueBy(1M);
    }
}