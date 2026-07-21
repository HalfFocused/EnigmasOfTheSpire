using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class Apostasy() : SpireEnigmasCard.SacrificeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5M, ValueProp.Move),
        new RepeatVar(2) //the amount of extra times block is gained if the player has a rare card in hand
    ];
    
    private bool HasRareInHand => PileType.Hand.GetPile(Owner).Cards.Any(c => RarityHelper.GetModifiedRarity(c) is CardRarity.Rare);
    
    protected override bool ShouldGlowGoldInternal => HasRareInHand;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (HasRareInHand)
        {
            for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
    }
}