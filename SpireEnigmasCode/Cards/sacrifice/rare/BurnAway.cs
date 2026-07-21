using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class BurnAway() : SpireEnigmasCard.SacrificeCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9M, ValueProp.Move),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => GetBurns(card.Owner).Count()))
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Burn>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int hits = (int) ((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hits).FromCard(this, play).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
        await CardPileCmd.AddGeneratedCardsToCombat([CombatState.CreateCard<Burn>(Owner)], PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
    
    public static IEnumerable<CardModel> GetBurns(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where(c => c is Burn && c.Pile.Type != PileType.Exhaust);
    }
}