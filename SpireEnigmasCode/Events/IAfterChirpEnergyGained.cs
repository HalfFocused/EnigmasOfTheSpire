using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IAfterChirpEnergyGained
{
    Task AfterChirpEnergyGained(PlayerChoiceContext choiceContext, Player player, Decimal amount);
}