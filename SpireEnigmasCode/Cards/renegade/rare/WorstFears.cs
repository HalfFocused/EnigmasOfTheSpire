#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.other;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.renegade.rare;

public class WorstFears() : SpireEnigmasCard.RenegadeCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Paranoia>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
        CardModel? drawPileCard = PileType.Draw.GetPile(Owner).Cards.ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        
        CardModel? handCard = PileType.Hand.GetPile(Owner).Cards.ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        
        CardModel? discardPileCard = PileType.Discard.GetPile(Owner).Cards.ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();

        List<CardTransformation> transformations = new();
        
        if (drawPileCard is not null)
        {
            transformations.Add(new (drawPileCard, CombatState.CreateCard<Paranoia>(Owner)));
        }
        
        if (handCard is not null)
        {
            transformations.Add(new (handCard, CombatState.CreateCard<Paranoia>(Owner)));
        }
        
        if (discardPileCard is not null)
        {
            transformations.Add(new (discardPileCard, CombatState.CreateCard<Paranoia>(Owner)));
        }

        await CardCmd.Transform(transformations, Owner.RunState.Rng.CombatCardGeneration, CardPreviewStyle.GridLayout);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
    }
}