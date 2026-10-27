using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;

public class ThinkItOver() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self),
    IRenderStory
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardModel drawnCard in await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner))
        {
            EnigmaCmd.GiveRetainUntilPlayed(drawnCard);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}