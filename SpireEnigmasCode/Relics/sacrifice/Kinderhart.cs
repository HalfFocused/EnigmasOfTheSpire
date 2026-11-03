#region

using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics.sacrifice;

public class Kinderhart : TheSacrificeRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is RestSiteRoom))
            return;
        Flash();
        await HealRestSiteOption.ExecuteRestSiteHeal(Owner, false);
    }
}