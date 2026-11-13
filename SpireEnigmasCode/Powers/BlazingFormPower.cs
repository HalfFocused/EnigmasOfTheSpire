#region

using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class BlazingFormPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        return hitCount <= 1 || !(attack.ModelSource is CardModel modelSource) || modelSource.Type != CardType.Attack || modelSource.Owner.Creature != Owner ? hitCount : hitCount + Amount;
    }
}