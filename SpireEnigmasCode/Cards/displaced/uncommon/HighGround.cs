using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

public class HighGround() : SpireEnigmasCard.DisplacedCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VulnerablePower>(1M),
        new CardsVar(1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        FlashbackHoverTip()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);

        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        foreach (CardModel card in (await CardSelectCmd.FromSimpleGrid(choiceContext,
                     PileType.Discard.GetPile(Owner).Cards.Where(c => (c.Type == CardType.Attack)).ToList(), Owner, prefs)))
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<VulnerablePower>().UpgradeValueBy(1M);
    }
}