using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using SpireEnigmas.SpireEnigmasCode.Cards.chronicle.other;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.ancient;

public class Epitaph() : SpireEnigmasCard.ChronicleCard(1,
    CardType.Power, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("ShortStoryLength", 3),
        new ("LongStoryLength", 7)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        StoryHoverTip()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await StoryManager.EndStory(Owner, CombatState);
        
        CardModel shortChoice = CombatState.CreateCard(ModelDb.Card<StoryChoice>(), Owner);
        CardModel longChoice = CombatState.CreateCard(ModelDb.Card<StoryChoice>(), Owner);
        shortChoice.DynamicVars["IfChosenLength"].BaseValue = DynamicVars["ShortStoryLength"].BaseValue;
        longChoice.DynamicVars["IfChosenLength"].BaseValue = DynamicVars["LongStoryLength"].BaseValue;
        
        CardModel? choice = await CardSelectCmd.FromChooseACardScreen(choiceContext, [shortChoice, longChoice], Owner);
        if (choice == null)
            return;

        await StoryManager.EndStory(Owner, CombatState);
        await StoryManager.SetStoryLength(Owner, CombatState, choice.DynamicVars["IfChosenLength"].IntValue);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}