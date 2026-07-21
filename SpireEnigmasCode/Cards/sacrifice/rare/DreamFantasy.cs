using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class DreamFantasy() : SpireEnigmasCard.SacrificeCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int num = ResolveEnergyXValue();
        foreach (CardModel drawnCard in await CardPileCmd.Draw(choiceContext, num, Owner))
        {
            drawnCard.EnergyCost.SetThisTurn(0);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}