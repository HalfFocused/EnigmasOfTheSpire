using BaseLib.Abstracts;
using BaseLib.Utils;
using TheDisplaced.TheDisplacedCode.Character;

namespace TheDisplaced.TheDisplacedCode.Potions;

[Pool(typeof(TheDisplacedPotionPool))]
public abstract class TheDisplacedPotion : CustomPotionModel;