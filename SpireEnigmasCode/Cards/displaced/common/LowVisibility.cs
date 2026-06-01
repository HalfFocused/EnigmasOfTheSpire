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

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;

[Pool(typeof(TheDisplacedCardPool))]
public class LowVisibility() : DisplacedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5M, ValueProp.Move),
        new CardsVar(3)
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
        await Cmd.Wait(0.5f);
        CardModel cardToExhaust = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (cardToExhaust == null)
            return;
        await CardCmd.Exhaust(choiceContext, cardToExhaust);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(3M);
}