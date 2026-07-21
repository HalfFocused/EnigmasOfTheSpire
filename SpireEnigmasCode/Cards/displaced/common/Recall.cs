using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;

public class Recall() : SpireEnigmasCard.DisplacedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6M, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        FlashbackHoverTip()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        foreach (CardModel card in (await CardSelectCmd.FromSimpleGrid(choiceContext,
                     PileType.Discard.GetPile(Owner).Cards, Owner, prefs)))
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }
    

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}