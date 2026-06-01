using BaseLib.Abstracts;
using BaseLib.Utils;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;

namespace SpireEnigmas.SpireEnigmasCode.Potions;

[Pool(typeof(TheDisplacedPotionPool))]
public abstract class TheDisplacedPotion : CustomPotionModel;