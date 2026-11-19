#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.uncommon;

public class BunkerUp() : SpireEnigmasCard.RenegadeCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(20, ValueProp.Move),
        new ("Provoke", 1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        //HoverTipFactory.Static(EnigmaHoverTips.Provoke, DynamicVars["Provoke"])
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await AdversaryCmd.TakeImmediateTurns(Owner, DynamicVars["Provoke"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}