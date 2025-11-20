using BloomEngine.Config;
using MelonLoader;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using UnityEngine;

namespace BloomEngine.Menu;

public static class ModMenu
{
    private static ConcurrentDictionary<string, ModEntry> mods = new ConcurrentDictionary<string, ModEntry>();
    private static Dictionary<ModEntry, ConfigPanel> configPanels = new Dictionary<ModEntry, ConfigPanel>();
    private static ConfigPanel currentConfigPanel = null;

    /// <summary>
    /// A read-only dictionary of all registered mods in the Mod Menu, indexed by their ID.
    /// </summary>
    public static ReadOnlyCollection<ModEntry> Mods => new ReadOnlyCollection<ModEntry>(mods.Values.ToArray());

    /// <summary>
    /// A read-only dictionary that maps mod entries to their corresponding configuration panels, if registered.
    /// </summary>
    public static ReadOnlyDictionary<ModEntry, ConfigPanel> ConfigPanels => new ReadOnlyDictionary<ModEntry, ConfigPanel>(configPanels);

    /// <summary>
    /// Event invoked when a mod is registered to the Mod Menu
    /// </summary>
    public static event Action<ModEntry> OnModRegistered;

    public static ModEntry NewEntry(MelonMod mod, string id, string displayName = default)
        => new ModEntry(mod, id, displayName);

    internal static void RegisterConfigPanel(ConfigPanel panel) => configPanels[panel.Mod] = panel;

    public static void ShowConfigPanel(ModEntry mod)
    {
        if (currentConfigPanel is not null)
            return;

        if (configPanels.TryGetValue(mod, out var panel))
        {
            panel.ShowPanel();
            currentConfigPanel = panel;
        }
        else Log($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.", LogType.Warning);
    }

    public static void HideConfigPanel()
    {
        if (currentConfigPanel is null)
            return;

        currentConfigPanel.HidePanel();
        currentConfigPanel = null;
    }

    /// <summary>
    /// Registers a mod entry with the mod menu and invoked the <see cref="OnModRegistered"/> event.
    /// </summary>
    internal static void Register(ModEntry entry)
    {
        mods[entry.Id] = entry;

        if (OnModRegistered is not null)
            OnModRegistered.Invoke(entry);

        if (entry.Config is null)
        {
            Log($"Successfully registered {entry.DisplayName} with no config properties.");
            return;
        }

        ModMenu.Log($"Successfully registered {entry.DisplayName} with {entry.Config.Properties.Count} config {(entry.Config.Properties.Count > 1 ? "properties" : "property")}.");
        foreach (var prop in entry.Config.Properties)
        {
            if (prop is null)
                continue;

            ModMenu.Log($"    - {prop.Name} ({prop.ValueType.ToString()})");
        }
    }

    /// <summary>
    /// Logs a message to the MelonLoader console with the ModMenu prefix.
    /// Uses Unity's LogType enum because I can't be bothered to make my own XD.
    /// Defaults to Log, can also use Warning or Error.
    /// </summary>
    internal static void Log(string text, LogType type = LogType.Log)
    {
        switch (type)
        {
            case LogType.Warning:
                Melon<BloomEnginePlugin>.Logger.Warning($"[ModMenu] {text}");
                break;
            case LogType.Error:
                Melon<BloomEnginePlugin>.Logger.Error($"[ModMenu] {text}");
                break;
            default:
                Melon<BloomEnginePlugin>.Logger.Msg($"[ModMenu] {text}");
                break;
        }
    }
}
