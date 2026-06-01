using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

public class Apathy() : DisplacedCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(14M, ValueProp.Move),
        new ("StrengthLoss", 2)
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
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, hittableEnemy, -DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
        DynamicVars["StrengthLoss"].UpgradeValueBy(1);
    }
}