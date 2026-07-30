using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

public class Manifest() : SpireEnigmasCard.DisplacedCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        var etherealCard = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Keywords.Contains(CardKeyword.Ethereal) && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList().StableShuffle(Owner.RunState.Rng.Shuffle).FirstOrDefault() ?? PileType.Draw.GetPile(Owner).Cards.Where(c => c.Keywords.Contains(CardKeyword.Ethereal)).ToList().StableShuffle(Owner.RunState.Rng.Shuffle).FirstOrDefault();
        if (etherealCard == null)
            return;
        await CardCmd.AutoPlay(choiceContext, etherealCard, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}