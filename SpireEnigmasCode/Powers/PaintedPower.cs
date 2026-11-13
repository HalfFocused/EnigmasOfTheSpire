#region

using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Monsters;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class PaintedPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private Color _uniqueColor;

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        _uniqueColor = GetRainbowColorByTime();
        NCreature? targetNode = NCombatRoom.Instance?.GetCreatureNode(Owner);
        if (targetNode is not null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSplashVfx.Create(targetNode.VfxSpawnPosition, _uniqueColor));
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || (!props.IsPoweredAttack() && dealer?.Monster is not Adversary) || result.TotalDamage < Amount) return;
        
        NCreature targetNode = NCombatRoom.Instance.GetCreatureNode(Owner);
        if (targetNode != null)
        {
            await Cmd.Wait(0.5f);
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(NSplashVfx.Create(targetNode.VfxSpawnPosition, _uniqueColor));
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(NLiquidOverlayVfx.Create(Owner, _uniqueColor));
            //NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, cardSource._vfxTint));
        }
        
        
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.Remove(this);

        InvokeDisplayAmountChanged();
    }

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        return [new(Amount, _uniqueColor, HealthBarForecastDirection.FromLeft)];
    }
    
    public Color GetRainbowColorByTime()
    {
        double currentTime = Time.GetUnixTimeFromSystem();

        // 2. Calculate the hue (0.0 to 1.0) based on time and speed
        // Mathf.PosMod ensures the value safely wraps around from 1.0 back to 0.0
        float hue = (float)Mathf.PosMod(currentTime, 1.0);

        // 3. Return the Color using HSV (Hue, Saturation, Value)
        return Color.FromHsv(hue, 1.0f, 1.0f);
    }
}