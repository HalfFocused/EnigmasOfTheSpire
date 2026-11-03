#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class CornerOfYourEye() : SpireEnigmasCard.RenegadeCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (GetAdversary is null) return;

        await PowerCmd.Apply<StrengthPower>(choiceContext, GetAdversary, DynamicVars.Power<StrengthPower>().BaseValue, Owner.Creature,
            this);
    }
    
    protected override void OnUpgrade() => DynamicVars.Strength.UpgradeValueBy(1M);
}