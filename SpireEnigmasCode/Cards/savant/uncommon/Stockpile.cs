using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class Stockpile() : SpireEnigmasCard.SavantCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip()
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override bool HasEnergyCostX => true;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int energyGain = ResolveEnergyXValue() + (IsUpgraded ? 1 : 0);
        await ChirpCmd.GainEnergy(choiceContext, Owner, energyGain, this);
    }
}