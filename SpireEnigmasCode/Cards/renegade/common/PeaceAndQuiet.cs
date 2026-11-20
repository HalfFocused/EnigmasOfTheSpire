#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.common;

public class PeaceAndQuiet() : SpireEnigmasCard.RenegadeCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => EnigmasHelper.DebuffsOnCreature(Owner.Creature) > 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6M, ValueProp.Move),
        new BlockVar("BonusBlock", 8M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        //HoverTipFactory.Static(EnigmaHoverTips.Unwell)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (EnigmasHelper.DebuffsOnCreature(Owner.Creature) > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars["BonusBlock"], play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1M);
        DynamicVars["BonusBlock"].UpgradeValueBy(2);
    }
}