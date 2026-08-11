using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class RapidDeployment() : SpireEnigmasCard.SavantCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        //CardKeyword.Retain
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedGadgets").WithMultiplier((card, _) => PileType.Exhaust.GetPile(card.Owner).Cards.Count((c => c is Gadget)))
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        GadgetHoverTip()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> gadgets = PileType.Exhaust.GetPile(Owner).Cards.Where((c => c is Gadget)).ToList();
        foreach (CardModel gadget in gadgets)
        {
            await CardCmd.AutoPlay(choiceContext, gadget, null);
        }
    }
    
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}