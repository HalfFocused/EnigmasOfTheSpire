using System.Reflection.Emit;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.token;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;
using Label = System.Reflection.Emit.Label;

namespace SpireEnigmas.SpireEnigmasCode.Patches;

/*
 * This is stored in each card and updated whenever PlayerCombatState.HasEnoughResourcesFor is called on that card.
 * It represents the amount of energy Chirp should lose from it's battery when the card is played.
 * This will be 0 for X cost cards. The battery loss for them is handled in InternalBatteryPower.
 */
class ChirpBatteryCostField
{
    public static readonly SpireField<CardModel, int> ChirpBatteryCost = new(()=>0);
}

[HarmonyPatch(typeof(ScrollBoxes), nameof(ScrollBoxes.GenerateRandomBundles))]
class ScrollBoxesGenerateRandomBundlesPatch
{
    /*
     * Adds a 1/100 chance for either of the Scroll Boxes rewards to be replaced with 3 copies of "Recurring Dream."
     * It's the Displaced's version of a Claw Pack.
     */
    
    [HarmonyPostfix]
    static void ChanceForNightmarePack(Player player, ref List<IReadOnlyList<CardModel>> __result)
    {
        Rng rewardsRng = player.PlayerRng.Rewards;
        if (player.Character is TheDisplaced)
        {
            for (int i = 0; i < __result.Count; i++)
            {
                if (rewardsRng.NextInt(100) < 1)
                {
                    CardModel recurringDream = ModelDb.Card<RecurringDream>();
                    __result[i] =
                    [
                        recurringDream, recurringDream, recurringDream
                    ];
                }
            }
        }
    }
}


[HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
internal static class GadgetDescriptionPatch
{
    [HarmonyPostfix]
    static void UseOneGadgetDescriptionPatch(CardModel __instance, ref LocString __result)
    {
        if (__instance is Gadget gadget)
        {
            __result = new LocString("cards", ModelDb.GetId(typeof(Gadget)).Entry + ".description");
        }
    }
}




/*
 * Modify the power icon of the Foretold power to display an infinity instead of a number under certain conditions.
 */
[HarmonyPatch(typeof(NPower), nameof(NPower.RefreshAmount))]
class PowerNodeInfinityDisplayPatch
{
    [HarmonyPostfix]
    static void ChangeLabelToInfinity(NPower __instance)
    {
        if (__instance._model is ForetoldPower)
        {
            if (__instance._model.Amount >= ForetoldPower.INFINITY)
            {
                __instance._amountLabel.SetTextAutoSize("∞");
            }
        }
    }
}

/*
 * Modify the description of the Foretold power if it is infinite.
 */
[HarmonyPatch(typeof(PowerModel), "SmartDescription", MethodType.Getter)]
class ForetoldPowerDescriptionPatch
{
    [HarmonyPostfix]
    static void ChangeDescriptionToInfinity(PowerModel __instance, ref LocString __result)
    {
        if (__instance is ForetoldPower)
        {
            if (__instance.Amount >= ForetoldPower.INFINITY)
            {
                __result = new LocString("powers", ((ForetoldPower)__instance).InfiniteDescriptionLocKey);
            }
        }
    }
}

/*
 * Stop debuff durations from decreasing if the creature has Stasis
 */
[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.TickDownDuration))]
class StasisDurationPatch
{
    [HarmonyPrefix]
    static bool CancelTickDown(ref Task __result, PowerModel power)
    {
        if (power.TypeForCurrentAmount == PowerType.Debuff && power is not StasisPower)
        {
            if (power.Owner.HasPower<StasisPower>())
            {
                __result = Task.CompletedTask;
                return false;
            }
        }

        return true;
    }
}

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
        if (card.Keywords.Contains(EnigmaKeywords.Command))
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

            if (__instance.Keywords.Contains(EnigmaKeywords.Command))
            {
                amount = 0;
                return;
            }
        }
        amount -= calculatedBatteryCost;
    }
}

[HarmonyPatch(typeof(CardModel), "ShouldGlowGold", MethodType.Getter)]
class FreestyleGlowGoldPatch
{
    [HarmonyPostfix]
    static void MakeGlowGold(CardModel __instance, ref bool __result)
    {
        if (__instance.Owner.HasPower<FreestylePower>())
        {
            if (FreestylePower.CardDrawnDuringThisTurn(__instance) && __instance.Type is CardType.Attack)
            {
                __result = true;
            }
        }
    }
}

/*
 * Doubles all card draw during the player's turn if they have the Unsustainable power.
 */
[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), 
    typeof(PlayerChoiceContext), typeof(Decimal), typeof(Player), typeof(bool))]
class DrawIncreasePatch
{
    [HarmonyPrefix]
    static void IncreaseCardDraw(PlayerChoiceContext choiceContext,
        ref Decimal count,
        Player player,
        bool fromHandDraw)
    {
        if (player.HasPower<UnsustainablePower>() && !fromHandDraw && count != 0)
        {
            UnsustainablePower? power = player.Creature.GetPower<UnsustainablePower>();
            power.Flash();
            count *= power.Amount;
        }
    }
}

[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform), typeof(IEnumerable<CardTransformation>), typeof(Rng),
    typeof(CardPreviewStyle))]
class CardTransformationHookPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                new CodeMatch(OpCodes.Ldfld),
                CodeMatch.Calls(typeof(CardPile).Method(nameof(CardModel.AfterTransformedFrom))),
                CodeMatch.IsLdarg(0),
                new CodeMatch(OpCodes.Ldfld),
                CodeMatch.Calls(typeof(CardPile).Method(nameof(CardModel.AfterTransformedTo)))
            ).ThrowIfInvalid("Could not find Transform hooks");
            
        object originalField = codeMatcher.InstructionAt(1).operand;
        object replacementField = codeMatcher.InstructionAt(4).operand;

        codeMatcher.Advance(6);
            
        codeMatcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, replacementField),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, originalField),
            CodeInstruction.Call(() => CatchTransformation(default, default))
        );

        return codeMatcher.Instructions();
    }

    static void CatchTransformation(CardModel original, CardModel replacement)
    {
        if (replacement.Owner.PlayerCombatState != null)
        {
            if (original is Scrap)
            {
                replacement.EnergyCost.SetUntilPlayed(0);
            }
            
            if (original is FormAndFunction)
            {
                PlayerCmd.GainEnergy(original.DynamicVars.Energy.BaseValue, replacement.Owner);
                CardPileCmd.Draw(new BlockingPlayerChoiceContext(), original.DynamicVars.Cards.BaseValue, replacement.Owner);
            }

            Player player = replacement.Owner;

            MasterworkPower? masterworkPower = player.Creature.GetPower<MasterworkPower>();

            if (masterworkPower is not null)
            {
                masterworkPower.Flash();
                replacement.BaseReplayCount += masterworkPower.Amount;
            }
            
            ImprovisedShieldPower? improvisedShieldPower = player.Creature.GetPower<ImprovisedShieldPower>();

            if (improvisedShieldPower is not null)
            {
                //improvisedShieldPower.Flash();
                improvisedShieldPower.AfterCardTransformed();
            }
        }
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

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
class CardGlowTweakPatch
{
    public static readonly Color chirpPlayableColor = new Color(0.95f, 0.45f, 0f, 0.98f);
    public static readonly Color madeEtherealColor = new Color(0.35f, 0.35f, 0.35f, 0.98f);


    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0), //this
            new CodeMatch(OpCodes.Call), //get_CardNode
            new CodeMatch(OpCodes.Callvirt), //get_Model
            new CodeMatch(OpCodes.Callvirt), //CanPlay()
            new CodeMatch(OpCodes.Brtrue_S)

        ).ThrowIfInvalid("Could not find card model check");

        object getCardNodeMethod = codeMatcher.InstructionAt(1).operand;
        object getModelMethod = codeMatcher.InstructionAt(2).operand;

        //we advance late for the sake of setting those variables
        codeMatcher.Advance(5);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Callvirt),

            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldsfld), //
            /*
             * We want to sneak in our method here to intercept the loaded static color and potentially change it
             */
            new CodeMatch(OpCodes.Callvirt) //set_Modulate
        ).ThrowIfInvalid("Could not find glow color assignment");

        codeMatcher.Advance(8);

        codeMatcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0), //this
            new CodeInstruction(OpCodes.Call, getCardNodeMethod),
            new CodeInstruction(OpCodes.Callvirt, getModelMethod),
            CodeInstruction.Call(() => PossiblyTweakColor(default, default))
        );

        return codeMatcher.Instructions();
    }

    static Color PossiblyTweakColor(Color oldColor, CardModel cardModel)
    {
        int calculatedBatteryCost = ChirpBatteryCostField.ChirpBatteryCost.Get(cardModel);
        if (calculatedBatteryCost > 0)
        {
            return chirpPlayableColor;
        }

        if (cardModel.Owner.Character is TheDisplaced && cardModel.Keywords.Contains(CardKeyword.Ethereal) &&
            !cardModel.CanonicalInstance.Keywords.Contains(CardKeyword.Ethereal))
        {
            return madeEtherealColor;
        }

        return oldColor;
    }
}