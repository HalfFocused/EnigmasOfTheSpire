#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class AllocatePower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int nextTurnDraw = 0;
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        if (Owner.Player.PlayerCombatState.Energy != 0)
        {
            Flash();
            nextTurnDraw += Amount;
        }
    }
    
    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != Owner.Player? count : count + nextTurnDraw;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return;
        nextTurnDraw = 0;
    }
}