using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public class RadiantRevival() : SpireEnigmasCard.SacrificeCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new CalculationExtraVar(3),
        new CalculatedVar("CalculatedHeal").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => GetBurns(card.Owner).Count()))
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromCard<Burn>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {

        decimal healAmount = ((CalculatedVar)DynamicVars["CalculatedHeal"]).Calculate(play.Target);
        
        foreach (CardModel card in GetBurns(Owner).ToList())
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        await CreatureCmd.Heal(Owner.Creature, healAmount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationExtra.UpgradeValueBy(1);
    }
    
    public static IEnumerable<CardModel> GetBurns(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where(c => c is Burn && c.Pile.Type != PileType.Exhaust);
    }
}