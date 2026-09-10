using Godot;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Patches;

namespace SpireEnigmas.SpireEnigmasCode.Extensions;

public static class DynamicVarExtensions
{
    public static DynamicVar AndRefinesBy(this DynamicVar var, int amount)
    {
        RefinementFields.RefineBy.Set(var, amount);
        return var;
    }
    
    public static void UpgradeRefinementBy(this DynamicVar var, int amount)
    {
        RefinementFields.RefineBy.Set(var, RefinementFields.RefineBy.Get(var) + amount);
    }
    
    public static int RefinementAmount(this DynamicVar var)
    {
        return RefinementFields.RefineBy.Get(var);
    }
}