using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;

[Pool(typeof(TheDisplacedCardPool))]
public class Elude() : DisplacedCard(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1M)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Elude cardSource = this;
        await CreatureCmd.TriggerAnim(cardSource.Owner.Creature, "Cast", cardSource.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(cardSource.Owner.Creature, "vfx/vfx_flying_slash");
        int amount = cardSource.DynamicVars["WeakPower"].IntValue;
        foreach (Creature enemy in cardSource.CombatState.HittableEnemies)
        {
            if (enemy.Monster.IntendsToAttack)
            {
                await PowerCmd.Apply<WeakPower>(choiceContext, enemy, (Decimal) amount, cardSource.Owner.Creature, (CardModel) cardSource);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<WeakPower>().UpgradeValueBy(1M);
    }
}