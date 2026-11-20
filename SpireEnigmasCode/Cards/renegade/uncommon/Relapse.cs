#region

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Modifiers;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class Relapse() : SpireEnigmasCard.RenegadeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Paranoia>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int selectCount = Math.Min(DynamicVars.Cards.IntValue, CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count);
        if (selectCount <= 0)
            return;
        IReadOnlyList<CardPileAddResult> cardPileAddResultList = await CardPileCmd.Add(await CardSelectCmd.FromCombatPile(choiceContext, PileType.Discard.GetPile(Owner), Owner, new CardSelectorPrefs(SelectionScreenPrompt, selectCount)), PileType.Hand);

        foreach (CardModel card in cardPileAddResultList.Select(result => result.cardAdded))
        {
            CardModifier.AddModifier<TransformsIntoParanoiaThisTurnModifier>(card);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}