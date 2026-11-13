#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class MiseryLovesCompanyPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0M || power is not (WeakPower or FrailPower or VulnerablePower) || power.Owner != Owner)
            return;
        
        Flash();
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply(choiceContext, (PowerModel) power.ClonePreservingMutability(), enemy, amount, Owner, null, true);
        }
    }
}