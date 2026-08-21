using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Relics.sacrifice;

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