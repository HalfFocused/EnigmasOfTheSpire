using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class Revelation() : SpireEnigmasCard.SacrificeCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(20M, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Purity>(IsUpgraded)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        CardModel purity = CombatState.CreateCard<Purity>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(purity);
        }
        await CardPileCmd.AddGeneratedCardsToCombat([purity], PileType.Hand, Owner);
    }
    
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5M);
}