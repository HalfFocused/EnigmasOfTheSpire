using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
class ChirpEnergyPatch
{
    [HarmonyPostfix]
    static void AllowPlayingIfChirpBatterySufficient(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
    {
        Creature? playerChirp = ChirpHelper.GetChirpFromPlayer(__instance._player);
        if(playerChirp is null) return;
        Chirp chirpMonster = (Chirp) playerChirp.Monster;
        
        int calculatedEnergyCost = Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
        int chirpBatteryEnergy = chirpMonster.GetEnergy();
        
        /*
         * The logic here could almost certainly be done in a more clever way.
         * But I don't care!!!!!!!!!!!!!!!!!!!!!
         */
        
        //step 1: if the card is a command, we can only play it if our chirp battery is >= its cost
        if (card.Keywords.Contains(EnigmaEnums.Command))
        {
            /*
             * If we think we can play this card, but our Chirp battery is less than its cost,
             * we actually can't!
             */
            //because this card is a command, it will always cost battery equal to it's Cost.
            ChirpBatteryCostField.ChirpBatteryCost.Set(card, calculatedEnergyCost);
            if (calculatedEnergyCost <= chirpBatteryEnergy)
            {
                //if chirp has enough battery energy:
                __result = true;
                reason = UnplayableReason.None;
            }
            else
            {
                //if chirp does not have enough battery energy:
                __result = false;
                reason = UnplayableReason.EnergyCostTooHigh;
                //todo: add new unplayable reason that chirp doesn't have enough energy
            }
            return;
        }
        else
        {
            //if the card is not a command, the player and chirp's energy can be used together.
            if (!__result && reason == UnplayableReason.EnergyCostTooHigh)
            {
                //if we cannot play the card because we don't have enough energy
                if (calculatedEnergyCost <= chirpBatteryEnergy + __instance.Energy)
                {
                    //if the player and chirp combined have enough energy
                    ChirpBatteryCostField.ChirpBatteryCost.Set(card, calculatedEnergyCost - __instance.Energy);
                    __result = true;
                    reason = UnplayableReason.None;
                    return;
                }
            }
            //if we already can play the card, set it's battery cost to 0.
            ChirpBatteryCostField.ChirpBatteryCost.Set(card, 0);
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendEnergy))]
class SpendChirpBatteryCostPatch
{
    [HarmonyPrefix]
    static void SpendCalculatedChirpBatteryCost(CardModel __instance, ref int amount)
    {
        int calculatedBatteryCost = ChirpBatteryCostField.ChirpBatteryCost.Get(__instance);
        if (calculatedBatteryCost > 0)
        {
            Creature? playerChirp = ChirpHelper.GetChirpFromPlayer(__instance.Owner);
            if(playerChirp is null) return; //this shouldnt be possible
            Chirp chirpMonster = (Chirp) playerChirp.Monster;
            chirpMonster.SpendEnergy(calculatedBatteryCost);

            if (__instance.Keywords.Contains(EnigmaEnums.Command))
            {
                amount = 0;
                return;
            }
        }
        amount -= calculatedBatteryCost;
    }
}

/*
 * Did you know that entomancer's Personal Hive power has a hard-coded Osty check? :D
 * So without this patch, Chirp attacks brick the game when attacking it.
 *
 * This patch simply adds in a similar Chirp check & reassignment right after the Osty check
 */
[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived))]
class PersonalHiveChirpAttackPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
                CodeMatch.IsLdarg(0), //0 - this
                new CodeMatch(OpCodes.Ldfld), //1 - loads the 'dealer' variable
                new CodeMatch(OpCodes.Callvirt), //2 - calls getMonster()
                new CodeMatch(OpCodes.Isinst), //3 - checks if it's an Osty
                new CodeMatch(OpCodes.Brfalse_S), //4 - skips past 5-10 if not Osty
                
                CodeMatch.IsLdarg(0), //5 - this
                CodeMatch.IsLdarg(0), //6 - this
                new CodeMatch(OpCodes.Ldfld), //7 - loads 'dealer' again
                new CodeMatch(OpCodes.Callvirt), //8 - getPetOwner()
                new CodeMatch(OpCodes.Callvirt), //9 - getCreature()
                new CodeMatch(OpCodes.Stfld) //10 - sets 'dealer' to the result
            ).ThrowIfInvalid("Could not find Osty check & reassignment");
            
        object dealerField = codeMatcher.InstructionAt(1).operand;
        object getMonsterMethod = codeMatcher.InstructionAt(2).operand;
        
        object getPetOwnerMethod = codeMatcher.InstructionAt(8).operand;
        object getCreatureMethod = codeMatcher.InstructionAt(9).operand;

        /*
         * they call this move the pointer shuffle
         */
        
        //we advance late for the sake of setting those variables
        codeMatcher.Advance(11); 
        
        //save this position.
        codeMatcher.CreateLabel(out Label startOfChirpCheck);
        //go back to the BrFalse instruction
        codeMatcher.Advance(-7);
        //instead of the Osty check skipping to end, we want it to skip to here
        codeMatcher.SetOperandAndAdvance(startOfChirpCheck);
        //go back to where the chirp check is being created
        codeMatcher.Advance(7);
        
        codeMatcher.InsertAndAdvance(
            /*
             * All this is a functional copy of the Osty check, except with Chirp instead.
             * This is why we needed to grab those 4 variables to ensure resistance against field or method renames
             */
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, dealerField),
            new CodeInstruction(OpCodes.Callvirt, getMonsterMethod),
            new CodeInstruction(OpCodes.Isinst, typeof(Chirp)),
            new CodeInstruction(OpCodes.Brfalse_S), //created without an operand, this gets set later once we have a label
            
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, dealerField),
            new CodeInstruction(OpCodes.Callvirt, getPetOwnerMethod),
            new CodeInstruction(OpCodes.Callvirt, getCreatureMethod),
            new CodeInstruction(OpCodes.Stfld, dealerField)
        );
        //grab this position
        codeMatcher.CreateLabel(out Label endOfChirpCheck);
        //go back to the BrFalse we created without an Operand and tell it to jump here.
        codeMatcher.Advance(-7);
        codeMatcher.SetOperandAndAdvance(endOfChirpCheck);

        return codeMatcher.Instructions();
    }
}

[HarmonyPatch(typeof(PenNib), nameof(PenNib.ModifyDamageMultiplicative))]
class PenNibChirpAttackPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_S), //gets cardSource
            new CodeMatch(OpCodes.Brtrue_S), //if cardSource is not null, continue
            new CodeMatch(OpCodes.Ldsfld), //load 1
            new CodeMatch(OpCodes.Ret), //return

            new CodeMatch(OpCodes.Ldarg_S), //get dealer
            new CodeMatch(OpCodes.Ldarg_0), //this
            new CodeMatch(OpCodes.Call), //getOwner
            new CodeMatch(OpCodes.Callvirt), //get Creature field
            new CodeMatch(OpCodes.Beq_S), //if dealer = owner.Creature, continue

            new CodeMatch(OpCodes.Ldarg_S), //get dealer
            new CodeMatch(OpCodes.Ldarg_0), //this
            new CodeMatch(OpCodes.Call), //getOwner
            new CodeMatch(OpCodes.Callvirt), //get Osty field
            new CodeMatch(OpCodes.Beq_S) //if dealer = Owner.Osty, continue

        ).ThrowIfInvalid("Could not find Osty check");

        object dealerField = codeMatcher.InstructionAt(9).operand;
        object getOwnerMethod = codeMatcher.InstructionAt(11).operand;

        object breakToAddress = codeMatcher.InstructionAt(13).operand;

        //we advance late for the sake of setting those variables
        codeMatcher.Advance(14);

        codeMatcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_S, dealerField), //dealer variable
            new CodeInstruction(OpCodes.Ldarg_0), //this
            new CodeInstruction(OpCodes.Call, getOwnerMethod), //getOwner
            CodeInstruction.Call(() => ChirpHelper.GetChirpFromPlayer(default)), //getChirpFromPlayer
            new CodeInstruction(OpCodes.Beq_S, breakToAddress) //if dealer = getChirpFromPlayer(this.GetOwner), continue
        );

        return codeMatcher.Instructions();
    }
}

/*
 * These following two patches ensure that Chirp is included in the participant list for
 * the BeforeSideTurnEnd and AfterSideTurnEnd hooks.
 *
 * This is important for ensuring powers like The Bomb, Temporary Strength/Dexterity, etc work on it.
 */
[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeSideTurnEnd))]
class AddChirpToBeforeSideEndParticipants
{
    [HarmonyPrefix]
    static void AddToBeforeSideEndParticipants(
        ICombatState combatState,
        CombatSide side,
        ref IEnumerable<Creature> participants)
    {
        foreach (Creature participant in participants)
        {
            Creature? chirp = ChirpHelper.GetChirpFromPlayer(participant.Player);

            if (chirp is not null)
            {
                participants = participants.Append(chirp);
            }
        }
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterSideTurnEnd))]
class AddChirpToAfterSideEndParticipants
{
    [HarmonyPrefix]
    static void AddToAfterSideEndParticipants(
        ICombatState combatState,
        CombatSide side,
        ref IEnumerable<Creature> participants)
    {
        foreach (Creature participant in participants)
        {
            Creature? chirp = ChirpHelper.GetChirpFromPlayer(participant.Player);

            if (chirp is not null)
            {
                participants = participants.Append(chirp);
            }
        }
    }
}