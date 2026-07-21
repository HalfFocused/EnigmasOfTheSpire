using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class RarityHelper
{
    
    /*
     * Modified Rarity "Buckets"
     *
     * The primary goal is for them to be identified by banner color
     * Common: Basic, Common, Token
     * Uncommon: Uncommon
     * Rare: Rare
     * Ancient: Ancient
     * Event: Event
     * Status: Status
     * Curse: Curse
     * Quest: Quest
     */

    public static CardRarity GetModifiedRarity(CardModel card)
    {
        if (card.Rarity is CardRarity.Basic or CardRarity.Common or CardRarity.Token)
        {
            return CardRarity.Common;
        }
        return card.Rarity;
    }

    public static int UniqueRaritiesInHandExcludingCard(CardModel card)
    {
        return PileType.Hand.GetPile(card.Owner).Cards.Where(c => c != card).DistinctBy(GetModifiedRarity).Count();
    }
}