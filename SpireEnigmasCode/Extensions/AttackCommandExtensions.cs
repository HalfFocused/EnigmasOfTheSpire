using Godot;
using Godot.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using SpireEnigmas.SpireEnigmasCode.Monsters;

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

        return NShivThrowVfx.Create(chirpLocation, targetLocation, Colors.Red);
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
            vfxContainer?.AddChildSafely(NShivThrowVfx.Create(chirpLocation, targetLocation, Colors.Red));
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

        return NShivThrowVfx.Create(initialLocation, targetLocation, Colors.Red);
    }
    
    private static async Task VaporizeVfx(Creature chirp, Creature? targetCreature)
    {
        if(targetCreature is null) return;
        
        NHyperbeamVfx? vfx = NHyperbeamVfx.Create(chirp, targetCreature);

        if (vfx == null)
            return;

        vfx.Modulate = Colors.Red;

        NCombatRoom.Instance.Ui.AddChildSafely(vfx);

        await Cmd.Wait(NHyperbeamVfx.hyperbeamAnticipationDuration);
    }
    
    private static async Task AoeVfx(Creature chirp)
    {
        NSweepingBeamVfx? vfx = NSweepingBeamVfx.Create(chirp, chirp.CombatState!.GetOpponentsOf(chirp).ToList());

        if (vfx == null)
            return;

        vfx.Modulate = Colors.Red;

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
}