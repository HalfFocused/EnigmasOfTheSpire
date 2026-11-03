#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics.sacrifice;

public class Lifeblood : TheSacrificeRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<Kinderhart>();


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(9)
    ];
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is RestSiteRoom))
            return;
        Flash();
        decimal healAmount = DynamicVars.Heal.BaseValue;
        await CreatureCmd.Heal(Owner.Creature, healAmount);
        if (!LocalContext.IsMe(Owner))
            return;
        PlayerFullscreenHealVfx.Play(Owner, healAmount, NRestSiteRoom.Instance);
    }
}