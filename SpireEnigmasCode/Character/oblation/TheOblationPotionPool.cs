using BaseLib.Abstracts;
using Godot;
using SpireEnigmas.SpireEnigmasCode.Extensions;

namespace SpireEnigmas.SpireEnigmasCode.Character.oblation;

public class TheOblationPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => TheOblation.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}