using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.common;

public class RadialPulse() : SpireEnigmasCard.SavantCard(2, CardType.Attack, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChirpDamageVar(8M, ValueProp.Move),
        new PowerVar<VulnerablePower>(2)
    ];
    
    protected override bool ShouldGlowRedInternal => EnigmasHelper.GetChirpFromPlayer(Owner) == null;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        EnigmaEnums.Command
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if(GetChirp is null) return;
        await DamageCmd.Attack(DynamicVars["ChirpDamage"].BaseValue).FromChirp(GetChirp, this, play, AttackCommandExtensions.ChirpAttackVfxStyle.Aoe).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies, DynamicVars["VulnerablePower"].BaseValue, GetChirp, this);
    }
    
    protected override void OnUpgrade() => DynamicVars["ChirpDamage"].UpgradeValueBy(4);
}