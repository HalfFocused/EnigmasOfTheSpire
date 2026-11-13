#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.renegade.token;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics.renegade;

public class Watercolors : TheRenegadeRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;
    
    //public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<PerfectedCore>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [

    ];

    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || cardSource.Owner != Owner)
            return 1M;

        int attacksBeganPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count(e => e.HappenedThisTurn(Owner.Creature.CombatState) && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Player == Owner) + (cardSource.Pile?.Type != PileType.Play ? 1 : 0);
        
        return attacksBeganPlayedThisTurn switch
        {
            1 => 0,
            2 => 3,
            _ => 1
        };
    }
}