using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;

public class Hone() : SpireEnigmasCard.ChronicleCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IOnRefinementChanged
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1).AndRefinesBy(2)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        EnigmaKeywords.Refine
    ];
    
    protected override bool ShouldGlowGoldInternal => IsRefined;
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("IsRefined", IsRefined);
        description.Add("EnergyRefinement", DynamicVars.Energy.RefinementAmount());
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeRefinementBy(1);
    }

    public void OnRefinementChanged(bool newRefinement)
    {
        if (newRefinement)
        {
            AddKeyword(CardKeyword.Exhaust);
        }
        else
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}