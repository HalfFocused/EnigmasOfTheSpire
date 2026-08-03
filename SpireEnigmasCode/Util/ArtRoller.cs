using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpireEnigmas.SpireEnigmasCode.Util;

/*
 * Credit for this wonderful art roller script goes to tatietho (https://github.com/Ghost-sourceK/Kovan-Mod)
 * They gave general permission for its use in #sts2-modding
 */
public static class ArtRoller
{
    // Base folders that contain vanilla card artwork.
    private static readonly string[] BasePaths =
    {
        "res://images/packed/card_portraits/defect",
        "res://images/packed/card_portraits/ironclad",
        "res://images/packed/card_portraits/silent"
    };
    // Special-case mapping for cards that should ALWAYS use their canonical artwork.
    // This is used to avoid confusing players with randomized visuals for core cards
    private static readonly Dictionary<string, string> ReservedArts = new()
    {
        { "strike", "res://images/packed/card_portraits/ironclad/strike_ironclad.png" },
        { "defend", "res://images/packed/card_portraits/ironclad/defend_ironclad.png" }
    };
    // Explicit blacklist of vanilla artworks that should never be used.
    // These are typically cards with different scale in the images.
    private static readonly HashSet<string> ExcludedArts =
    [
        "biased_cognition.png",
        "quadcast.png",
        "break.png",
        "corruption.png",
        "suppress.png",
        "wraith_form.png"
    ];
    // Maps a card seed to a permanently assigned artwork.
    private static readonly Dictionary<string, string> AssignedArts = new();
    // Sequential index used to distribute artworks across unseen seeds.
    private static int _nextIndex;
    // Cached list of all available vanilla card artworks.
    private static List<string>? _cache;
    /// <summary>
    /// Loads all available card portrait images from the configured base folders.
    /// 
    /// The method:
    /// - scans each character folder
    /// - filters valid PNG assets
    /// - removes excluded/reserved entries
    /// - returns a unified list of usable artwork paths
    /// </summary>
    private static List<string> Load()
    {
        var list = new List<string>();
        
        foreach (var basePath in BasePaths)
        {
            // Get all files inside the current character folder
            var files = DirAccess.GetFilesAt(basePath);
            // Filter only PNG textures and convert them into valid resource paths
            list.AddRange(from file in files where file.Contains(".png") select file.Replace(".import", "") 
                into pngName where !ReservedArts.Values.Any(path => path.EndsWith(pngName)) where !ExcludedArts.Contains(pngName)
                select $"{basePath}/{pngName}");
        }

        return list;
    }
    /// <summary>
    /// Checks whether a card name matches a reserved vanilla artwork rule.
    /// 
    /// Reserved cards (e.g. Strike, Defend) are always mapped to fixed art
    /// to preserve clarity and player familiarity.
    /// </summary>
    private static bool TryGetReserved(string seed, out string path)
    {
        var lower = seed.ToLowerInvariant();

        foreach (var kv in ReservedArts.Where(kv => lower.Contains(kv.Key)))
        {
            path = kv.Value;
            return true;
        }

        path = "";
        return false;
    }
    /// <summary>
    /// Returns a deterministic artwork for a given card seed.
    /// 
    /// Rules:
    /// 1. Basic cards with reserved keywords always use fixed artwork.
    /// 2. All other cards are assigned a stable, non-repeating artwork.
    /// 3. Assignments persist across sessions via caching in AssignedArts.
    /// 4. A sequential index is used to distribute artworks evenly across new seeds.
    /// </summary>
    public static string Get(string seed, CardRarity rarity)
    {
        _cache ??= Load();
        
        if (rarity == CardRarity.Basic && TryGetReserved(seed, out var reserved))
            return reserved;

        if (_cache.Count == 0)
            return "res://images/packed/card_portraits/ironclad/strike_ironclad.png";
        
        if (AssignedArts.TryGetValue(seed, out var existing))
            return existing;
        
        string art = _cache[_nextIndex % _cache.Count];

        AssignedArts[seed] = art;
        _nextIndex++;

        return art;
    }
}