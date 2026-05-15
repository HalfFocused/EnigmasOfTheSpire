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
using TheDisplaced.TheDisplacedCode.Character;

namespace TheDisplaced.TheDisplacedCode.Cards.common;

[Pool(typeof(TheDisplacedCardPool))]
public class FadeAway() : TheDisplacedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        FadeAway fadeAway = this;
        await CreatureCmd.GainBlock(fadeAway.Owner.Creature, fadeAway.DynamicVars.Block, play);
        CardPile hand = PileType.Hand.GetPile(Owner);
        List<CardModel> nonEtherealCards = hand.Cards.Where(c => !c.Keywords.Contains(CardKeyword.Ethereal)).ToList();
        CardModel cardToMakeEthereal = Owner.RunState.Rng.Shuffle.NextItem(nonEtherealCards);
        if (cardToMakeEthereal == null)
        {
            return;
        }
        CardCmd.ApplyKeyword(cardToMakeEthereal, CardKeyword.Ethereal);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(3M);
}