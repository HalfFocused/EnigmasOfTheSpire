using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.ancient;

public class DevastationRains() : SpireEnigmasCard.SavantCard(1, CardType.Attack, CardRarity.Ancient, TargetType.RandomEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [
        //CardTag.Strike
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        EnigmaKeywords.Command
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip()
    ];
    
    protected override bool ShouldGlowRedInternal => ChirpCmd.GetChirpFromPlayer(Owner) == null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChirpDamageVar(11M, ValueProp.Move),
        new RepeatVar(4)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars["ChirpDamage"].BaseValue).FromChirp(GetChirp, this, play).WithHitCount(DynamicVars.Repeat.IntValue).TargetingRandomOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    protected override void OnUpgrade() => DynamicVars["ChirpDamage"].UpgradeValueBy(5M);
}