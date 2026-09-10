using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    public static int UniqueCardsInDeck(Player player, CardType? type)
    {
        return PileType.Deck.GetPile(player).Cards.Where(c => type == null || c.Type == type).DistinctBy(c => c.Id).Count();
    }
}