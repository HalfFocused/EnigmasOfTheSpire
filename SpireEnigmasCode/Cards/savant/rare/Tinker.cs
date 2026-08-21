using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class Tinker() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> cards = await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        CardModel? card1 = cards.ElementAtOrDefault(0);
        CardModel? card2 = cards.ElementAtOrDefault(1);

        if (card1 is not null && 
            card2 is not null && 
            !EnigmasHelper.DoesCardHaveEnergyCostX(card1) && 
            !EnigmasHelper.DoesCardHaveEnergyCostX(card2) &&
            card1.EnergyCost._base >= 0 &&
            card2.EnergyCost._base >= 0)
        {
            int card1Cost = card1.EnergyCost._base;
            int card2Cost = card2.EnergyCost._base;
            
            card1.EnergyCost.SetThisCombat(card2Cost);
            card2.EnergyCost.SetThisCombat(card1Cost);
        }
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}