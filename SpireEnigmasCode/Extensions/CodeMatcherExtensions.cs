using HarmonyLib;

namespace SpireEnigmas.SpireEnigmasCode.Extensions;

public static class CodeMatcherExtensions
{
    public static CodeMatcher CurrentPosWithOffset(this CodeMatcher matcher, int offset, out int pos)
    {
        pos = matcher.Pos + offset;
        return matcher;
    }

    public static CodeMatcher CurrentPos(this CodeMatcher matcher, out int pos) => matcher.CurrentPosWithOffset(0, out pos);
}