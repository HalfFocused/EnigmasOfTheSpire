using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

public class PhaseShift() : SpireEnigmasCard.DisplacedCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Dazed>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int num = ResolveEnergyXValue();
        for (int i = 0; i < num; ++i)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat([CombatState.CreateCard<Dazed>(Owner), CombatState.CreateCard<Dazed>(Owner)], PileType.Draw, Owner, CardPilePosition.Random));
        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {

    }
}