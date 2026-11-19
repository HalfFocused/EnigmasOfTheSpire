#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class Mania() : SpireEnigmasCard.RenegadeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Provoke", 1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await AdversaryCmd.TakeImmediateTurns(Owner, 1,
            IsUpgraded ? CombatState!.HittableEnemies.ToList() : [play.Target!]);
    }

    protected override void OnUpgrade()
    {
    }
}