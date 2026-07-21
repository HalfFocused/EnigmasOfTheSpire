using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;

public class FutureSight() : SpireEnigmasCard.DisplacedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    private int _cardsToDraw = 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new CardsVar(1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        ForgetHoverTip()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this)
            return;
        
        CardCmd.Preview(this, 0.85f, CardPreviewStyle.MessyLayout);
        if (causedByEthereal)
        {
            _cardsToDraw += (int) DynamicVars.Cards.BaseValue;
        }
        else
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {

        if (side != CombatSide.Player)
        {
            _cardsToDraw = 0;
        }
        else
        {
            if (_cardsToDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, _cardsToDraw, Owner);
                _cardsToDraw = 0;
            }
        }
    }
    
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3M);
}