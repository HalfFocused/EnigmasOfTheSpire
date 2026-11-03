#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Commands;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class TemplatePower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is not Gadget || creator == null || creator.Creature != Owner)
            return;
        Flash();
        await EnigmaCmd.InventGadget(Owner.Player!, CombatState, [
            new BlockVar(Gadget.InventionBlockKey, Amount, ValueProp.Unpowered),
            new DamageVar(Amount, ValueProp.Unpowered)
        ]);
    }
}