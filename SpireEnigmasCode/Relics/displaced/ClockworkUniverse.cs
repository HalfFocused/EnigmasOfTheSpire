#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics.displaced;

public class ClockworkUniverse() : TheDisplacedRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    public override bool ShouldTakeExtraTurn(Player player)
    {
        return player == Owner && Owner.PlayerCombatState.TurnNumber == 1;
    }
    
    public override Task AfterTakingExtraTurn(Player player)
    {
        if (player != Owner)
            return Task.CompletedTask;
        Flash();
        return Task.CompletedTask;
    }
}