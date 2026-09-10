using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IShouldPreventCardAddedToStory
{
    bool ShouldPreventCardAddedToStory(Player player, CardModel? card);
}