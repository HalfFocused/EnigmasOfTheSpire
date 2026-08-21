using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Monsters;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmasHelper
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

    public static bool DoesCardHaveEnergyCostX(CardModel card)
    {
        PropertyInfo propInfo = AccessTools.Property(typeof(CardModel), "HasEnergyCostX");
        return (bool) propInfo.GetValue(card);
    }
}