using BaseLib.Abstracts;
using TheDisplaced.TheDisplacedCode.Extensions;
using Godot;

namespace TheDisplaced.TheDisplacedCode.Character;

public class TheDisplacedPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => TheDisplaced.Color;
    

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}