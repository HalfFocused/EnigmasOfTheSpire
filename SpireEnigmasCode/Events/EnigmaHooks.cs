using MegaCrit.Sts2.Core.Combat;
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
}