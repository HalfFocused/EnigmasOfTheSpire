using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class CracklingFormPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergySpent(CardModel card, int energySpent)
    {
        for (int i = 0; i < energySpent; i++)
        {
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                VfxCmd.PlayOnCreature(hittableEnemy, "vfx/vfx_attack_lightning");
            }
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner, null, null);
        }
    }

    public void FakeEnergySpend(int energySpent)
    {
        for (int i = 0; i < energySpent; i++)
        {
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                VfxCmd.PlayOnCreature(hittableEnemy, "vfx/vfx_attack_lightning");
            }
            CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner, null, null);
        }
    }
}