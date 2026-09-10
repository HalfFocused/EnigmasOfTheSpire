using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class WritersBlockPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        StoryHoverTip(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        int blockGain = Amount * (StoryManager.PlayerMaxStoryLength(Owner.Player) -
                        StoryManager.PlayerCurrentStoryLength(Owner.Player));
        
        if (blockGain != 0)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, blockGain, ValueProp.Unpowered, null, true);
        }
    }
}