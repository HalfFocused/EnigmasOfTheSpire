using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class Pioneer() : SpireEnigmasCard.SavantCard(5, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        //CardKeyword.Retain
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, MaxHandSizePatch.GetMaxHandSize(Owner, CardPile.MaxCardsInHand) - Owner.PlayerCombatState.Hand.Cards.Count, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue,  Owner);

    }
    
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}