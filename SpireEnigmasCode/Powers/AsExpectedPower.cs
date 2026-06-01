using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class AsExpectedPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<ForetoldPower>()
    ];

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner)
        {
            foreach (var result in command.Results.ToArray())
            {
                foreach (var damageResult in result)
                {
                    if (!damageResult.Receiver.HasPower<ForetoldPower>())
                    {
                        Flash();
                        await PowerCmd.Apply<ForetoldPower>(choiceContext, damageResult.Receiver, Amount, Owner, null);
                    }
                }

            }
        }

    }

}