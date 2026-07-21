using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.token;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

public class Witness() : SpireEnigmasCard.DisplacedCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Visions", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Vision>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        for (int i = 0; i < DynamicVars["Visions"].IntValue; i++)
        {
            var vision = PileType.Draw.GetPile(Owner).Cards.Where(c => c is Vision).ToList().StableShuffle(Owner.RunState.Rng.Shuffle).FirstOrDefault() ?? PileType.Draw.GetPile(Owner).Cards.Where(c => c is Vision).ToList().StableShuffle(Owner.RunState.Rng.Shuffle).FirstOrDefault();
            if (vision != null)
            {
                await CardCmd.AutoPlay(choiceContext, vision, play.Target);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Visions"].UpgradeValueBy(1);
    }
}