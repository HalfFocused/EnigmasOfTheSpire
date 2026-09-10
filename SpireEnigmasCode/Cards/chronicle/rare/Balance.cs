using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.rare;

public class Balance() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self), IRenderStory
{
    private int EnergyGain => IsMutable && StoryManager.IsCardAllowedToEnterStory(CombatState, Owner, this) ? StoryManager.NextCardChapter(Owner) - 1 : 0;
    private int CardDraw => IsMutable && StoryManager.IsCardAllowedToEnterStory(CombatState, Owner, this) ? StoryManager.PlayerMaxStoryLength(Owner) - StoryManager.NextCardChapter(Owner) : 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StoryHoverTip(),
        HoverTipFactory.ForEnergy(this)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("EnergyGain", EnergyGain);
        description.Add("CardDraw", CardDraw);
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(EnergyGain, Owner);
        await CardPileCmd.Draw(choiceContext, CardDraw, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}