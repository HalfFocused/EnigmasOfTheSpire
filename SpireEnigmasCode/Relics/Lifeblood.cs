using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireEnigmas.SpireEnigmasCode.Relics;

public class Lifeblood : TheSacrificeRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

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