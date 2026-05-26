using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheDisplaced.TheDisplacedCode.Powers;

public class AsExpectedPower : TheDisplacedPower
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