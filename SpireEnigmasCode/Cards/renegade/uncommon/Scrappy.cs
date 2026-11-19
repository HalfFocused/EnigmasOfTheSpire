#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class Scrappy() : SpireEnigmasCard.RenegadeCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScrappyPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ScrappyPower>(choiceContext, Owner.Creature, DynamicVars.Power<ScrappyPower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}