using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class BlockNextStoryPower : SpireEnigmaPower,
    IAfterStoryStart,
    IAfterStoryEnd,
    IRenderStory
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /*
     * Simple internal state where the power checks if it's seen a Story end or not.
     * This is to prevent when you gain this power as a result of playing a card into Chapter 1.
     * The story start hooks are dispatched immediately after, but it shouldn't count this Story as next Story.
     */
    private bool _seenStoryEnd = false;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        StoryHoverTip(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public async Task AfterStoryStart(PlayerChoiceContext context, Player player)
    {
        if (player != Owner.Player) return;

        if (_seenStoryEnd)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null, true);
            await PowerCmd.Remove(this);
        }
    }

    public async Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player, List<CardModel> storyState)
    {
        if (player != Owner.Player) return;

        _seenStoryEnd = true;
    }
}