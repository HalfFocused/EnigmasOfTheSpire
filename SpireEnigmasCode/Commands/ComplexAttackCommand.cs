using System.Collections;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode.Commands;

/*
 * The plan is simple
 * In an ideal world, this class takes a list of attack commands and returns a single one
 * with the combined attacks of all of them. but counting as one attack command. wish me fucking luck
 */
public static class ComplexAttackCommand
{
    public static async Task CreateCompositeAttack(PlayerChoiceContext choiceContext, CardModel cardSource, CardPlay cardPlay, AttackCommand[] attackCommands)
    {
        AttackContext attackContext = await AttackCommand.CreateContextAsync(cardSource.CombatState, choiceContext, cardPlay);
        IEnumerable<DamageResult> damageResults;
        
        foreach (AttackCommand command in attackCommands)
        {
            for (int i = 0; i < command._hitCount; i++)
            {
                List<Creature> creaturesGettingHit = new List<Creature>();
            
                if (command.IsSingleTargeted)
                {
                    creaturesGettingHit.Add(command._singleTarget);
                }else if (command.IsMultiTargeted && !command.IsRandomlyTargeted)
                {
                    creaturesGettingHit.AddRange(command.GetPossibleTargets());
                }else if (command.IsMultiTargeted && command.IsRandomlyTargeted)
                {
                    List<Creature> validTargets = command.GetPossibleTargets().Where(c => c.IsAlive).ToList();
                    //TODO: figure out wtf is up with doesRandomTargetingAllowDuplicates. is it even used?

                    creaturesGettingHit.Add((command.Attacker.Player ?? command.Attacker.PetOwner).RunState.Rng.CombatTargets
                        .NextItem(validTargets));
                }

                //TODO: vfx support for the attack commands. fucking kill me!!!!!
                damageResults = await CreatureCmd.Damage(choiceContext, creaturesGettingHit, command._damagePerHit, command.DamageProps,
                    command.Attacker, command.ModelSource as CardModel, command.CardPlay);
                attackContext.AddHit(damageResults);
            }
        }
        await attackContext.DisposeAsync();
    }
}