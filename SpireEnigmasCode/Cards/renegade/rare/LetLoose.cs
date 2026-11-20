#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.rare;

public class LetLoose() : SpireEnigmasCard.RenegadeCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("Provoke", 5)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<DodgePower>(),
        //HoverTipFactory.Static(EnigmaHoverTips.Provoke, DynamicVars["Provoke"])
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<DodgePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        
        if (GetAdversary is null) return;
        
        await AdversaryCmd.TakeImmediateTurns(Owner, DynamicVars["Provoke"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Provoke"].UpgradeValueBy(2);
    }
}