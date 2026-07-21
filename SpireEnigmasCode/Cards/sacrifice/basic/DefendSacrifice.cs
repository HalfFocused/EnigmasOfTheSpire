using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.basic;

public class DefendSacrifice() : SpireEnigmasCard.SacrificeCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>() { CardTag.Defend };
    }
    
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(3M);
}