using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.rare;

public class Multitudes() : SpireEnigmasCard.ChronicleCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StoryHoverTip()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Threshold", 10)
    ];
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("InRunButNotCombat", IsMutable && RunState is not null && CombatState is null);
        description.Add("UniqueCards", IsCanonical || RunState is null ? 0 : EnigmasHelper.UniqueCardsInDeck(Owner, null));
        description.Add("DrawPerTurn", IsCanonical || RunState is null ? 0 : EnigmasHelper.UniqueCardsInDeck(Owner, null) / DynamicVars["Threshold"].IntValue);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int draw = EnigmasHelper.UniqueCardsInDeck(Owner, null) / DynamicVars["Threshold"].IntValue;
        await PowerCmd.Apply<MultitudesPower>(choiceContext, Owner.Creature, draw, Owner.Creature, this);
    }
    

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}