using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class RecklessModule() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    private CardModel PreviewGadget()
    {
        Gadget previewGadget = (Gadget) ModelDb.Get<Gadget>().ToMutable();
        previewGadget.TakeAttributesFrom([], [CardKeyword.Ethereal], 1);
        return previewGadget;
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        InventHoverTip(),
        HoverTipFactory.FromCard(PreviewGadget())
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await EnigmaCmd.InventGadget(Owner, CombatState, [], [CardKeyword.Ethereal], 1);
    }
    
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}