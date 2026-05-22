using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheDisplaced.TheDisplacedCode.Cards.rare;
using TheDisplaced.TheDisplacedCode.Cards.token;

namespace TheDisplaced.TheDisplacedCode.Powers;

public class TheStarsAlignedPower : TheDisplacedPower
{
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override bool TryModifyEnergyCostInCombatLate(
    CardModel card,
    Decimal originalCost,
    out Decimal modifiedCost)
  {
    modifiedCost = originalCost;
    if (card is not Vision || CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any(e => e.CardPlay.Card == card)) return false;
    modifiedCost = 0M;
    return true;
  }
}