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

// Decompiled with JetBrains decompiler
// Type: MegaCrit.Sts2.Core.Models.Powers.VeilpiercerPower
// Assembly: sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7D2A9E0-F1AE-4213-B874-1504473AAEDB
// Assembly location: C:\Users\josep\RiderProjects\EnigmasOfTheSpire\.godot\mono\temp\obj\Debug\PublicizedAssemblies\sts2.747684A44202B1081D60DFFC9111EC87\sts2.dll
// XML documentation location: C:\Users\josep\RiderProjects\EnigmasOfTheSpire\.godot\mono\temp\obj\Debug\PublicizedAssemblies\sts2.747684A44202B1081D60DFFC9111EC87\sts2.xml


public sealed class MagnumOpusPower : SpireEnigmaPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
  [
      HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
  ];

  public override bool TryModifyEnergyCostInCombatLate(
    CardModel card,
    Decimal originalCost,
    out Decimal modifiedCost)
  {
    modifiedCost = originalCost;
    if (card.Owner.Creature != this.Owner || RarityHelper.GetModifiedRarity(card) is not CardRarity.Rare)
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
    if (cardPlay.Card.Owner.Creature != Owner || RarityHelper.GetModifiedRarity(cardPlay.Card) is not CardRarity.Rare)
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
