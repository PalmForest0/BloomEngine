using MelonLoader;

namespace BloomEngine.ModMenu.Services;

/// <summary>
/// A static class responsible for registering mod entries and adding them to the mod menu.
/// </summary>
public static class ModMenuService
{
    internal static MelonLogger.Instance ModMenuLogger { get; } = new MelonLogger.Instance($"{nameof(BloomEngine)}.{nameof(ModMenu)}");
    internal static Dictionary<MelonMod, ModMenuEntry> ModEntries { get; } = new();  

    /// <summary>
    /// Event that is invoked when a mod is added to the mod menu using <see cref="ModMenuEntry.Register"/>."/>
    /// </summary>
    public static event Action<ModMenuEntry> OnModRegistered;

    /// <summary>
    /// Creates a new mod entry which can be customised and added to the mod menu with <see cref="ModMenuEntry.Register"/>.
    /// </summary>
    /// <param name="mod">The mod this entry belongs to.</param>
    /// <returns>A new mod entry for the given mod</returns>
    public static ModMenuEntry CreateEntry(MelonMod mod)
    {
        if(!ModEntries.ContainsKey(mod))
            return new ModMenuEntry(mod);

        ModMenuLogger.Warning($"Failed to create a mod menu entry for {mod.Info.Name} since one has already been created.");
        return null;
    }

    /// <summary>
    /// Registers a mod entry with the mod menu and invoked the <see cref="OnModRegistered"/> event.
    /// </summary>
    internal static void RegisterModEntry(ModMenuEntry entry)
    {
        ModEntries[entry.Mod] = entry;
        OnModRegistered?.Invoke(entry);

        ModMenuLogger.Msg($"Successfully added {entry.DisplayName} to the mod menu.");
    }
}