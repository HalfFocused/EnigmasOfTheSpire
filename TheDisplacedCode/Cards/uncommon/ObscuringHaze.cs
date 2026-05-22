using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheDisplaced.TheDisplacedCode.Cards;

namespace TheDisplaced.TheDisplacedCode.Cards.uncommon;

public class ObscuringHaze() : TheDisplacedCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    private bool HasEtherealInHand => PileType.Hand.GetPile(Owner).Cards.Any(c => c.Keywords.Contains(CardKeyword.Ethereal));
    protected override bool ShouldGlowGoldInternal => HasEtherealInHand;

    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8M, ValueProp.Move),
        new EnergyVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (HasEtherealInHand)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }
}