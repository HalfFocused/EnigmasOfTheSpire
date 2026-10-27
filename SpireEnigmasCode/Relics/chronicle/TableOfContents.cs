using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Relics.chronicle;

public class TableOfContents : TheChronicleRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;
    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("Boost", 3)
    ];
    
    public override Task AfterObtained()
    {
        foreach (CardModel card in (IEnumerable<CardModel>) PileType.Deck.GetPile(Owner).Cards.ToList())
        {
            if (card.Rarity == CardRarity.Basic)
            {
                if (card.Tags.Contains(CardTag.Strike))
                {
                    card.AddKeyword(EnigmaKeywords.Refine);
                    foreach(DynamicVar dynamicVar in card.DynamicVars.Values)
                    {
                        if (dynamicVar is DamageVar)
                        {
                            dynamicVar.AndRefinesBy(DynamicVars["Boost"].IntValue);
                        }
                    }
                }
                
                if (card.Tags.Contains(CardTag.Defend))
                {
                    card.AddKeyword(EnigmaKeywords.Refine);
                    foreach(DynamicVar dynamicVar in card.DynamicVars.Values)
                    {
                        if (dynamicVar is BlockVar)
                        {
                            dynamicVar.AndRefinesBy(DynamicVars["Boost"].IntValue);
                        }
                    }
                }
            }
        }
        return Task.CompletedTask;
    }
}