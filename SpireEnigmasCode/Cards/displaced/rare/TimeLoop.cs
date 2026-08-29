using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.token;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

public class TimeLoop() : SpireEnigmasCard.DisplacedCard(3,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardPlayFinishedEntry entry in CombatManager.Instance.History.Entries
                     .OfType<CardPlayFinishedEntry>()
                     .Where(e => e.Actor == Owner.Creature && e.HappenedLastPlayerTurn(Owner))
                     .ToList())
        {
            CardModel cardPlayed = entry.CardPlay.Card;

            if (cardPlayed is not TimeLoop)
            {
                await CardCmd.AutoPlay(choiceContext, cardPlayed.CreateDupe(Owner), null);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}