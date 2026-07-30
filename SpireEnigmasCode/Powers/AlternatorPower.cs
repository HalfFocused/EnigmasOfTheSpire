using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Monsters;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class AlternatorPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ChirpHoverTip(),
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;
        
        Chirp? chirp = (Chirp) ChirpCmd.GetChirpFromPlayer(Owner.Player).Monster;
        
        if (chirp == null) return;
        if (chirp.GetEnergy() == 0)
        {
            await ChirpCmd.GainEnergy(choiceContext, Owner.Player, Amount, this);
        }
    }
}