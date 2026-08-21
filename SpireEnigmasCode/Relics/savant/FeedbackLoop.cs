using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Relics.savant;

public class FeedbackLoop : TheSavantRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        Creature? chirp = EnigmasHelper.GetChirpFromPlayer(Owner);
        //hypothetically this is impossible, as this is a savant relic, who always has chirp
        if(chirp is null) return;
        if (command.Attacker == chirp)
        {
            if(CombatManager.Instance.History.Entries.OfType<CreatureAttackedEntry>().Count(entry => entry.Actor == chirp && entry.HappenedThisTurn(Owner.Creature.CombatState)) == 1)
            {
                Flash();
                await ChirpCmd.GainEnergy(choiceContext, Owner, DynamicVars.Energy.BaseValue, this);
            }
        }
        
    }
}