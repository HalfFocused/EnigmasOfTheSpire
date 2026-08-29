using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Relics.displaced;

public class WindUpButterfly : TheDisplacedRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    
    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != Owner || !CombatManager.Instance.History.Entries
            .OfType<CardExhaustedEntry>()
            .Any(e => e.Actor == Owner.Creature && e.HappenedLastPlayerTurn(Owner)) ? count : count + DynamicVars.Cards.BaseValue;
    }

    public override Task AfterModifyingHandDraw()
    {
        TaskHelper.RunSafely(DoActivateVisuals());
        return Task.CompletedTask;
    }

    public async Task DoActivateVisuals()
    {
        Flash();
        await Cmd.Wait(1f);
    }
}