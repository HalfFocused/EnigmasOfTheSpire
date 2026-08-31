using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;

public class Motif() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self),
    IAfterStoryEnd,
    IShouldRenderStory
{
    private bool PlayedDuringStory = false;
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        PlayedDuringStory = true;
    }
    
    

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }

    public async Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player)
    {
        PileType? pileType = Pile?.Type;
        if (PlayedDuringStory && pileType is not null && pileType != PileType.Hand)
        {
            await CardPileCmd.Add(this, PileType.Hand);
        }
        PlayedDuringStory = false;
    }
}