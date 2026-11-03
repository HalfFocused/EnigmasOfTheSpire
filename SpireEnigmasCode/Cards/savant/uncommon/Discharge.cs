#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class Discharge() : SpireEnigmasCard.SavantCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20M, ValueProp.Move),
        new EnergyVar("InventionEnergy", 2)
    ];
    
    private CardModel PreviewGadget()
    {
        Gadget previewGadget = (Gadget) ModelDb.Get<Gadget>().ToMutable();
        previewGadget.TakeAttributesFrom([DynamicVars["InventionEnergy"]]);
        return previewGadget;
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(EnigmaHoverTips.Invent),
        HoverTipFactory.FromCard(PreviewGadget())
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        await EnigmaCmd.InventGadget(Owner, CombatState, [DynamicVars["InventionEnergy"]]);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6M);
        DynamicVars["InventionEnergy"].UpgradeValueBy(1M);
    }
}