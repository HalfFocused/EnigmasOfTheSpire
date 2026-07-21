using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class SpeedBlitzPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.ForEnergy(this)
    ];
    
    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || fromHandDraw || card.CombatState.CurrentSide != card.Owner.Creature.Side)
            return;
        
        if (CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>()
                .Count(e => !e.FromHandDraw && e.RoundNumber == card.CombatState.RoundNumber) == 1)
        {
            await PlayerCmd.GainEnergy(Amount, Owner.Player);
        }
    }
}