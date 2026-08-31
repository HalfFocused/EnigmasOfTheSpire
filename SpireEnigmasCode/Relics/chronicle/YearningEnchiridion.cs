using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Relics.chronicle;

public class YearningEnchiridion : TheChronicleRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<BoundlessEnchiridion>();


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("Boost", 2)
    ];
    
    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return Owner.Creature != dealer || !props.IsPoweredAttack() || cardSource is null || !StoryManager.InChapter(cardSource, StoryManager.PlayerMaxStoryLength(Owner), StoryManager.CardInChapterTimings.BeforePlayAndResolution) ? 0M : DynamicVars["Boost"].BaseValue;
    }
    
    public override Decimal ModifyBlockAdditive(
        Creature target,
        Decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource is null || cardSource.Owner.Creature != Owner.Creature || !StoryManager.InChapter(cardSource, StoryManager.PlayerMaxStoryLength(Owner), StoryManager.CardInChapterTimings.BeforePlayAndResolution) ? 0M : DynamicVars["Boost"].BaseValue;
    }
}