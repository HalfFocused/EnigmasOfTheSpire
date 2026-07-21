using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class Flamethrower() : SpireEnigmasCard.SacrificeCard(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new RepeatVar(3)
    ];
    
    private CardModel? BurnInHand => PileType.Hand.GetPile(Owner).Cards.FirstOrDefault(c => c is Burn, null);

    protected override bool ShouldGlowGoldInternal => BurnInHand is not null;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel? burn = BurnInHand;
        int hits = 1;
        if (burn is not null)
        {
            await CardCmd.Exhaust(choiceContext, burn);
            hits = DynamicVars.Repeat.IntValue;
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hits).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}