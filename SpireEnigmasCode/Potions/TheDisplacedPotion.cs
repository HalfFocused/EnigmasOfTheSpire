#region

using BaseLib.Abstracts;
using BaseLib.Utils;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Potions;

[Pool(typeof(TheDisplacedPotionPool))]
public abstract class TheDisplacedPotion : CustomPotionModel;