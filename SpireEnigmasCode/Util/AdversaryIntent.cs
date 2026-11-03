#region

using MegaCrit.Sts2.Core.Entities.Creatures;
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

    public AdversaryIntent(Func<Dictionary<Creature, int>, Task> action, AbstractIntent intent, bool isTemporary)
    {
        Action = action;
        Intent = intent;
        IsTemporary = isTemporary;
    }
}