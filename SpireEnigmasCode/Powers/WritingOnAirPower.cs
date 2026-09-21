using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class WritingOnAirPower : SpireEnigmaPower, IRenderStory, IAfterStoryEnd
{
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.None;
  
  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
  [
    StoryHoverTip(),
    HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
  ];

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

  public bool ShouldSkip(CardModel card)
  {
    if (card.Owner.Creature != Owner) return true;
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


  public async Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player, List<CardModel> storyState)
  {
    if(player != Owner.Player) return;

    await PowerCmd.Remove(this);
  }
}