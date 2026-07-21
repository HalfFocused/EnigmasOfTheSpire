using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.common;

public class Endure() : SpireEnigmasCard.SacrificeCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    
    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation oldLocation = base.GetResultLocationForCardPlay();
        return oldLocation.pileType == PileType.Discard && oldLocation.player == Owner ? new CardLocation(Owner, PileType.Draw, CardPilePosition.Top) : new CardLocation(oldLocation.player, oldLocation.pileType, oldLocation.position);
    }
    

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3M);
}