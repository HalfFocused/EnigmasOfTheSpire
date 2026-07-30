using BaseLib.Abstracts;
using Godot;
using SpireEnigmas.SpireEnigmasCode.Character.sacrifice;
using SpireEnigmas.SpireEnigmasCode.Extensions;

namespace SpireEnigmas.SpireEnigmasCode.Character.savant;

public class TheSavantRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => TheSacrifice.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}