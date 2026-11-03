#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.rare;

public class War() : SpireEnigmasCard.ChronicleCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self), IRenderStory
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(EnigmaHoverTips.Story)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardModel card in StoryManager.GetStoryState(Owner))
        {
            if (card.Type == CardType.Attack)
            {
                await CardCmd.AutoPlay(choiceContext, card.CreateDupe(Owner), null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}