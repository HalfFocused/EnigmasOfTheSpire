using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class BrainstormPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        if (Owner.Player.PlayerCombatState.Energy != 0)
        {
            Flash();
            for (int i = 0; i < Amount; ++i)
            {
                List<CardModel> list = PileType.Hand.GetPile(Owner.Player).Cards.Where(c => !c.Keywords.Contains(CardKeyword.Unplayable)).ToList();
                CardModel card = Owner.Player.RunState.Rng.Shuffle.NextItem(list);
                if (card != null)
                    await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }

        await PowerCmd.Remove(this);
    }
}