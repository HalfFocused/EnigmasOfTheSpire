using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode;
using SpireEnigmas.SpireEnigmasCode.Commands;

public class ChirpDamageVar : DynamicVar
{
    public const string defaultName = "ChirpDamage";

    public ValueProp Props { get; set; }

    public ChirpDamageVar(Decimal damage, ValueProp props)
        : base(defaultName, damage)
    {
        Props = props;
    }

    public ChirpDamageVar(string name, Decimal damage, ValueProp props)
        : base(name, damage)
    {
        Props = props;
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        Decimal originalDamage1 = BaseValue;
        EnchantmentModel enchantment = card.Enchantment;
        if (enchantment != null)
        {
            Decimal originalDamage2 = originalDamage1 + enchantment.EnchantDamageAdditive(originalDamage1, Props);
            originalDamage1 = originalDamage2 * enchantment.EnchantDamageMultiplicative(originalDamage2, Props);
            if (!card.IsEnchantmentPreview)
                EnchantedValue = originalDamage1;
        }
        if (runGlobalHooks)
        {
            ICombatState combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            originalDamage1 = Hook.ModifyDamage(card.Owner.RunState, combatState, target, ChirpCmd.GetChirpFromPlayer(card.Owner), BaseValue, Props, card, null, ModifyDamageHookType.All, previewMode, out IEnumerable<AbstractModel> _);
        }
        PreviewValue = originalDamage1;
    }
}