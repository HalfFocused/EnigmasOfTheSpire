using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

[Pool(typeof(TheDisplacedCardPool))]
public class Fathom() : SpireEnigmasCard.DisplacedCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardModel drawnCard in await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner))
        {
            CardCmd.ApplyKeyword(drawnCard, CardKeyword.Ethereal);
        }
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1);
}