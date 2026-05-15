using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheDisplaced.TheDisplacedCode.Cards;
using TheDisplaced.TheDisplacedCode.Cards.token;
using TheDisplaced.TheDisplacedCode.Character;

namespace TheDisplaced.TheDisplacedCode.Cards.common;

[Pool(typeof(TheDisplacedCardPool))]
public class LowVisibility() : TheDisplacedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6M, ValueProp.Move),
        new CardsVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        LowVisibility card = this;
        await CreatureCmd.GainBlock(card.Owner.Creature, card.DynamicVars.Block, play);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        CardPile pile = PileType.Hand.GetPile(Owner);
        CardModel cardToExhaust = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (cardToExhaust == null)
            return;
        await CardCmd.Exhaust(choiceContext, cardToExhaust);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(3M);
}