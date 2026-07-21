using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class TwoVirtues() : SpireEnigmasCard.SacrificeCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        for(int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var commonCard = PileType.Draw.GetPile(Owner).Cards.Where((c => RarityHelper.GetModifiedRarity(c) is CardRarity.Common && !c.Keywords.Contains(CardKeyword.Unplayable))).ToList().StableShuffle(Owner.RunState.Rng.Shuffle).FirstOrDefault() ?? PileType.Draw.GetPile(Owner).Cards.Where(c => c.Keywords.Contains(CardKeyword.Ethereal)).ToList().StableShuffle(Owner.RunState.Rng.Shuffle).FirstOrDefault();
            if (commonCard == null)
                return;
            await CardCmd.AutoPlay(choiceContext, commonCard, null);
        }
    }

    protected override void OnUpgrade()
    {
        _titleLocString = new LocString("cards", this.Id.Entry + ".upgraded_title");
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}