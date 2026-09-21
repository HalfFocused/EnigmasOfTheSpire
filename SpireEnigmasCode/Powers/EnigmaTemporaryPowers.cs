using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;
using SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.common;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class AcquiescePower : CustomTemporaryPowerModelWrapper<Acquiesce, FealtyPower> {}

public class MalicePower : CustomStoryTemporaryPowerModelWrapper<Malice, DexterityPower> {}

public class IncitePower : CustomStoryTemporaryPowerModelWrapper<Incite, StrengthPower> {}

public class RepressPower : CustomStoryTemporaryPowerModelWrapper<Repress, StrengthPower> {}

public class EpiphanyStrengthPower : CustomStoryTemporaryPowerModelWrapper<Epiphany, StrengthPower> {}

public class EpiphanyDexterityPower : CustomStoryTemporaryPowerModelWrapper<Epiphany, DexterityPower> {}