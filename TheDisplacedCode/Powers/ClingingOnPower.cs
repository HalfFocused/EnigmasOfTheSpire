using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace TheDisplaced.TheDisplacedCode.Powers;

public class ClingingOnPower : TheDisplacedPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => new Data();

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if(CombatManager.Instance.History.Entries.OfType<CardExhaustedEntry>().All(e => (e.Card == card && e.RoundNumber == CombatState.RoundNumber) || e.RoundNumber != CombatState.RoundNumber)){


            if (causedByEthereal)
            {
                GetInternalData<Data>().queuedDraw += Amount;
            }
            else
            {
                Flash();
                await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
            }
        }
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (GetInternalData<Data>().queuedDraw > 0)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, GetInternalData<Data>().queuedDraw, Owner.Player);
            GetInternalData<Data>().queuedDraw = 0;
        }
    }
    
    public class Data
    {
        public int queuedDraw;
    }
}