using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class CracklingFormPower : SpireEnigmaPower, IAfterEnergyGained, IAfterChirpEnergyGained
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterEnergyGained(PlayerChoiceContext choiceContext, Player player, decimal amount, bool duringTurn)
    {
        Flash();
        if (player != Owner.Player || !duringTurn) return;
        
        await Trigger(choiceContext, amount);
    }
    
    public async Task AfterChirpEnergyGained(PlayerChoiceContext choiceContext, Player player, decimal amount)
    {
        Flash();
        if (player != Owner.Player) return;

        await Trigger(choiceContext, amount);
    }

    private async Task Trigger(PlayerChoiceContext choiceContext, decimal amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Creature? target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (target is null) break;
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_lightning");
            await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, Owner, null, null);
        }
    }
}