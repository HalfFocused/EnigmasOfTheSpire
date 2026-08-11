using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireEnigmas.SpireEnigmasCode.Monsters;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class ChirpHelper
{
    public static Creature? GetChirpFromPlayer(Player? player)
    {
        return player?.PlayerCombatState?.GetPet<Chirp>();
    }
  
    public static bool IsPlayerChirpAlive(Player player)
    {
        Creature? chirp = GetChirpFromPlayer(player);
        return chirp != null && chirp.IsAlive;
    }
}