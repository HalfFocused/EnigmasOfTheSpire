using System.Reflection;
using BaseLib.Patches.Features;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Patches;

/*
 * These patches are isolated from the rest because I expect them to become redundant at some point in the future,
 * when Baselib's custom targeting better supports potions.
 */
public class PotionTargetingPatches
{
    public static readonly Type[] CanTargetChirp = [
        typeof(StrengthPotion), 
        typeof(DexterityPotion),
        typeof(EnergyPotion),
        typeof(FlexPotion),
        typeof(SpeedPotion),
        typeof(FyshOil),
        //typeof(GigantificationPotion),
        typeof(MazalethsGift)
    ];
}

[HarmonyPatch(typeof(ModelDb), "Init")]
internal static class ModelDbTargetTypeInitPatch
{
    [HarmonyPostfix]
    private static void RegisterTargetTypes()
    {
        CustomTargetType.RegisterSingleTargetType(EnigmaEnums.ChirpOrAnyPlayer,
            (target, player) => target.IsAlive && ((target.Monster is Chirp && target.PetOwner == player) || target.IsPlayer));
    }
}

[HarmonyPatch]
internal static class PotionTargetingPatch
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        List<MethodBase> methods = new List<MethodBase>();
        
        foreach (Type type in PotionTargetingPatches.CanTargetChirp)
        {
            MethodInfo method = AccessTools.PropertyGetter(type, "TargetType");
            if (method != null) methods.Add(method);
        }
        
        return methods;
    }
    
    [HarmonyPostfix]
    static void LetPotionsTargetChirp(PotionModel __instance, ref TargetType __result)
    {
        if (ChirpHelper.GetChirpFromPlayer(__instance.Owner) is not null)
        {
            __result = EnigmaEnums.ChirpOrAnyPlayer;
        }
    }
}

[HarmonyPatch(typeof(PotionModel), nameof(PotionModel.IsValidTarget))]
internal class IsValidTargetPatch
{
    [HarmonyPrefix]
    static bool CustomValidTargets(PotionModel __instance, Creature? target, ref bool __result)
    {
        if (target == null) return true;
        if (__instance.TargetType == EnigmaEnums.ChirpOrAnyPlayer)
        {
            Func<Creature, Player, bool> func = (target, player) =>
                target.IsAlive && ((target.Monster is Chirp && target.PetOwner == player) || target.IsPlayer);
            
            __result = func.Invoke(target, __instance.Owner);
            return false;
        }
        //CustomTargetType.SingleTargeting.TryGetValue(__instance.TargetType, out var func);
        return true;
    }
}

[HarmonyPatch(typeof(PotionModel), nameof(PotionModel.CanThrowAtAlly))]
class PotionOnAllyPatch
{
    [HarmonyPostfix]
    static void AllowTargetedThrowing(PotionModel __instance, ref bool __result)
    {
        if (__instance.TargetType == EnigmaEnums.ChirpOrAnyPlayer)
        {
            __result = true;
        }
    }
}

/*
 * This patch overrides the energy potion's default functionality when it targets Chirp, instead giving Chirp 2 energy.
 * We need to prefix + early terminate because energy potion (rightfully) assumes it will only target players.
 */
[HarmonyPatch(typeof(EnergyPotion), "OnUse")]
class EnergyPotionChargesChirpPatch
{
    [HarmonyPrefix]
    static bool LetChargeChirp(PotionModel __instance, PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target?.Monster is Chirp)
        {
            NCombatRoom.Instance?.PlaySplashVfx(target, new Color("f2e35c"));
            ChirpCmd.GainEnergy(choiceContext, target.PetOwner, __instance.DynamicVars.Energy.BaseValue, __instance);
            return false;
        }
        return true;
    }
}