using MelonLoader;
using System.Collections.Concurrent;

namespace BloomEngine.Menu;

public static class ModMenu
{
    private static ConcurrentDictionary<string, ModEntry> mods = new ConcurrentDictionary<string, ModEntry>();

    /// <summary>
    /// Event invoked when a mod is registered to the Mod Menu
    /// </summary>
    public static event Action<ModEntry> OnModRegistered;

    public static ModEntry NewEntry(MelonMod mod, string id, string displayName = default)
        => new ModEntry(mod, id, displayName);

    /// <summary>
    /// Registers a mod entry with the mod menu and invoked the <see cref="OnModRegistered"/> event.
    /// </summary>
    internal static void Register(ModEntry entry)
    {
        mods[entry.Id] = entry;
        OnModRegistered?.Invoke(entry);

        Melon<BloomEnginePlugin>.Logger.Msg($"[ModMenu] Successfully registered {entry.DisplayName} with {entry.Properties.Count} config {(entry.Properties.Count > 1 ? "properties" : "property")}.");
        foreach (var prop in entry.Properties)
        {
            Melon<BloomEnginePlugin>.Logger.Msg($"    - {prop.Name} ({prop.InputType.ToString()})");
        }
    }
}
