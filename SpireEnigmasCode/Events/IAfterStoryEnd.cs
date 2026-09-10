using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IAfterStoryEnd
{
    /*
     * Called whenever the story is ended, whether naturally or by force
     * This hook is sent out just before the story would be cleared (if the ending is a forceful one)
     *
     * There is no way for this hook to differentiate between a natural and forced story ending.
     * Which is probably for the best as they should be functionally identical to the player.
     */
    Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player, List<CardModel> storyState);
}