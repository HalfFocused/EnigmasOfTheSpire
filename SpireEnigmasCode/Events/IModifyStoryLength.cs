#region

using MegaCrit.Sts2.Core.Entities.Players;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Events;

public interface IModifyStoryLength
{
    int ModifyStoryLength(int amount, Player player);
}