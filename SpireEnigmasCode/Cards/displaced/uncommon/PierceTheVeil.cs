using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

public class PierceTheVeil() : DisplacedCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(2M),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => PileType.Exhaust.GetPile(card.Owner).Cards.Count)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(null), ValueProp.Move, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationExtra.UpgradeValueBy(1);
    }
}