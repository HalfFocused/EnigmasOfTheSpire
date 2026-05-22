using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using TheDisplaced.TheDisplacedCode.Cards;

namespace TheDisplaced.TheDisplacedCode.Cards.uncommon;

public class Endurance() : TheDisplacedCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(10, ValueProp.Move)
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
        //Not previewing the old endurance is actually better for clarity here.
        //CardCmd.Preview(this, 0.85f, CardPreviewStyle.MessyLayout);
        CardModel copy = CreateClone();
        copy.DynamicVars.Block.BaseValue = DynamicVars.Block.BaseValue - 2;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, card.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}