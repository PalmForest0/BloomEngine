using BloomEngine.Config;
using MelonLoader;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using UnityEngine;

namespace BloomEngine.Menu;

public static class ModMenu
{
    private static ConcurrentDictionary<string, ModEntry> mods = new ConcurrentDictionary<string, ModEntry>();
    private static Dictionary<ModEntry, ConfigPanel> configs = new Dictionary<ModEntry, ConfigPanel>();
    private static ConfigPanel currentConfigPanel = null;

    /// <summary>
    /// A read-only dictionary of all registered mod entries in the Mod Menu, indexed by their mod name.
    /// </summary>
    public static ReadOnlyDictionary<string, ModEntry> Mods => new ReadOnlyDictionary<string, ModEntry>(mods);

    /// <summary>
    /// Event invoked when a mod is registered to the Mod Menu
    /// </summary>
    public static event Action<ModEntry> OnModRegistered;

    public static ModEntry NewEntry(MelonMod mod, string displayName = null)
        => new ModEntry(mod, displayName ?? mod.Info.Name);

    internal static void RegisterConfigPanel(ConfigPanel panel) => configs[panel.Mod] = panel;

    public static void ShowConfigPanel(ModEntry mod)
    {
        if (currentConfigPanel is not null)
            return;

        if (configs.TryGetValue(mod, out var panel))
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
        mods[entry.Mod.Info.Name] = entry;

        if (OnModRegistered is not null)
            OnModRegistered.Invoke(entry);

        if (!entry.HasConfig)
        {
            Log($"Successfully registered {entry.DisplayName} with the mod menu.");
            return;
        }

        // List all registered input fields
        ModMenu.Log($"Successfully registered {entry.DisplayName} with {entry.ConfigInputFields.Count} config input field{(entry.ConfigInputFields.Count > 1 ? "s" : "")}.");
        foreach (var field in entry.ConfigInputFields)
            ModMenu.Log($"    - {field.Name} ({field.ValueType.ToString()})");
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
                Melon<BloomEngineMod>.Logger.Warning($"[ModMenu] {text}");
                break;
            case LogType.Error:
                Melon<BloomEngineMod>.Logger.Error($"[ModMenu] {text}");
                break;
            default:
                Melon<BloomEngineMod>.Logger.Msg($"[ModMenu] {text}");
                break;
        }
    }
}
