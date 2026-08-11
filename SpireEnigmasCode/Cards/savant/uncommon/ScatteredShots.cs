using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.ancient;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class ScatteredShots() : SpireEnigmasCard.SavantCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        EnigmaKeywords.Command
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip()
    ];
    
    protected override bool ShouldGlowRedInternal => ChirpHelper.GetChirpFromPlayer(Owner) == null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChirpDamageVar(6M, ValueProp.Move),
        new RepeatVar(6)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars["ChirpDamage"].BaseValue).FromChirp(GetChirp, this, play).WithHitCount(DynamicVars.Repeat.IntValue).TargetingRandomOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    protected override void OnUpgrade() => DynamicVars.Repeat.UpgradeValueBy(1);
}