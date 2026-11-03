#region

using BaseLib.Abstracts;
using Godot;
using SpireEnigmas.SpireEnigmasCode.Character.savant;
using SpireEnigmas.SpireEnigmasCode.Extensions;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Character.renegade;

public class TheRenegadePotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => TheSavant.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}