using BloomEngine.Core;
using BloomEngine.ModMenu.UI;
using BloomEngine.UI;
using Il2CppUI.Scripts;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace BloomEngine.ModMenu;

/// <summary>
/// A static class responsible for registering mod entries and adding them to the mod menu.
/// </summary>
public static class ModMenuService
{
    /// <summary>
    /// Specifies the prefix to use for all log messages from this service.
    /// </summary>
    internal const string LogPrefix = $"[{nameof(ModMenuService)}] ";

    internal static Dictionary<MelonMod, ModMenuEntry> ModEntries { get; } = new();
    internal static IEnumerable<ModMenuEntry> RegisteredEntries => ModEntries.Values;

    /// <summary>
    /// Creates a new mod entry which can be customised and added to the mod menu with <see cref="ModMenuEntry.Register"/>.
    /// </summary>
    /// <param name="mod">The mod this entry belongs to.</param>
    /// <returns>A new ModMenuEntry for the given mod, or the current one if it already exists.</returns>
    public static ModMenuEntry CreateEntry(MelonMod mod)
    {
        if(!ModEntries.TryGetValue(mod, out var entry))
            return new ModMenuEntry(mod);
            
        BloomLogger.Warn($"Encountered duplicate CreateEntry() call for {mod.Info.Name}, returning existing ModMenuEntry instance.", LogPrefix);
        return entry;
    }

    /// <summary>
    /// Waits for the UIHelper to load all templates, then creates the mod menu UI off of the existing achievements UI.
    /// </summary>
    internal static IEnumerator Co_CreateModMenu(AchievementsUI achievementsUI)
    {
        yield return new WaitUntil((Il2CppSystem.Func<bool>)(() => UIHelper.AllTemplatesLoaded));

        BloomLogger.Info("All UI templates loaded, creating mod menu.", LogPrefix);
        ModMenuUI.Create(achievementsUI);
    }
}