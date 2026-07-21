using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class Contradoxy() : SpireEnigmasCard.SacrificeCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards.ToList())
        {
            CardModel rareCard = CardFactory.GetDistinctForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).Where( (c => RarityHelper.GetModifiedRarity(c) is CardRarity.Rare)), 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if(IsUpgraded) CardCmd.Upgrade(rareCard);
            await CardCmd.Transform(card, rareCard);
        }
    }
}