using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace SpireEnigmas.SpireEnigmasCode.Events;

public class EnigmaHooks
{
    public enum HookScope
    {
        Run,
        Combat,
        CombatRaw
    }
    
    private static IEnumerable<AbstractModel> ResolveListeners(
        HookScope scope,
        ICombatState? combatState = null,
        IRunState? runState = null)
    {
        return scope switch
        {
            HookScope.Run => runState == null ? [] : runState.IterateHookListeners(combatState),
            HookScope.Combat => combatState == null ? [] : Hook.IterateCombatHookListeners(combatState),
            HookScope.CombatRaw => combatState == null ? [] : combatState.IterateHookListeners(),
            _ => throw new ArgumentOutOfRangeException(nameof(scope))
        };
    }
    
    
    public static async Task Dispatch<THook>(ICombatState? combatState, Func<THook, Task> action,
        HookScope scope = HookScope.Combat, IRunState? runState = null)
        where THook : class
    {
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
            await action(model);
    }
    
    public static async Task DispatchWithContext<THook>(Player player,
        Func<THook, PlayerChoiceContext, Task> action,
        HookScope scope = HookScope.Combat, IRunState? runState = null)
        where THook : class
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        var netId = player.NetId;
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
        {
            if (model is not AbstractModel abstractModel) continue;
            var hookCtx = new HookPlayerChoiceContext(abstractModel, netId, combatState, GameActionType.Combat);
            var task = action(model, hookCtx);
            await hookCtx.AssignTaskAndWaitForPauseOrCompletion(task);
            abstractModel.InvokeExecutionFinished();
        }
    }
    
    public static bool Any<THook>(ICombatState? combatState, Func<THook, bool> predicate,
        HookScope scope = HookScope.Combat, IRunState? runState = null)
        where THook : class
    {
        return ResolveListeners(scope, combatState, runState).OfType<THook>().Any(predicate);
    }
    
    public static bool Any<THook>(ICombatState? combatState, Func<THook, bool> predicate,
        out IEnumerable<THook> matches, HookScope scope = HookScope.Combat, IRunState? runState = null)
        where THook : class
    {
        var list = ResolveListeners(scope, combatState, runState).OfType<THook>()
            .Where(predicate).ToList();
        matches = list;
        return list.Count > 0;
    }
    
    public static TValue Modify<THook, TValue>(
        ICombatState? combatState,
        TValue originalAmount,
        Func<THook, TValue, TValue> amountModifier,
        out IEnumerable<THook> modifiers,
        HookScope scope = HookScope.Combat,
        IRunState? runState = null)
        where THook : class
        where TValue : IEquatable<TValue>
    {
        var amount = originalAmount;
        var abstractModelList = new List<THook>();
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
        {
            var previous = amount;
            amount = amountModifier.Invoke(model, amount);
            if (!previous.Equals(amount))
                abstractModelList.Add(model);
        }

        modifiers = abstractModelList;
        return amount;
    }
}