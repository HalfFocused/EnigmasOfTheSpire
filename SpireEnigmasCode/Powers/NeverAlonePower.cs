#region

using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Monsters;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class NeverAlonePower : SpireEnigmaPower, IAddDumbVariablesToPowerDescription
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override bool ShouldPlayVfx => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("AdversaryMovePattern")
    ];

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature != Owner;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    private static string GetIntentString(string locEntry)
    {
        return new LocString("intents", "SPIREENIGMAS-ADVERSARY" + "." + locEntry).GetRawText();
    }
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        string formattedDescription = GetIntentString("header1");
        
        Adversary? adversary = Owner.Monster as Adversary;
        if(adversary is null) return;
        
        List<AdversaryIntent> intentsThisTurn = adversary.CurrentIntents;
        for (int i = 0; i < intentsThisTurn.Count; i++)
        {
            AdversaryIntent adversaryIntent = intentsThisTurn[i];
            formattedDescription += "\n" + adversaryIntent.GetStringRepresentation();
            
        }
        
        formattedDescription += GetIntentString("header2");
        
        for(int i = 0; i < adversary.MoveCycle.Count; i++)
        {
            intentsThisTurn = adversary.MoveCycle[i];
            for (int j = 0; j < intentsThisTurn.Count; j++)
            {
                AdversaryIntent adversaryIntent = intentsThisTurn[j];
                if (!adversaryIntent.IsTemporary)
                {
                    formattedDescription += "\n" + (i == adversary.MoveCyclePosition ? ">" : "-") + adversaryIntent.GetStringRepresentation();
                }
            }
        }
        
        description.Add("IntentCycle", formattedDescription);
    }
}