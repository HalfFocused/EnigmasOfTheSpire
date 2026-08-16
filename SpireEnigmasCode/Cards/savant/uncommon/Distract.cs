using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class Distract() : SpireEnigmasCard.SavantCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(3),
        new PowerVar<VulnerablePower>(3)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        EnigmaEnums.Command,
        CardKeyword.Exhaust
    ];
    
    protected override bool ShouldGlowRedInternal => GetChirp == null;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if(GetChirp is null) return;
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars["WeakPower"].BaseValue, GetChirp, this);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars["VulnerablePower"].BaseValue, GetChirp, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["WeakPower"].UpgradeValueBy(1M);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1M);
    }
}