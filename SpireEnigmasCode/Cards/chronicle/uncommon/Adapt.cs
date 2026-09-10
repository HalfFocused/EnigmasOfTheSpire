using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;

public class Adapt() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Threshold", 10),
        new BlockVar(8M, ValueProp.Move)
    ];
    
    protected override bool ShouldGlowGoldInternal => EnigmasHelper.UniqueCardsInDeck(Owner, CardType.Skill) >= DynamicVars["Threshold"].IntValue;

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("InRunButNotCombat", IsMutable && RunState is not null && CombatState is null);
        description.Add("UniqueCards", IsCanonical || RunState is null ? 0 : EnigmasHelper.UniqueCardsInDeck(Owner, CardType.Skill));
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StoryHoverTip()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (EnigmasHelper.UniqueCardsInDeck(Owner, CardType.Skill) >= DynamicVars["Threshold"].IntValue)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
    }
}