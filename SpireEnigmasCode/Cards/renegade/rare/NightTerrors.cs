#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.DynamicVars;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.rare;

public class NightTerrors() : SpireEnigmasCard.RenegadeCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new AdversaryDamageVar(16, ValueProp.Move),
        new PowerVar<WeakPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await AdversaryCmd.AddAttackToIntent(Owner, DynamicVars["AdversaryDamage"].IntValue, 1);
        await AdversaryCmd.AddDebuffToIntent<WeakPower>(Owner, DynamicVars.Weak.IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AdversaryDamage"].UpgradeValueBy(6);
        DynamicVars.Weak.UpgradeValueBy(1);
    }
}