using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public abstract class CustomStoryTemporaryPowerModelWrapper<TModel, TPower> :
    CustomTemporaryPowerModelWrapper<TModel, TPower>, IRenderStory, IAfterStoryEnd
    where TModel : AbstractModel
    where TPower : PowerModel

{
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {}
    
    public async Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player, List<CardModel> storyState)
    {
        //TODO: Add smarter logic for debuffs with story durations. maybe track who applied them and not allow cross-player stacking? idk
        if (Applier != player.Creature)
            return;
        if (InternallyAppliedPower is CustomTemporaryPowerModel)
            await PowerCmd.Remove(this);
        else if (DynamicVars.Repeat.BaseValue > 0M)
        {
            DynamicVars.Repeat.UpgradeValueBy(-1M);
        }
        else
        {
            Flash();
            await ApplyPowerFunc(choiceContext, Owner, InvertInternalPowerAmount ? Amount : -Amount, Owner, null, true);
            await PowerCmd.Remove(this);
        }
    }
    
    public override LocString Description => new LocString("powers", Amount > 0 == !InvertInternalPowerAmount ? "SPIREENIGMAS-CUSTOM_STORY_TEMPORARY_POWER_MODEL.UP.description" : "SPIREENIGMAS-CUSTOM_STORY_TEMPORARY_POWER_MODEL.DOWN.description");


    protected override string SmartDescriptionLocKey => Amount > 0 != !InvertInternalPowerAmount ? "SPIREENIGMAS-CUSTOM_STORY_TEMPORARY_POWER_MODEL.DOWN.smartDescription" : "SPIREENIGMAS-CUSTOM_STORY_TEMPORARY_POWER_MODEL.UP.smartDescription";

}