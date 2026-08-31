using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;

public class BetterDays() : SpireEnigmasCard.ChronicleCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(11M, ValueProp.Move),
        new BlockVar("BlockNextStory", 5M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BlockVar dynamicVar = (BlockVar) DynamicVars["BlockNextStory"];
        Decimal blockNextStoryAmount = Hook.ModifyBlock(CombatState, Owner.Creature, dynamicVar.BaseValue, dynamicVar.Props, this, play, out IEnumerable<AbstractModel> _);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<BlockNextStoryPower>(choiceContext, Owner.Creature, blockNextStoryAmount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
        DynamicVars["BlockNextStory"].UpgradeValueBy(2M);
    }
}