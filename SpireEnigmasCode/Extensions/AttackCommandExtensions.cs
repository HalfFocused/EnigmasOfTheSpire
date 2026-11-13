#region

using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using SpireEnigmas.SpireEnigmasCode.Monsters;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Extensions;

//Mostly utilities to get asset paths.
public static class AttackCommandExtensions
{
    public static AttackCommand FromChirp(this AttackCommand command, Creature chirp, CardModel card, CardPlay? cardPlay, ChirpAttackVfxStyle style = ChirpAttackVfxStyle.Standard)
    {
        command.Attacker = chirp.Monster is Chirp ? chirp : throw new ArgumentException("Creature is not Chirp");
        command.ModelSource = card;
        command.CardPlay = cardPlay;
        command._attackerAnimName = "Attack";
        
        command._sourceType = AttackCommand.SourceType.Card;

        switch (style)
        {
            case ChirpAttackVfxStyle.Standard:
                command._attackerAnimDelay = 0.25f;
                command._customHitVfxNodes.Add(creature => CreateBulletWithInaccuracy(chirp, creature, 0.45f));
                command._customHitVfxNodes.Add(creature => CreateExplosionWithInaccuracy(creature, 0.25f, command._damagePerHit));
                break;
            case ChirpAttackVfxStyle.DevastationRains:
                command._attackerAnimDelay = 0.15f;
                command._customHitVfxNodes.Add(creature => CreateDescendingShotWithInaccuracy(chirp, creature, 0.45f));
                command._customHitVfxNodes.Add(creature => CreateExplosionWithInaccuracy(creature, 0.25f, command._damagePerHit * 2));
                break;
            case ChirpAttackVfxStyle.Vaporize:
                command._attackerAnimDelay = 0.15f;
                command._beforeDamage = () => VaporizeVfx(chirp, command._singleTarget);
                break;
            case ChirpAttackVfxStyle.Aoe:
                command._attackerAnimDelay = 0.15f;
                command._beforeDamage = () => AoeVfx(chirp);
                break;
        }
        
        return command.WithAttackerFx(sfx: "event:/sfx/characters/osty/osty_attack");
    }
    
    public static AttackCommand FromAdversary(this AttackCommand command, Creature adversary)
    {
        command.Attacker = adversary.Monster is Adversary ? adversary : throw new ArgumentException("Creature is not Adversary");
        
        command._attackerAnimName = "Swing";
        command._attackerAnimDelay = 0.15f;

        command._sourceType = AttackCommand.SourceType.None;
        
        return command.WithAttackerFx(sfx: "event:/sfx/characters/osty/osty_attack");
    }


    private static NShivThrowVfx? CreateBulletWithInaccuracy(Creature chirp, Creature targetCreature, float inaccuracy)
    {
        Vector2 targetLocation = Vector2.Zero;
        var chirpCreatureNode = NCombatRoom.Instance?.GetCreatureNode(chirp);
        Vector2 chirpLocation = chirpCreatureNode?.VfxSpawnPosition ?? Vector2.Zero;
        NCreature? targetCreatureNode = null;

        if (!targetCreature.IsDead)
        {
            targetCreatureNode = NCombatRoom.Instance?.GetCreatureNode(targetCreature);
            targetLocation = targetCreatureNode?.VfxSpawnPosition ?? Vector2.Zero;
        }
            
        if (targetCreatureNode is not null)
        {
            float offsetX = targetCreatureNode.Hitbox.Size.X * Rng.Chaotic.NextFloat(-inaccuracy, inaccuracy);
            float offsetY = targetCreatureNode.Hitbox.Size.Y * Rng.Chaotic.NextFloat(-inaccuracy, inaccuracy);
    
            targetLocation += new Vector2(offsetX, offsetY);
        }

        return NShivThrowVfx.Create(chirpLocation, targetLocation, Colors.Aqua);
    }
    
    private static NFireBurstVfx? CreateExplosionWithInaccuracy(Creature targetCreature, float inaccuracy, decimal damage)
    {
        Vector2 targetLocation = Vector2.Zero;
        NCreature? targetCreatureNode = null;

        if (!targetCreature.IsDead)
        {
            targetCreatureNode = NCombatRoom.Instance?.GetCreatureNode(targetCreature);
            targetLocation = targetCreatureNode?.VfxSpawnPosition ?? Vector2.Zero;
        }
            
        if (targetCreatureNode is not null)
        {
            float offsetX = targetCreatureNode.Hitbox.Size.X * Rng.Chaotic.NextFloat(-inaccuracy, inaccuracy);
            float offsetY = targetCreatureNode.Hitbox.Size.Y * Rng.Chaotic.NextFloat(-inaccuracy, inaccuracy);
    
            targetLocation += new Vector2(offsetX, offsetY);
        }

        return NFireBurstVfx.Create(targetLocation, GetExplosionSizeForDamage(damage));
    }
    
    public static async Task CreateUpwardsShotsWithInaccuracy(Creature chirp, int shots, float inaccuracy)
    {
        for (int i = 0; i < shots; i++)
        {
            var chirpCreatureNode = NCombatRoom.Instance?.GetCreatureNode(chirp);
            Vector2 chirpLocation = chirpCreatureNode?.VfxSpawnPosition ?? Vector2.Zero;
            Vector2 targetLocation = chirpLocation;
            
            if (chirpCreatureNode is not null)
            {
                float offsetX = chirpCreatureNode.Hitbox.Size.X * 2 * (0.15f + Rng.Chaotic.NextFloat(inaccuracy));
                float offsetY = chirpCreatureNode.Hitbox.Size.Y * -6;
    
                targetLocation += new Vector2(offsetX, offsetY);
            }
            Control? vfxContainer = chirp.GetVfxContainer();
            vfxContainer?.AddChildSafely(NShivThrowVfx.Create(chirpLocation, targetLocation, Colors.Aqua));
            SfxCmd.Play("event:/sfx/characters/osty/osty_attack");
            await Cmd.Wait(0.15f);
        }
    }
    
    private static NShivThrowVfx? CreateDescendingShotWithInaccuracy(Creature chirp, Creature targetCreature, float inaccuracy)
    {
        Vector2 initialLocation = Vector2.Zero;
        Vector2 targetLocation = Vector2.Zero;
        NCreature? targetCreatureNode = null;
        var chirpCreatureNode = NCombatRoom.Instance?.GetCreatureNode(chirp);


        if (!targetCreature.IsDead)
        {
            targetCreatureNode = NCombatRoom.Instance?.GetCreatureNode(targetCreature);
            initialLocation = targetLocation = targetCreatureNode?.VfxSpawnPosition ?? Vector2.Zero;
        }
            
        if (targetCreatureNode is not null)
        {
            float initialOffsetX = chirpCreatureNode.Hitbox.Size.X * -2f * (0.35f + Rng.Chaotic.NextFloat(inaccuracy));
            float initialOffsetY = chirpCreatureNode.Hitbox.Size.Y * -5f * (0.5f + Rng.Chaotic.NextFloat(0.5f));
            
            float targetOffsetX = targetCreatureNode.Hitbox.Size.X * Rng.Chaotic.NextFloat(-inaccuracy, inaccuracy);
            float targetOffsetY = targetCreatureNode.Hitbox.Size.Y * Rng.Chaotic.NextFloat(-inaccuracy, inaccuracy);
    
            initialLocation += new Vector2(initialOffsetX, initialOffsetY);
            targetLocation += new Vector2(targetOffsetX, targetOffsetY);
        }

        return NShivThrowVfx.Create(initialLocation, targetLocation, Colors.Aqua);
    }
    
    private static async Task VaporizeVfx(Creature chirp, Creature? targetCreature)
    {
        if(targetCreature is null) return;
        
        NHyperbeamVfx? vfx = NHyperbeamVfx.Create(chirp, targetCreature);

        if (vfx == null)
            return;

        vfx.Modulate = Colors.Aqua;

        NCombatRoom.Instance.Ui.AddChildSafely(vfx);

        await Cmd.Wait(NHyperbeamVfx.hyperbeamAnticipationDuration);
    }
    
    private static async Task AoeVfx(Creature chirp)
    {
        NSweepingBeamVfx? vfx = NSweepingBeamVfx.Create(chirp, chirp.CombatState!.GetOpponentsOf(chirp).ToList());

        if (vfx == null)
            return;

        vfx.Modulate = Colors.Aqua;

        NCombatRoom.Instance.Ui.AddChildSafely(vfx);

        //await Cmd.Wait(NSweepingBeamVfx.du);
    }

    private static float GetExplosionSizeForDamage(decimal damage)
    {
        return float.Lerp(0.15f, 0.65f, Math.Clamp((float)damage / 30f, 0f, 1f));
    }

    public enum ChirpAttackVfxStyle
    {
        Standard,
        Aoe,
        DevastationRains,
        Vaporize
    }

    public static async Task<AttackCommand> ExecuteWithDamageTable(this AttackCommand attackCommand, PlayerChoiceContext? choiceContext,
        Dictionary<Creature, int> damages)
    {
        ICombatState combatState = attackCommand.Attacker?.CombatState;
        if (attackCommand.Attacker == null)
          throw new InvalidOperationException("No attacker set.");
        if (CombatManager.Instance.IsOverOrEnding && (combatState == null || combatState.IsLiveCombat()))
          return attackCommand;
        if (combatState == null)
          throw new InvalidOperationException("No combat state even though combat is not over.");
        if (attackCommand.Attacker.IsDead)
          return attackCommand;
        if (!attackCommand.IsSingleTargeted && !attackCommand.IsMultiTargeted)
          throw new InvalidOperationException("No targets set.");
        await Hook.BeforeAttack(combatState, attackCommand);
        Decimal attackCount = Hook.ModifyAttackHitCount(combatState, attackCommand, attackCommand._hitCount);
        for (int i = 0; (Decimal) i < attackCount && !attackCommand.Attacker.IsDead; ++i)
        {
          List<Creature> validTargets = attackCommand.GetPossibleTargets().Where(c => c.IsAlive).ToList();
          if (validTargets.Count != 0 || !combatState.IsLiveCombat())
          {
            if (attackCommand._playOnEveryHit || i == 0)
            {
              if (attackCommand._attackerVfx != null)
                VfxCmd.PlayOnCreatureCenter(attackCommand.Attacker, attackCommand._attackerVfx);
              foreach (Func<Node2D> customAttackerVfxNode in attackCommand._customAttackerVfxNodes)
              {
                Control vfxContainer = attackCommand.Attacker.GetVfxContainer();
                if (vfxContainer != null)
                  vfxContainer.AddChildSafely((Node) customAttackerVfxNode());
              }
              if (attackCommand._attackerSfx != null)
                SfxCmd.Play(attackCommand._attackerSfx);
              else if (attackCommand._tmpAttackerSfx != null)
                NDebugAudioManager.Instance?.Play(attackCommand._tmpAttackerSfx);
              if (attackCommand._attackerAnimName != null && attackCommand._shouldPlayAnimation)
                await CreatureCmd.TriggerAnim(attackCommand._visualAttacker ?? attackCommand.Attacker, attackCommand._attackerAnimName, attackCommand._attackerAnimDelay);
              if (attackCommand._afterAttackerAnim != null)
                await attackCommand._afterAttackerAnim();
            }
            if (validTargets.Count > 0)
            {
              if (attackCommand.HitSfx != null)
                SfxCmd.Play(attackCommand.HitSfx);
              else if (attackCommand.TmpHitSfx != null)
                NDebugAudioManager.Instance?.Play(attackCommand.TmpHitSfx);
            }
            Creature? singleTarget;
            singleTarget = validTargets.Count != 1 ? null : validTargets[0];
            
            if ((attackCommand._waitBeforeHit).Any((w => (double) w > 0.0)))
              await Cmd.CustomScaledWait(attackCommand._waitBeforeHit[0], attackCommand._waitBeforeHit[1]);
            foreach (Func<Creature, Node2D> customHitVfxNode in attackCommand._customHitVfxNodes)
            {
              if (singleTarget != null)
              {
                Control? vfxContainer = singleTarget.GetVfxContainer();
                if (vfxContainer != null)
                  vfxContainer.AddChildSafely((Node) customHitVfxNode(singleTarget));
              }
              else
              {
                foreach (Creature creature in validTargets)
                {
                  Control? vfxContainer = creature.GetVfxContainer();
                  if (vfxContainer != null)
                    vfxContainer.AddChildSafely((Node) customHitVfxNode(creature));
                }
              }
            }
            if (attackCommand.HitVfx != null)
            {
              if (singleTarget != null)
              {
                if (attackCommand._spawnVfxOnCreatureCenter)
                  VfxCmd.PlayOnCreatureCenter(singleTarget, attackCommand.HitVfx);
                else
                  VfxCmd.PlayOnCreature(singleTarget, attackCommand.HitVfx);
              }
              else if (attackCommand._spawnVfxOnEachCreature)
              {
                if (attackCommand._spawnVfxOnCreatureCenter)
                  VfxCmd.PlayOnCreatureCenters((IEnumerable<Creature>) validTargets, attackCommand.HitVfx);
                else
                  VfxCmd.PlayOnCreatures((IEnumerable<Creature>) validTargets, attackCommand.HitVfx);
              }
              else
                VfxCmd.PlayOnSide(attackCommand.Attacker.Side.GetOppositeSide(), attackCommand.HitVfx, combatState);
            }
            if (attackCommand._beforeDamage != null)
              await attackCommand._beforeDamage();
            
            PlayerChoiceContext choiceContext1 = choiceContext ?? new BlockingPlayerChoiceContext();
            List<Creature> targets;
            if (singleTarget == null)
            {
              targets = validTargets;
            }
            else
            {
              targets = new List<Creature>(1);
              targets.Add(singleTarget);
            }
            
            foreach (Creature target in targets)
            {
                if (!damages.TryGetValue(target, out var amount))
                {
                    amount = (int) attackCommand._damagePerHit;
                }
                
                IEnumerable<DamageResult> results = await CreatureCmd.Damage(choiceContext1, [target], amount, attackCommand.DamageProps, attackCommand.Attacker, attackCommand.ModelSource as CardModel, attackCommand.CardPlay, ModifyDamageHookType.Cap);
                attackCommand.AddResultsInternal(results);
            }
          }
          else
            break;
        }
        CombatManager.Instance.History.CreatureAttacked(combatState, attackCommand.Attacker, attackCommand._results.SelectMany((r => r)).ToList());
        await Hook.AfterAttack(combatState, choiceContext ?? new BlockingPlayerChoiceContext(), attackCommand);
        return attackCommand;
    }
}