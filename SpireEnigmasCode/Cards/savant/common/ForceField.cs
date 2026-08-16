using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.common;

public class ForceField() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5M, ValueProp.Move),
        new DynamicVar(Gadget.InventionBlockKey, 7)
    ];
    
    private CardModel PreviewGadget()
    {
        Gadget previewGadget = (Gadget) ModelDb.Get<Gadget>().ToMutable();
        previewGadget.TakeAttributesFrom([DynamicVars[Gadget.InventionBlockKey]]);
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
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await EnigmaCmd.InventGadget(Owner, CombatState, [DynamicVars[Gadget.InventionBlockKey]]);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1M);
        DynamicVars[Gadget.InventionBlockKey].UpgradeValueBy(2M);
    }
}