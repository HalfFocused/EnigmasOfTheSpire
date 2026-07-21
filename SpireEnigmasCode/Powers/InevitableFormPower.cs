using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class InevitableFormPower : SpireEnigmaPower
{
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;
  protected override object InitInternalData() => new Data();

  public override Task BeforePowerAmountChanged(
    PowerModel power,
    Decimal amount,
    Creature target,
    Creature? applier,
    CardModel? cardSource)
  {
    if (power != this)
      return Task.CompletedTask;
    HideTemporaryZeroCostVisual();
    return Task.CompletedTask;
  }

  public override Task BeforeApplied(
    Creature target,
    Decimal amount,
    Creature? applier,
    CardModel? cardSource)
  {
    HideTemporaryZeroCostVisual();
    return Task.CompletedTask;
  }

  public override bool TryModifyEnergyCostInCombatLate(
    CardModel card,
    Decimal originalCost,
    out Decimal modifiedCost)
  {
    modifiedCost = originalCost;
    if (ShouldSkip(card))
      return false;
    modifiedCost = 0M;
    return true;
  }

  public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card.Owner.Creature == Owner && cardPlay != null && !cardPlay.IsAutoPlay && cardPlay.IsLastInSeries && cardPlay.Card.Type is not CardType.Power)
      ++GetInternalData<Data>().cardsPlayedThisTurn;
    return Task.CompletedTask;
  }

  public override Task BeforeSideTurnStart(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IReadOnlyList<Creature> participants,
    ICombatState combatState)
  {
    if (side == Owner.Side)
      GetInternalData<Data>().cardsPlayedThisTurn = 0;
    return Task.CompletedTask;
  }

  public bool ShouldSkip(CardModel card)
  {
    if (card.Owner.Creature != Owner) return true;
    if (card.Type is CardType.Power) return true;
    if(GetInternalData<Data>().cardsPlayedThisTurn >= Amount) return true;
    return card.Pile.Type is not (PileType.Hand or PileType.Play);
  }
  
  public override CardLocation ModifyCardPlayResultLocation(
    CardModel card,
    bool isAutoPlay,
    ResourceInfo resources,
    CardLocation cardLocation)
  {
    if (ShouldSkip(card))
      return new CardLocation(cardLocation.player, cardLocation.pileType, cardLocation.position);
    return new CardLocation(Owner.Player, PileType.Exhaust, cardLocation.position);
  }

  public void HideTemporaryZeroCostVisual()
  {
    GetInternalData<Data>().cardsPlayedThisTurn = 999999999;
  }

  public class Data
  {
    public int cardsPlayedThisTurn;
  }
}