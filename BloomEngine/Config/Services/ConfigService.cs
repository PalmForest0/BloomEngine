using BloomEngine.Config.Internal;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using MelonLoader;

namespace BloomEngine.Config.Services;

/// <summary>
/// A static class which provides methods for creating input fields for a mod config menu.
/// The supported types are: <c>string, int, float, bool and enum</c>
/// </summary>
public static class ConfigService
{
    private static readonly Dictionary<ModMenuEntry, ConfigPanel> configs = new();
    private static readonly MelonLogger.Instance logger = new MelonLogger.Instance($"{nameof(BloomEngine)}.{nameof(Config)}");
    
    private static ConfigPanel currentConfigPanel;

    /// <summary>
    /// A value that indicates whether a mod config panel is currently open.
    /// </summary>
    public static bool IsConfigPanelOpen => currentConfigPanel is not null;

    /// <summary>
    /// Displays the config panel for the specified mod if it is registered and no other configuration panel is currently open.
    /// If the mod does not have a registered config panel, a warning is logged.
    /// </summary>
    /// <param name="mod">The mod for which to display the configuration panel. Must not be null.</param>
    public static void ShowConfigPanel(ModMenuEntry mod)
    {
        if (currentConfigPanel is not null)
            return;

        if (configs.TryGetValue(mod, out var panel))
        {
            panel.ShowPanel();
            currentConfigPanel = panel;
        }
        else logger.Warning($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.");
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


    internal static void RegisterConfigPanel(ConfigPanel panel)
    {
        if(configs.ContainsKey(panel.Mod))
            logger.Warning($"Mod {panel.Mod.DisplayName} attempted to register multiple config panels. Overwriting previous panel.", "Config");

        configs[panel.Mod] = panel;
    }

    internal static void RegisterModConfig(ModConfig config) => SaveModConfig(config, false);

    internal static void SaveModConfig(ModConfig config, bool printMessage)
    {
        if (config is null)
            return;

        config.MelonCategory.SaveToFile(false);

        if(printMessage)
            logger.Msg($"Updated mod config for {config.ModMenuEntry.DisplayName} and saved MelonPreferences.");
    }
}