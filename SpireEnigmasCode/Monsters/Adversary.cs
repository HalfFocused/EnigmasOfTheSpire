#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using BaseLib.Patches.Features;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Monsters;

public sealed class Adversary : CustomMonsterModel
{
    public Adversary()
    {
        CustomContentDictionary.RegisterType(GetType());
    }

    public Dictionary<AttackIntent, int> LockedMoveDamages = new ();
    
    public override int MinInitialHp => 999;
    public override int MaxInitialHp => 999;

    public string ExposedSfx => AttackSfx;
    
    public static Vector2 Offset => new (-460, 140);
    
    //This gets incremented by 1 at the start of combat since it uses the same logic as every other turn
    public int MoveCyclePosition = 3;

    private List<List<AdversaryIntent>>? _moveCycle;

    public List<List<AdversaryIntent>> MoveCycle => _moveCycle ??=
    [
        [new (SingleHitAttackMove, new SingleAttackIntent(3), false)],
        [new (SingleHitAttackMove, new SingleAttackIntent(3), false)],
        [new (ThreeHitAttackMove, new MultiAttackIntent(1, 3), false)],
        [new (EncroachMove, new BuffIntent(), false, ModelDb.Get<StrengthPower>().Title.GetFormattedText(), 1)]
    ];

    public List<AdversaryIntent> CurrentIntents => MoveCycle[MoveCyclePosition];
    
    private async Task EncroachMove(Dictionary<Creature, int> targets)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
    }
    
    private async Task SingleHitAttackMove(Dictionary<Creature, int> damages)
    {
        await PerformGroupAttack(damages);
    }
    
    private async Task ThreeHitAttackMove(Dictionary<Creature, int> damages)
    {
        await PerformGroupAttack(damages, 3);
    }
    
    protected override string VisualsPath => "res://SpireEnigmas/monsters/transient/transient.tscn";
    
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> states = new List<MonsterState>();
        MoveState state1 = new MoveState("ENCROACH_MOVE", _ => Task.CompletedTask, 
            new SleepIntent()
        );
        state1.FollowUpState = state1;
        return new MonsterMoveStateMachine(states, state1);
    }
    
    public async Task PerformGroupAttack(Dictionary<Creature, int> damages, int hitCount = 1)
    {
        await DamageCmd.Attack(0).FromAdversary(Creature).TargetingFiltered(damages.Keys).WithValueProp(ValueProp.Move).WithHitCount(hitCount).WithAttackerFx(sfx: AttackSfx).WithHitVfxNode(t => NScratchVfx.Create(t, Creature.PetOwner!.RunState.Rng.Niche.NextBool()))
            .ExecuteWithDamageTable(new ThrowingPlayerChoiceContext(), damages);
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idle = new AnimState("Idle", true);
        var attack = new AnimState("Attack");
        var hurt = new AnimState("Hurt");

        attack.NextState = idle;
        hurt.NextState = idle;

        var animator = new CreatureAnimator(idle, controller);
        animator.AddAnyState("Swing", attack);
        animator.AddAnyState("Hurt", hurt);

        return animator;
    }

    public async Task AddIntentToCycle(Func<Dictionary<Creature, int>, Task> action, AbstractIntent intent, bool temporary, string powerName = "", int powerAmount = 0)
    {
        AdversaryIntent intentToAdd = new AdversaryIntent(action, intent, true, powerName, powerAmount);
        CurrentIntents.Add(intentToAdd);
        await ReorderAndDisplayIntents();
    }

    /*
     * The adversary performs each of its intents, then clears all temporary intents,
     * then evaluates it's next intent.
     *
     * Some cards might make the Adversary take a turn against a specific creature,
     * in which case overrideTargets will not be null.
     */
    public async Task PerformTurn(List<Creature>? overrideTargets = null)
    {
        List<Creature> targets = overrideTargets ?? EnigmasHelper.GetAdversaryTargetsForPlayer(Creature.PetOwner!);
        
        foreach (AdversaryIntent adversaryIntent in CurrentIntents)
        {
            await FlashSpecificIntent(adversaryIntent.Intent);
            await Cmd.CustomScaledWait(0.35f, 0.5f);
            Dictionary<Creature, int> damagesToDeal = new Dictionary<Creature, int>();

            if (adversaryIntent.Intent is AttackIntent { DamageCalc: not null } attackIntent)
            {
                foreach (Creature target in targets)
                {
                    if (target == Creature.PetOwner!.Creature && LockedMoveDamages.Remove(attackIntent, out var cachedDamage))
                    {
                        damagesToDeal.Add(target, cachedDamage);
                    }
                    else
                    {
                        damagesToDeal.Add(target, attackIntent.GetDamageTo(Creature.PetOwner.RunState, target, Creature));
                    }
                }

                LockedMoveDamages.Remove(attackIntent);
            }
            else
            {
                foreach (Creature target in targets)
                {
                    damagesToDeal.Add(target, 0);
                }
            }

            await adversaryIntent.Action(damagesToDeal);
        }

        //Clear temporary intents

        IsPerformingMove = false;
        await GoToNextIntent();
        await Cmd.CustomScaledWait(0.2f, 0.35f);
    }

    private async Task ReorderAndDisplayIntents()
    {
        for (int i = 0; i < MoveCycle.Count; i++)
        {
            List<AdversaryIntent> intentsInTurn = MoveCycle[i].ToList();

            List<AdversaryIntent> attackingIntents =
                intentsInTurn.Where(intent => intent.Intent is AttackIntent).ToList();
        
            List<AdversaryIntent> nonAttackingIntents =
                intentsInTurn.Where(intent => intent.Intent is not AttackIntent).ToList();

            List<AdversaryIntent> reorderedIntents = attackingIntents.Concat(nonAttackingIntents).ToList();
            if (reorderedIntents.Count > 1)
            {
                reorderedIntents = reorderedIntents.Where(intent => intent.Intent is not SleepIntent).ToList();
            }
            MoveCycle[i] = reorderedIntents.ToList();
        }
        
        //CurrentIntents = MoveCycle[MoveCyclePosition].ToList();
        NextMove.Intents = CurrentIntents.Select(intent => intent.Intent).ToList();
        
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
        
        if (creatureNode == null)
            return;

        await TaskHelper.RunSafely(creatureNode.RefreshIntents());
    }
    
    /*
     * Clears temporary intents on the current intent list, then moves on.
     */
    public async Task GoToNextIntent()
    {
        MoveCycle[MoveCyclePosition] = CurrentIntents.Where(adversaryIntent => !adversaryIntent.IsTemporary).ToList();
        
        MoveCyclePosition = (MoveCyclePosition + 1) % 4;

        //Keep temporary intents while remove cycle intent. Then add the next intent
        //Intents = Intents.Where(adversaryIntent => adversaryIntent.IsTemporary).Concat([MoveCycle[MoveCyclePosition]]).ToList();
        //CurrentIntents = MoveCycle[MoveCyclePosition].ToList();
        
        //Reorder the intents and process their display
        await ReorderAndDisplayIntents();
    }

    public void CachePlayerIntentDamage()
    {
        foreach (AttackIntent attackIntent in CurrentIntents.Select(intent => intent.Intent)
                     .OfType<AttackIntent>())
        {
            LockedMoveDamages.Add(attackIntent, attackIntent.GetSingleDamage(EnigmasHelper.GetAdversaryTargetsForPlayer(Creature.PetOwner!), Creature));
        }
    }

    public async Task FlashSpecificIntent(AbstractIntent intent)
    {
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
        
        if (creatureNode == null)
            return;
        
        foreach (NIntent nintent in creatureNode.GetChildren().OfType<NIntent>())
        {
            if (nintent._intent == intent)
            {
                nintent.PlayPerform();
                nintent.SetFrozen(true);
            }
        }
    }
}