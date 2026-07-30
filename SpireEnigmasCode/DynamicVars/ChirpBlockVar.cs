using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode;
using SpireEnigmas.SpireEnigmasCode.Commands;

public class ChirpBlockVar : DynamicVar
{
    public const string defaultName = "ChirpBlock";

    public ValueProp Props { get; set; }

    public ChirpBlockVar(Decimal damage, ValueProp props)
        : base(defaultName, damage)
    {
        Props = props;
    }

    public ChirpBlockVar(string name, Decimal damage, ValueProp props)
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
        Decimal originalBlock1 = BaseValue;
        EnchantmentModel enchantment = card.Enchantment;
        if (enchantment != null)
        {
            Decimal originalBlock2 = originalBlock1 + enchantment.EnchantBlockAdditive(originalBlock1);
            originalBlock1 = originalBlock2 * enchantment.EnchantBlockMultiplicative(originalBlock2);
            if (!card.IsEnchantmentPreview)
                EnchantedValue = originalBlock1;
        }

        if (runGlobalHooks)
        {
            ICombatState combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            originalBlock1 = Hook.ModifyBlock(card.CombatState, ChirpCmd.GetChirpFromPlayer(card.Owner), BaseValue,
                Props, null, null, out IEnumerable<AbstractModel> _);
        }
        PreviewValue = originalBlock1;
    }
}