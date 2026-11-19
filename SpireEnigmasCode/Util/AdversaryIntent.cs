#region

using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Util;

/*
 * Helper class for sorting intents on the Adversary.
 * 
 */
public class AdversaryIntent
{
    public Func<Dictionary<Creature, int>, Task> Action;
    public AbstractIntent Intent;
    public bool IsTemporary;
    private string PowerName;
    private int PowerAmount;

    public AdversaryIntent(Func<Dictionary<Creature, int>, Task> action, AbstractIntent intent, bool isTemporary, string powerName = "", int powerAmount = 0)
    {
        Action = action;
        Intent = intent;
        IsTemporary = isTemporary;
        PowerName = powerName;
        PowerAmount = powerAmount;
    }

    public string GetStringRepresentation()
    {
        string locTableEntry = "";
        if (Intent is SingleAttackIntent)
        {
            locTableEntry = "singleAttack";
        }else if (Intent is MultiAttackIntent)
        {
            locTableEntry = "multiAttack";
        }else if (Intent is BuffIntent)
        {
            locTableEntry = "buff";
        }else if (Intent is DebuffIntent)
        {
            locTableEntry = "debuff";
        }

        LocString intentLocString = new LocString("intents", "SPIREENIGMAS-ADVERSARY" + "." + locTableEntry);
        intentLocString.Add("Damage", (Intent as AttackIntent)?.DamageCalc?.Invoke() ?? 0);
        intentLocString.Add("Hits", (Intent as AttackIntent)?.Repeats ?? 0);
        intentLocString.Add("PowerName", PowerName);
        intentLocString.Add("Amount", PowerAmount);

        return intentLocString.GetFormattedText();
    }
}
