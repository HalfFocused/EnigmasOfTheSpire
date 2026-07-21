using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class BattleTempo() : SpireEnigmasCard.SacrificeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5M, ValueProp.Move),
        new PowerVar<BattleTempoPower>(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<BattleTempoPower>(choiceContext, Owner.Creature, DynamicVars["BattleTempoPower"].BaseValue, Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
        DynamicVars["BattleTempoPower"].UpgradeValueBy(1);
    }
}