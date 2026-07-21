using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class Wildfyre() : SpireEnigmasCard.SacrificeCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(16M, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Burn>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        
        int num = CardPile.MaxCardsInHand - CardPile.GetCards(Owner, PileType.Hand).Count();
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < num; ++index)
            cards.Add(CombatState.CreateCard<Burn>(Owner));
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4M);
    }
}