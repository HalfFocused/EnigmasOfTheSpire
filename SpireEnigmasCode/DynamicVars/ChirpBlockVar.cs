using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.DynamicVars;

public class ChirpBlockVar : DynamicVar
{
    public const string defaultName = "ChirpBlock";

    public ValueProp Props { get; set; }

    public ChirpBlockVar(Decimal block, ValueProp props)
        : base(defaultName, block)
    {
        Props = props;
    }

    public ChirpBlockVar(string name, Decimal block, ValueProp props)
        : base(name, block)
    {
        Props = props;
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        Decimal previewedBlock = BaseValue;
        EnchantmentModel enchantment = card.Enchantment;
        
        if (enchantment != null)
        {
            Decimal originalBlock2 = previewedBlock + enchantment.EnchantBlockAdditive(previewedBlock);
            previewedBlock = originalBlock2 * enchantment.EnchantBlockMultiplicative(originalBlock2);
            if (!card.IsEnchantmentPreview)
                EnchantedValue = previewedBlock;
        }

        if (runGlobalHooks)
        {
            /*
             * jank warning
             * i can't pass the card into this hook or it'll use player powers
             * but if i don't pass the card, it won't use enchantments.
             *
             * thus, we add the enchantments ourselves so we can safely not pass the card into the hook
             *
             * my biggest fear is that passing in this fake base value could cause problems but i can't foresee how :shrug:
             */
            decimal fakeBaseValue = BaseValue;
            if (enchantment != null)
            {
                fakeBaseValue += enchantment.EnchantBlockAdditive(fakeBaseValue);
                fakeBaseValue *= enchantment.EnchantBlockMultiplicative(fakeBaseValue);
            }
            
            previewedBlock = Hook.ModifyBlock(card.CombatState, EnigmasHelper.GetChirpFromPlayer(card.Owner), fakeBaseValue,
                Props, null, null, out IEnumerable<AbstractModel> _);
        }
        PreviewValue = previewedBlock;
    }
}