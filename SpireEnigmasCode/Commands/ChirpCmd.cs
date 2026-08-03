using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Commands;

public static class ChirpCmd
{
  
  private static Tween? _activeTween;
  
  public static async Task<SummonResult> GainEnergy(
    PlayerChoiceContext choiceContext,
    Player summoner,
    Decimal amount,
    AbstractModel? source)
  {
    ICombatState combatState = summoner.Creature.CombatState;
    
    if (amount == 0M)
      return new SummonResult(GetChirpFromPlayer(summoner), 0M);
    
    if (CombatManager.Instance.IsInProgress)
      SfxCmd.Play("event:/sfx/characters/necrobinder/necrobinder_summon");
    
    Creature? chirp = combatState.Allies.FirstOrDefault(c => c.Monster is Chirp && c.PetOwner == summoner);
    if (IsPlayerChirpAlive(summoner))
    {
      ((Chirp) chirp.Monster).GainEnergy(amount);
    }
    else
    {
      chirp = await PlayerCmd.AddPet<Chirp>(summoner);
      NCreature? chirpNode = NCombatRoom.Instance?.GetCreatureNode(chirp);
      chirpNode.ToggleIsInteractable(true);

      if (chirpNode != null && source is CardModel)
      {
        chirpNode.Modulate = Colors.Transparent;
        chirpNode.CreateTween().TweenProperty(chirpNode, (NodePath) "modulate", Colors.White, 0.3499999940395355).SetDelay(0.10000000149011612);
        chirpNode.StartReviveAnim();
      }
      await PowerCmd.Apply<InternalBatteryPower>(choiceContext, chirp, 1, null, null);
      //chirpNode?.TrackBlockStatus(summoner.Creature);
      ((Chirp) chirp.Monster).GainEnergy(amount);
      chirpNode.Position += GetChirpOffsetFromPlayer(chirp);
      chirp.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
      await CreatureCmd.SetMaxAndCurrentHp(chirp, 999999999M);
    }
    //CombatManager.Instance.History.Summoned(combatState, (int) amount, summoner);
    return new SummonResult(GetChirpFromPlayer(summoner), amount);
  }
  
  /*
   * This is probably a bad idea.
   * The goal is for this command to essentially act like Chirp is a player giving its Owner block.
   * So it should be affected by its own powers but not those of the target.
   *
   * Tentative plan is to treat Chirp like it's giving itself block up until the end, but give the
   * block to its Owner instead.
   *
   * This is complicated by the fact that the player is playing the card!
   * Players usually put their powers into cards they play. So cardPlay should be null
   * in a lot of places probably.
   */
  public static async Task<Decimal> GiveBlockToOwner(
    Player player,
    Decimal amount,
    ValueProp props,
    CardPlay? cardPlay,
    bool fast = false)
  {
    //experimental
    CardModel? card = cardPlay?.Card;
    cardPlay = null;
    Creature realTarget = player.Creature;
    
    Creature chirp = GetChirpFromPlayer(player);
    if(chirp is null) 
      return 0M;
    if (CombatManager.Instance.IsOverOrEnding)
      return 0M;
    if (player.Creature.IsDead)
      return 0M;
    
    ICombatState combatState = chirp.CombatState;
    await Hook.BeforeBlockGained(combatState, chirp, amount, props, card);
    Decimal modifiedAmount = amount;
    IEnumerable<AbstractModel> modifiers;
    modifiedAmount = Hook.ModifyBlock(combatState, chirp, modifiedAmount, props, card, cardPlay, out modifiers);
    modifiedAmount = Math.Max(modifiedAmount, 0M);
    await Hook.AfterModifyingBlockAmount(combatState, modifiedAmount, card, cardPlay, modifiers);
    
    if (modifiedAmount > 0M)
    {
      SfxCmd.Play("event:/sfx/block_gain");
      VfxCmd.PlayOnCreatureCenter(realTarget, "vfx/vfx_block");
      realTarget.GainBlockInternal(modifiedAmount);
      CombatManager.Instance.History.BlockGained(combatState, realTarget, (int) modifiedAmount, props, cardPlay);
      if (fast)
        await Cmd.CustomScaledWait(0.0f, 0.03f);
      else
        await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
    await Hook.AfterBlockGained(combatState, realTarget, modifiedAmount, props, card);
    return modifiedAmount;
  }
  
  public static Creature? GetChirpFromPlayer(Player player)
  {
    return player.PlayerCombatState?.GetPet<Chirp>();
  }
  
  public static bool IsPlayerChirpAlive(Player player)
  {
    Creature? chirp = GetChirpFromPlayer(player);
    return chirp != null && chirp.IsAlive;
  }
  
  private static Vector2 GetChirpOffsetFromPlayer(Creature chirp)
  {
    return NCombatRoom.Instance.GetCreatureNode(chirp.PetOwner.Creature).Hitbox.Position * 0.5f + Chirp.Offset;
  }
}