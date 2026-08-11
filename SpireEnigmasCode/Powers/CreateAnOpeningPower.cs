using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public sealed class CreateAnOpeningPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ChirpHoverTip(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if(!participants.Contains(Owner.PetOwner.Creature))
            return;
        Creature? randomEnemy = Owner.PetOwner.RunState.Rng.CombatTargets.NextItem(CombatState.GetOpponentsOf(Owner.PetOwner.Creature));
        if (randomEnemy is null) return;
        Flash();
        await PowerCmd.Apply<VulnerablePower>(new BlockingPlayerChoiceContext(), randomEnemy, Amount, Owner, null);
    }
}