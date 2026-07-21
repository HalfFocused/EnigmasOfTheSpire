using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class AbandonPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation cardLocation)
    {
        if (card.Owner.Creature != Owner || card.Type is not (CardType.Attack or CardType.Skill))
            return new CardLocation(Owner.Player, cardLocation.pileType, cardLocation.position);
        Flash();
        PowerCmd.TickDownDuration(this);
        return new CardLocation(Owner.Player, PileType.Exhaust, CardPilePosition.Top);
    }
}