using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;

public class Malice : SpireEnigmasCard.ChronicleCard
{

    public Malice() : base(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        RetainTrackingFields.HasRetainUntilPlayed.Set(this, true);
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitCount(2).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}