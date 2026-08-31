using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.common;

public class Beam() : SpireEnigmasCard.SavantCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [
        //CardTag.Strike
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        EnigmaEnums.Command
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip()
    ];
    
    protected override bool ShouldGlowRedInternal => EnigmasHelper.GetChirpFromPlayer(Owner) == null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChirpDamageVar(6M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (GetChirp is null) return;
        await DamageCmd.Attack(DynamicVars["ChirpDamage"].BaseValue).FromChirp(GetChirp, this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    protected override void OnUpgrade() => DynamicVars["ChirpDamage"].UpgradeValueBy(3M);
}