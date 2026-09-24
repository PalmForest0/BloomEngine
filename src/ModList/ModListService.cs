using System.Collections;
using BloomEngine.Core;
using BloomEngine.ModList.UI;
using BloomEngine.UI;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;

namespace BloomEngine.ModList;

/// <summary>
/// A static class responsible for registering mod entries and adding them to the mod list.
/// </summary>
public static class ModListService
{
    /// <summary>
    /// Specifies the prefix to use for all log messages from this service.
    /// </summary>
    internal const string LogPrefix = $"[{nameof(ModListService)}] ";

    internal static Dictionary<MelonMod, ModListEntry> ModEntries { get; } = new();
    internal static IEnumerable<ModListEntry> RegisteredEntries => ModEntries.Values;

    /// <summary>
    /// Creates a new mod entry which can be customised and added to the mod list with <see cref="ModListEntry.Register"/>.
    /// </summary>
    /// <param name="mod">The mod this entry belongs to.</param>
    /// <returns>A new <see cref="ModListEntry"/> for the given mod, or the current one if it already exists.</returns>
    public static ModListEntry CreateEntry(MelonMod mod)
    {
        if(!ModEntries.TryGetValue(mod, out var entry))
            return new ModListEntry(mod);
            
        BloomLogger.Warn($"Encountered duplicate CreateEntry() call for {mod.Info.Name}, returning existing {nameof(ModListEntry)} instance.", LogPrefix);
        return entry;
    }

    /// <summary>
    /// Waits for the UIHelper to load all templates, then creates the mod list UI from the existing achievements UI.
    /// </summary>
    internal static IEnumerator Co_CreateModList(AchievementsUI achievementsUI)
    {
        yield return new WaitUntil((Il2CppSystem.Func<bool>)(() => UIHelper.AllTemplatesLoaded));

        BloomLogger.Info("All UI templates loaded, creating mod list.", LogPrefix);
        ModListUI.Create(achievementsUI);
    }
}