using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

public class MomentOfTruth() : SpireEnigmasCard.DisplacedCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    protected override bool ShouldGlowGoldInternal => BeenPlayedOnce();


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (BeenPlayedOnce())
        {
            foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards)
            {
                if (!card.EnergyCost.CostsX)
                    card.SetToFreeThisTurn();
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    private static bool BeenPlayedOnce()
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>()
            .Count(e => e.CardPlay.Card is MomentOfTruth) % 2 == 1;
    }
}