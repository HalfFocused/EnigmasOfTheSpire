using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.ancient;

public class MorningsLight() : SpireEnigmasCard.ChronicleCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self),
    IShouldRenderStory

{
    protected override HashSet<CardTag> CanonicalTags => [
    ];
    
    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => IsInChapter(3, StoryManager.CardInChapterTimings.BeforePlay);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new EnergyVar(1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (IsInChapter(3, StoryManager.CardInChapterTimings.Resolution))
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
    
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4M);
}