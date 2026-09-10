using System.Runtime.Serialization;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;

using System.Globalization;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;
using SpireEnigmas.SpireEnigmasCode.Patches;

#nullable enable
namespace SpireEnigmas.SpireEnigmasCode.DynamicVars;


public class RefinementAmountFormatter : IAutoRegisterFormatSpecifier 
{
  public string Name
  {
    get => "refinementAmount";
    set => throw new NotImplementedException();
  }

  public bool CanAutoDetect { get; set; }

  public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
  {
    string str1 = (string) null;
    String result;
    switch (formattingInfo.CurrentValue)
    {
      case DynamicVar dynamicVar:
        result = Convert.ToString(RefinementFields.RefineBy.Get(dynamicVar));
        break;
      default:
        throw new LocException($"SpireEnigmas localization no worky");
    }
    formattingInfo.Write(result);
    return true;
  }
}
