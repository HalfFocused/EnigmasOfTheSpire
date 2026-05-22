using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheDisplaced.TheDisplacedCode.Cards;

namespace TheDisplaced.TheDisplacedCode.Cards.uncommon;

public class Oppress() : TheDisplacedCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(0),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedEnergy").WithMultiplier((card, _) => GetUniqueDebuffsAmongEnemies(card))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(((CalculatedVar) DynamicVars["CalculatedEnergy"]).Calculate(play.Target), Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public static int GetUniqueDebuffsAmongEnemies(CardModel card)
    {
        int debuffsAmongEnemies = 0;
        List<ModelId> seenBefore = new List<ModelId>();
        foreach (Creature enemy in card.CombatState.Enemies)
        {
            foreach (PowerModel power in enemy.Powers)
            {
                if (power.TypeForCurrentAmount == PowerType.Debuff && !(power is ITemporaryPower) && !seenBefore.Contains(power.Id))
                {
                    seenBefore.Add(power.Id);
                    debuffsAmongEnemies++;
                }
            }
        }
        return debuffsAmongEnemies;
    }
}