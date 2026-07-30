using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Monsters;

public sealed class Chirp : CustomMonsterModel
{

  public Chirp()
  {
    CustomContentDictionary.RegisterType(GetType());
  }
  
  public const float attackerAnimDelay = 0.3f;
  public const string pokeAnim = "attack_poke";
  public const string ostyAttackSfx = "event:/sfx/characters/osty/osty_attack";

  public static Vector2 Offset => new Vector2(125, -155);

  public override int MinInitialHp => 9999;

  public override int MaxInitialHp => 9999;
  
  public override string CustomVisualPath => "res://SpireEnigmas/monsters/chirp/sentry.tscn";

  public override bool IsHealthBarVisible => false;
  
  public override string DeathSfx => "event:/sfx/characters/osty/osty_die";

  public override bool HasDeathSfx => true;
  
  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    MoveState initialState = new MoveState("NOTHING_MOVE", (_ => Task.CompletedTask));
    initialState.FollowUpState = initialState;
    return new MonsterMoveStateMachine([initialState], initialState);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("idle", true);
    var attack = new AnimState("attack");
    var spaz1 = new AnimState("spaz1");
    var spaz2 = new AnimState("spaz2");
    var spaz3 = new AnimState("spaz3");
    var hit = new AnimState("hit");
    attack.NextState = idle;
    spaz1.NextState = idle;
    spaz2.NextState = idle;
    spaz3.NextState = idle;
    hit.NextState = idle;
    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Attack", attack);
    animator.AddAnyState("spaz1", spaz1);
    animator.AddAnyState("spaz2", spaz2);
    animator.AddAnyState("spaz3", spaz3);
    animator.AddAnyState("Hit", hit);

    var animState = controller.GetAnimationState();
    animState.SetTimeScale(2.0f);
    var current = animState.GetCurrent(0);
    current.SetTrackTime(Rng.NextFloat(current.GetAnimationEnd()));
    animState.Update(0.0f);
    animState.Apply(controller.GetSkeleton());

    return animator;
  }

  public static bool CheckMissingWithAnim(Player owner)
  {
    NCombatRoom.Instance?.ShakeOstyIfDead(owner);
    return owner.IsOstyMissing;
  }

  public void GainEnergy(decimal amount)
  {
    PowerModel? internalBatteryPower = Creature.GetPower<InternalBatteryPower>();
    if (internalBatteryPower == null) return;
    ((InternalBatteryPower) internalBatteryPower).GainReserve(amount);
  }
  
  public int GetEnergy()
  {
    PowerModel? internalBatteryPower = Creature.GetPower<InternalBatteryPower>();
    if (internalBatteryPower is null) return 0;
    return internalBatteryPower.DynamicVars.Energy.IntValue;
  }
  
  public void LoseEnergy(decimal amount)
  {
    PowerModel? internalBatteryPower = Creature.GetPower<InternalBatteryPower>();
    if (internalBatteryPower is null) return;
    ((InternalBatteryPower) internalBatteryPower).LoseReserve(amount);
  }
}