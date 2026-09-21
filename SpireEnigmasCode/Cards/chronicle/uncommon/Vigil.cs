using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;

public class Vigil() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VigilPower>(2), //duration in Stories
        new EnergyVar(1) //energy per turn
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        PowerModel? vigil = await PowerCmd.Apply<VigilPower>(choiceContext, Owner.Creature, DynamicVars.Power<VigilPower>().BaseValue, Owner.Creature, this);
        if(vigil is not null) vigil.DynamicVars.Energy.BaseValue = DynamicVars.Energy.BaseValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<VigilPower>().UpgradeValueBy(1);
    }
}