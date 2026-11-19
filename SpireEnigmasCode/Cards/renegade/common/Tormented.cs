#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.common;

public class Tormented() : SpireEnigmasCard.RenegadeCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, ValueProp.Move),
        new PowerVar<VulnerablePower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        //EnigmaKeywords.Fixation
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
        
        await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}