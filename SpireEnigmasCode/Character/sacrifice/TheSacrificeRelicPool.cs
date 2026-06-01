using BaseLib.Abstracts;
using Godot;
using SpireEnigmas.SpireEnigmasCode.Extensions;

namespace SpireEnigmas.SpireEnigmasCode.Character.sacrifice;

public class TheSacrificeRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => TheSacrifice.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}