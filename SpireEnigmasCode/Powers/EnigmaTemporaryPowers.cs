using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.common;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class AcquiescePower : CustomTemporaryPowerModelWrapper<Acquiesce, FealtyPower> {}

public class IncitePower : CustomStoryTemporaryPowerModelWrapper<Incite, StrengthPower> {}

public class RepressPower : CustomStoryTemporaryPowerModelWrapper<Repress, StrengthPower> {}