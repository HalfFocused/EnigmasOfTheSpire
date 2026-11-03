#region

using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics.chronicle;

public class BoundlessEnchiridion : TheChronicleRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];
    
    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner != Owner || !StoryManager.InChapter(card, StoryManager.PlayerMaxStoryLength(Owner), StoryManager.CardInChapterTimings.BeforePlayAndResolution))
            return false;
        modifiedCost -= DynamicVars.Energy.BaseValue;
        return true;
    }
}