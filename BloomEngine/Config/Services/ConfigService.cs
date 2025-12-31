using BloomEngine.ModMenu.Services;
using MelonLoader;

namespace BloomEngine.Config.Services;

/// <summary>
/// A static class which provides methods for creating input fields for a mod config menu.
/// The supported types are: <c>string, int, float, bool and enum</c>
/// </summary>
public static class ConfigService
{
    internal static MelonLogger.Instance ConfigLogger { get; } = new MelonLogger.Instance($"{nameof(BloomEngine)}.{nameof(Config)}");
    internal static Dictionary<ModMenuEntry, ModConfig> ModConfigs { get; } = new();
    
    private static ModConfig currentConfig;

    /// <summary>
    /// A value that indicates whether a mod config config is currently open.
    /// </summary>
    public static bool IsConfigPanelOpen => currentConfig is not null;

    /// <summary>
    /// Displays the config config for the specified mod if it is registered and no other configuration config is currently open.
    /// If the mod does not have a registered config config, a warning is logged.
    /// </summary>
    /// <param name="mod">The mod for which to display the configuration config. Must not be null.</param>
    public static void ShowConfigPanel(ModMenuEntry mod)
    {
        // Return if a panel is already open
        if (currentConfig is not null)
            return;

        // Log a warning if there is no config panel
        if (!ModConfigs.TryGetValue(mod, out var config))
        {
            ConfigLogger.Warning($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.");
            return;
        }

        config.ShowPanel();
        currentConfig = config;
    }

    /// <summary>
    /// Hides the currently displayed configuration config, if there is one.
    /// </summary>
    public static void HideConfigPanel()
    {
        if (currentConfig is null)
            return;

        currentConfig.HidePanel();
        currentConfig = null;
    }


    internal static void RegisterModConfig(ModConfig config)
    {
        if(config is null)
            return;

        if (ModConfigs.ContainsKey(config.ModEntry))
            ConfigLogger.Warning($"Mod {config.ModEntry.DisplayName} attempted to register a config multiple times. Overwriting previous mod config.");

        ModConfigs[config.ModEntry] = config;

        SaveModConfig(config, false);
    }

    internal static void SaveModConfig(ModConfig config, bool printMessage)
    {
        if (config is null)
            return;

        config.MelonCategory.SaveToFile(false);

        if(printMessage)
            ConfigLogger.Msg($"Updated mod config for {config.ModEntry.DisplayName} and saved preferences.");
    }
}