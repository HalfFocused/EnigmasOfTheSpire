using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Relics.displaced;

public class Raincoat : TheDisplacedRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    public bool _isActivating;
    public int _etherealCardsPlayed;
    
    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? EtherealCardsPlayed % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(5),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.ForEnergy(this)
    ];

    public bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    [SavedProperty]
    public int EtherealCardsPlayed
    {
        get => _etherealCardsPlayed;
        set
        {
            AssertMutable();
            _etherealCardsPlayed = value;
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            int intValue = DynamicVars.Cards.IntValue; 
            Status = EtherealCardsPlayed % intValue == intValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }
        InvokeDisplayAmountChanged();
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !cardPlay.Card.Keywords.Contains(CardKeyword.Ethereal))
            return;
        EtherealCardsPlayed++;
        int intValue = DynamicVars.Cards.IntValue;
        if (!CombatManager.Instance.IsInProgress || EtherealCardsPlayed % intValue != 0)
            return;
        TaskHelper.RunSafely(DoActivateVisuals());
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    public async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}