#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
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

    public List<AdversaryIntent> Intents = new ();
    
    public static Vector2 Offset => new Vector2(-480, 140);

    public static int BiteDamage => 2;

    public static int SliceDamage => 1;

    public static int SliceHits => 3;
    
    protected override string VisualsPath => "res://SpireEnigmas/monsters/transient/transient.tscn";
    
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
    }

    //This gets incremented by 1 at the start of combat since it uses the same logic as every other turn
    public int MoveCyclePosition = 3;

    public List<AdversaryIntent> MoveCycle =>
        [
            new (EncroachMove, new SleepIntent(), false),
            new (BiteMove, new SingleAttackIntent(BiteDamage), false),
            new (EncroachMove, new SleepIntent(), false),
            new (SliceMove, new MultiAttackIntent(SliceDamage, SliceHits), false)
        ];

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> states = new List<MonsterState>();
        MoveState state1 = new MoveState("ENCROACH_MOVE", _ => Task.CompletedTask, 
            new SleepIntent()
        );
        state1.FollowUpState = state1;
        return new MonsterMoveStateMachine(states, state1);
    }

    static async Task EncroachMove(Dictionary<Creature, int> targets)
    {
        await Task.CompletedTask;
    }
    
    private async Task BiteMove(Dictionary<Creature, int> damages)
    {
        await PerformGroupAttack(damages);
    }
    
    private async Task SliceMove(Dictionary<Creature, int> damages)
    {
        await PerformGroupAttack(damages, SliceHits);
    }
    
    public async Task PerformGroupAttack(Dictionary<Creature, int> damages, int hitCount = 1)
    {
        foreach (Creature target in damages.Keys)
        {
            damages.TryGetValue(target, out var damage);
            await DamageCmd.Attack(damage).FromAdversary(Creature).Targeting(target).WithValueProp(ValueProp.Unpowered).WithHitCount(hitCount).WithAttackerFx(sfx: AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
        }
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

    public async Task AddIntentForTurn(Func<Dictionary<Creature, int>, Task> action, AbstractIntent intent)
    {
        Intents.Add(new AdversaryIntent(action, intent, true));
        await ReorderAndDisplayIntents();
    }

    /*
     * The adversary performs each of its intents, then clears all temporary intents,
     * then evaluates it's next intent.
     */
    public async Task PerformTurn()
    {
        List<Creature> targets = EnigmasHelper.GetAdversaryTargetsForPlayer(Creature.PetOwner!);
        
        foreach (AdversaryIntent adversaryIntent in Intents)
        {
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
                        damagesToDeal.Add(target, attackIntent.GetSingleDamage(targets, Creature));
                    }
                }
            }
            await adversaryIntent.Action(damagesToDeal);
            await Cmd.CustomScaledWait(0.5f, 0.35f);
        }

        //Clear temporary intents
        Intents = Intents.Where(adversaryIntent => !adversaryIntent.IsTemporary).ToList();

        IsPerformingMove = false;
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        await GoToNextIntent();
    }

    private async Task ReorderAndDisplayIntents()
    {
        List<AdversaryIntent> allIntents = Intents.ToList();

        List<AdversaryIntent> attackingIntents =
            allIntents.Where(intent => intent.Intent is AttackIntent).ToList();
        
        List<AdversaryIntent> nonAttackingIntents =
            allIntents.Where(intent => intent.Intent is not AttackIntent).ToList();

        List<AdversaryIntent> reorderedIntents = attackingIntents.Concat(nonAttackingIntents).ToList();
        if (reorderedIntents.Count > 1)
        {
            reorderedIntents = reorderedIntents.Where(intent => intent.Intent is not SleepIntent).ToList();
        }

        Intents = reorderedIntents;

        NextMove.Intents = reorderedIntents.Select(intent => intent.Intent).ToList().AsReadOnly();
            
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
        
        if (creatureNode == null)
            return;

        await TaskHelper.RunSafely(creatureNode.RefreshIntents());
    }
    
    public async Task GoToNextIntent()
    {
        MoveCyclePosition = (MoveCyclePosition + 1) % 4;

        //Keep temporary intents while remove cycle intent. Then add the next intent
        Intents = Intents.Where(adversaryIntent => adversaryIntent.IsTemporary).Concat([MoveCycle[MoveCyclePosition]]).ToList();
        
        //Reorder the intents and process their display
        await ReorderAndDisplayIntents();
    }

    public void CachePlayerIntentDamage()
    {
        foreach (AttackIntent attackIntent in Intents.Select(intent => intent.Intent)
                     .OfType<AttackIntent>())
        {
            LockedMoveDamages.Add(attackIntent, attackIntent.GetSingleDamage(EnigmasHelper.GetAdversaryTargetsForPlayer(Creature.PetOwner!), Creature));
        }
    }
}