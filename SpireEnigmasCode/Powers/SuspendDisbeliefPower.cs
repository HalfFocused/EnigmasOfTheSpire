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
using SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class SuspendDisbeliefPower : SpireEnigmaPower,
    IAfterStoryEnd,
    IShouldRenderStory
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.None;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromCardWithCardHoverTips<SuspendDisbelief>();

    public async Task AfterStoryEnd(PlayerChoiceContext context, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        PlayerCmd.EndTurn(Owner.Player, false);
        await PowerCmd.Remove(this);
    }
}