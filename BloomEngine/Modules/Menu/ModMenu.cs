using BloomEngine.Modules.Config;
using MelonLoader;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using UnityEngine;

namespace BloomEngine.Modules.Menu;

/// <summary>
/// A static class responsible for registering mod entries and adding them to the mod menu.
/// </summary>
public static class ModMenu
{
    private static Dictionary<MelonMod, ModEntry> entries = new();
    private static Dictionary<ModEntry, ConfigPanel> configs = new();
    private static ConfigPanel currentConfigPanel;

    /// <summary>
    /// A value that indicates whether a mod config panel is currently open.
    /// </summary>
    public static bool IsConfigPanelOpen => currentConfigPanel is not null;

    /// <summary>
    /// A read-only dictionary of all registered mod entries in the Mod Menu, indexed by their associated MelonMod.
    /// </summary>
    public static ReadOnlyDictionary<MelonMod, ModEntry> Entries => new ReadOnlyDictionary<MelonMod, ModEntry>(entries);

    /// <summary>
    /// Event that is invoked when a mod is added to the mod menu using <see cref="ModEntry.Register"/>."/>
    /// </summary>
    public static event Action<ModEntry> OnModRegistered;

    /// <summary>
    /// Creates a new mod entry which can be customised and added to the mod menu with <see cref="ModEntry.Register"/>.
    /// </summary>
    /// <param name="mod">The mod this entry belongs to.</param>
    /// <returns>A new mod entry for the given mod</returns>
    public static ModEntry CreateEntry(MelonMod mod)
    {
        if(entries.ContainsKey(mod))
        {
            LogInfo($"Cannot create an entry for the mod {mod.Info.Name} since one has already been created.");
            return null;
        }

        return new ModEntry(mod);
    }

    /// <summary>
    /// Displays the config panel for the specified mod if it is registered and no other configuration panel is currently open.
    /// If the mod does not have a registered config panel, a warning is logged.
    /// </summary>
    /// <param name="mod">The mod for which to display the configuration panel. Must not be null.</param>
    public static void ShowConfigPanel(ModEntry mod)
    {
        if (currentConfigPanel is not null)
            return;

        if (configs.TryGetValue(mod, out var panel))
        {
            panel.ShowPanel();
            currentConfigPanel = panel;
        }
        else LogWarning($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.");
    }

    /// <summary>
    /// Hides the currently displayed configuration panel, if there is one.
    /// </summary>
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
    internal static void RegisterModEntry(ModEntry entry)
    {
        entries[entry.Mod] = entry;
        OnModRegistered?.Invoke(entry);

        LogInfo($"Successfully added {entry.DisplayName} to the mod menu.");
    }

    internal static void RegisterConfigPanel(ConfigPanel panel) => configs[panel.Mod] = panel;

    /// <summary>
    /// Logs a message to the console with the <c>[ModMenu]</c> prefix.
    /// </summary>
    internal static void LogInfo(object value) => MelonLogger.Msg($"[ModMenu] {value.ToString()}");

    /// <summary>
    /// Logs a warning to the console with the <c>[ModMenu]</c> prefix.
    /// </summary>
    internal static void LogWarning(object value) => MelonLogger.Warning($"[ModMenu] {value.ToString()}");

    /// <summary>
    /// Logs an error to the console with the <c>[ModMenu]</c> prefix.
    /// </summary>
    internal static void LogError(object value) => MelonLogger.Error($"[ModMenu] {value.ToString()}");
}
