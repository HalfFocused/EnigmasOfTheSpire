using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class ControlFlowPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(EnigmaEnums.Command)
    ];
    
    public override async Task BeforeSideTurnStart(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IReadOnlyList<Creature> participants,
            ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return;
        Flash();
        for (int i = 0; i < Amount; i++)
        {
            var command = CardFactory.GetDistinctForCombat(Owner.Player, Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint).Where(c => c.Keywords.Contains(EnigmaEnums.Command)), 1, Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (command == null)
                return;
            command.EnergyCost.SetThisTurn(0);
            await CardPileCmd.AddGeneratedCardToCombat(command, PileType.Hand, Owner.Player);
        }
    }
}