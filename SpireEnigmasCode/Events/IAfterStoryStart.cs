#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IAfterStoryStart
{
    Task AfterStoryStart(PlayerChoiceContext choiceContext, Player player);
}