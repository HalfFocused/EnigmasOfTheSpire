#region

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Commands;

public static class AdversaryCmd
{
  
  private static Tween? _activeTween;
  
  public static async Task SummonAdversary(
    PlayerChoiceContext choiceContext,
    Player summoner)
  {
    ICombatState combatState = summoner.Creature.CombatState;
    
    if (CombatManager.Instance.IsInProgress)
      SfxCmd.Play("event:/sfx/characters/necrobinder/necrobinder_summon");
    
    Creature? adversary = combatState.Allies.FirstOrDefault(c => c.Monster is Adversary && c.PetOwner == summoner);
    if (!EnigmasHelper.IsPlayerAdversaryAlive(summoner))
    {
      adversary = await PlayerCmd.AddPet<Adversary>(summoner);
      adversary.PrepareForNextTurn([.. summoner.Creature.CombatState.Enemies, summoner.Creature]);
      NCreature? adversaryNode = NCombatRoom.Instance?.GetCreatureNode(adversary);
      adversaryNode.ToggleIsInteractable(true);
      
      await PowerCmd.Apply<NeverAlonePower>(choiceContext, adversary, 1, null, null);
      adversaryNode.Position += GetAdversaryOffsetFromPlayer(adversary);
      adversary.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
      await CreatureCmd.SetMaxAndCurrentHp(adversary, 999999999M);
      
      await ((Adversary)adversary.Monster).GoToNextIntent(); 
    }
  }

  public static async Task AddDebuffToIntent<T>(
    Player player,
    decimal amount
    ) where T : PowerModel
  {
    Creature? creature = EnigmasHelper.GetAdversaryFromPlayer(player);

    if (creature?.Monster is Adversary adversary)
    {
      DebuffIntent intent = new DebuffIntent();

      await adversary.AddIntentForTurn(
        dict => PowerCmd.Apply<T>(new ThrowingPlayerChoiceContext(), dict.Keys, amount, creature, null), intent);
    }
  }
  
  public static async Task AddAttackToIntent(
    Player player,
    int damage,
    int hits
  )
  {
    Creature? creature = EnigmasHelper.GetAdversaryFromPlayer(player);

    if (creature?.Monster is Adversary adversary)
    {
      AbstractIntent intent = hits == 1 ? new SingleAttackIntent(damage) : new MultiAttackIntent(damage, hits);

      await adversary.AddIntentForTurn(
        dict => adversary.PerformGroupAttack(dict, hits), intent);
    }
  }
  
  public static async Task TakeImmediateTurn(
    Player player
  )
  {
    Creature? creature = EnigmasHelper.GetAdversaryFromPlayer(player);

    if (creature?.Monster is Adversary adversary)
    {
      await adversary.PerformTurn();
    }
  }
  
  private static Vector2 GetAdversaryOffsetFromPlayer(Creature chirp)
  {
    return NCombatRoom.Instance.GetCreatureNode(chirp.PetOwner.Creature).Hitbox.Position * 0.5f + Adversary.Offset;
  }
}