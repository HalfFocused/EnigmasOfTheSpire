#region

using System.Reflection;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmasHelper
{
    public static Creature? GetChirpFromPlayer(Player? player)
    {
        return player?.PlayerCombatState?.GetPet<Chirp>();
    }
    
    public static Creature? GetAdversaryFromPlayer(Player? player)
    {
        return player?.PlayerCombatState?.GetPet<Adversary>();
    }
  
    public static bool IsPlayerChirpAlive(Player player)
    {
        Creature? chirp = GetChirpFromPlayer(player);
        return chirp != null && chirp.IsAlive;
    }
    
    public static bool IsPlayerAdversaryAlive(Player player)
    {
        Creature? adversary = GetAdversaryFromPlayer(player);
        return adversary != null && adversary.IsAlive;
    }

    public static List<Creature> GetAdversaryTargetsForPlayer(Player player, bool consideringImmunity = true)
    {
        List<Creature> adversaryTargets = player.Creature.CombatState.HittableEnemies.ToList();
        if (!consideringImmunity || !(player.HasPower<DodgePower>() || player.HasPower<IDontFearYouPower>()))
        {
            adversaryTargets.Add(player.Creature);
        }

        return adversaryTargets;
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

    public static int DebuffsOnCreature(Creature? creature)
    {
        return creature is null ? 0 : creature.Powers.Count(p => p.TypeForCurrentAmount == PowerType.Debuff && p is not ITemporaryPower);
    }
}