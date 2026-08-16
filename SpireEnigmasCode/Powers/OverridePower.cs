using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class OverridePower : SpireEnigmaPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
  [
      HoverTipFactory.FromKeyword(EnigmaEnums.Command)
  ];

  public override bool TryModifyEnergyCostInCombatLate(
    CardModel card,
    Decimal originalCost,
    out Decimal modifiedCost)
  {
    modifiedCost = originalCost;
    if (card.Owner.Creature != this.Owner || !card.Keywords.Contains(EnigmaEnums.Command))
      return false;
    PileType? type = card.Pile?.Type;
    bool flag;
    if (type.HasValue)
    {
      switch (type.GetValueOrDefault())
      {
        case PileType.Hand:
        case PileType.Play:
          flag = true;
          goto label_6;
      }
    }
    flag = false;
label_6:
    if (!flag)
      return false;
    modifiedCost = 0M;
    return true;
  }

  public override async Task BeforeCardPlayed(CardPlay cardPlay)
  {
    if (cardPlay.Card.Owner.Creature != Owner || !cardPlay.Card.Keywords.Contains(EnigmaEnums.Command))
      return;
    PileType? type = cardPlay.Card.Pile?.Type;
    bool flag;
    if (type.HasValue)
    {
      switch (type.GetValueOrDefault())
      {
        case PileType.Hand:
        case PileType.Play:
          flag = true;
          goto label_6;
      }
    }
    flag = false;
label_6:
    if (!flag)
      return;
    await PowerCmd.Decrement(this);
  }
}
