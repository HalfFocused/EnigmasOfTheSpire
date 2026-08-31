using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.other;

[Pool(typeof(TokenCardPool))]
public class StoryChoice() : SpireEnigmasCard(-1, CardType.Power, CardRarity.Ancient, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;

    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("IfChosenLength", 5)
    ];
}