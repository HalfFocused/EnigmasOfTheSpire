using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.token;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class TheStarsAlignedPower : SpireEnigmaPower
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