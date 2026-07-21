using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class QuickThinking() : SpireEnigmasCard.SacrificeCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(3M),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => PileType.Hand.GetPile(card.Owner).Cards.Count(c => c != card))
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