#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IAfterEnergyGained
{
    Task AfterEnergyGained(PlayerChoiceContext choiceContext, Player player, Decimal amount, bool duringTurn);
}