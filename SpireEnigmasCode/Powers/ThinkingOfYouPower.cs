using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class ThinkingOfYouPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;


    protected override object InitInternalData() => new Data();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("Card")
    ];
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player == Owner.Player)
        {
            CardModel card = GetInternalData<Data>().selectedCard;
            await CardPileCmd.Add(card, PileType.Hand);
            await PowerCmd.Remove(this);
        }
    }
    
    public void SetSelectedCard(CardModel card)
    {
        CardCmd.ClearAffliction(card);
        GetInternalData<Data>().selectedCard = card;
        ((StringVar) DynamicVars["Card"]).StringValue = card.Title;
    }

    public class Data
    {
        public CardModel? selectedCard;
    }
}