using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IAfterStoryStart
{
    Task AfterStoryStart(PlayerChoiceContext choiceContext, Player player);
}