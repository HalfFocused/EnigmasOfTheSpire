using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

public class Invoke() : DisplacedCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Dazed>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        CardModel card = (await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Draw.GetPile(Owner).Cards.OrderBy((c => c.Rarity)).ThenBy((Func<CardModel, ModelId>) (c => c.Id)).ToList(), Owner, prefs)).FirstOrDefault();
        if (card == null)
            return;
        await CardPileCmd.Add(card, PileType.Hand);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat([CombatState.CreateCard<Dazed>(Owner), CombatState.CreateCard<Dazed>(Owner)], PileType.Draw, Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}