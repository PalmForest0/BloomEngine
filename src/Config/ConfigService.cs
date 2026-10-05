using BloomEngine.Config.Fields;
using BloomEngine.Config.UI;
using BloomEngine.Core;
using BloomEngine.Extensions;
using BloomEngine.ModList;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Object = UnityEngine.Object;

namespace BloomEngine.Config;

/// <summary>
/// A static class which provides methods for creating config fields.
/// The currently supported types are: <see cref="string"/>, <see cref="int"/>, <see cref="float"/>, <see cref="bool"/> and <see cref="Enum"/>.
/// </summary>
public static class ConfigService
{
    /// <summary>
    /// Specifies the prefix to use for all log messages from this service.
    /// </summary>
    internal const string LogPrefix = $"[{nameof(ConfigService)}] ";

    /// <summary>
    /// The config panel UI that is currently open.
    /// </summary>
    private static ConfigPanel? _currentPanel;

    private static bool _panelsCreated;

    /// <summary>
    /// A value that indicates whether a mod config is currently open.
    /// </summary>
    public static bool IsConfigPanelOpen => _currentPanel is not null;

    /// <summary>
    /// Creates a <see cref="StringConfigField"/> instance which represents a textbox. To add this field to your config,
    /// pass it to <see cref="ModListEntry.AddConfigFields"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModListEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    /// <param name="description">The description popup shown for this config field in the config panel.</param>
    /// <returns>
    /// A <see cref="StringConfigField"/> instance which can be passed to <see cref="ModListEntry.AddConfigFields"/>
    /// to add it to your mod's config.<br/>You can store this field instance and access its value using <see cref="ConfigField{T,TSelf}.Value"/>
    /// </returns>
    public static StringConfigField CreateString(string name, string defaultValue, string? description = null) => new(name, defaultValue, description);

    /// <summary>
    /// Creates an <see cref="IntConfigField"/> instance which represents a numeric textbox. To add this field to your config,
    /// pass it to <see cref="ModListEntry.AddConfigFields"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModListEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    /// <param name="description">The description popup shown for this config field in the config panel.</param>
    /// <returns>
    /// An <see cref="IntConfigField"/> instance which can be passed to <see cref="ModListEntry.AddConfigFields"/>
    /// to add it to your mod's config.<br/>You can store this field instance and access its value using <see cref="ConfigField{T,TSelf}.Value"/>
    /// </returns>
    public static IntConfigField CreateInt(string name, int defaultValue, string? description = null) => new(name, defaultValue, description);

    /// <summary>
    /// Creates a <see cref="FloatConfigField"/> instance which represents a slider. To add this field to your config,
    /// pass it to <see cref="ModListEntry.AddConfigFields"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModListEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    /// <param name="description">The description popup shown for this config field in the config panel.</param>
    /// <returns>
    /// A <see cref="FloatConfigField"/> instance which can be passed to <see cref="ModListEntry.AddConfigFields"/>
    /// to add it to your mod's config.<br/>You can store this field instance and access its value using <see cref="ConfigField{T,TSelf}.Value"/>
    /// </returns>
    public static FloatConfigField CreateFloat(string name, float defaultValue, string? description = null) => new(name, defaultValue, description);

    /// <summary>
    /// Creates a <see cref="BoolConfigField"/> instance which represents a checkbox. To add this field to your config,
    /// pass it to <see cref="ModListEntry.AddConfigFields"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModListEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    /// <param name="description">The description popup shown for this config field in the config panel.</param>
    /// <returns>
    /// A <see cref="BoolConfigField"/> instance which can be passed to <see cref="ModListEntry.AddConfigFields(ConfigFieldBase[])"/>
    /// to add it to your mod's config.<br/>You can store this field instance and access its value using <see cref="ConfigField{T,TSelf}.Value"/>
    /// </returns>
    public static BoolConfigField CreateBool(string name, bool defaultValue, string? description = null) => new(name, defaultValue, description);

    /// <summary>
    /// Creates an <see cref="EnumConfigField{TEnum}"/> instance which represents a dropdown. To add this field to your config,
    /// pass it to <see cref="ModListEntry.AddConfigFields"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModListEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    /// <param name="description">The description popup shown for this config field in the config panel.</param>
    /// <returns>
    /// An <see cref="EnumConfigField{TEnum}"/> instance which can be passed to <see cref="ModListEntry.AddConfigFields"/>
    /// to add it to your mod's config.<br/>You can store this field instance and access its value using <see cref="ConfigField{T,TSelf}.Value"/>
    /// </returns>
    public static EnumConfigField<TEnum> CreateEnum<TEnum>(string name, TEnum defaultValue, string? description = null) where TEnum : Enum => new(name, defaultValue, description);

    /// <summary>
    /// Displays the config panel for the specified mod if it is registered and no other configuration config is currently open.
    /// If the mod does not have a registered config, a warning is logged.
    /// </summary>
    /// <param name="mod">The mod for which to display the configuration config. Must not be null.</param>
    public static void ShowConfigPanel(ModListEntry mod)
    {
        // Return if a panel is already open
        if (IsConfigPanelOpen)
            return;

        // Log a warning if there is no config registered
        if (mod.Config is null || mod.Config.IsEmpty)
        {
            BloomLogger.Warn($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.", LogPrefix);
            return;
        }

        if(mod.Config.Panel is null)
        {
            BloomLogger.Error($"Failed to open mod config panel for {mod.DisplayName}: Config UI panel has not been created.", LogPrefix);
            return;
        }

        mod.Config.Panel.ShowPanel();
        _currentPanel = mod.Config.Panel;
    }

    /// <summary>
    /// Hides the currently displayed configuration config, if there is one.
    /// </summary>
    public static void HideConfigPanel()
    {
        if (_currentPanel is null)
            return;

        _currentPanel.HidePanel();
        _currentPanel = null;
    }

    /// <summary>
    /// Clones an existing panel for every registered mod with a config and creates a new ConfigPanel for it.
    /// Does not run if panels have already been created, or one of the provided parameters is null.
    /// </summary>
    internal static void TryCreateConfigPanels(MainMenuPanelView? mainMenu, PanelViewContainer? globalPanels)
    {
        if (_panelsCreated || mainMenu.IsNull() || globalPanels.IsNull())
            return;

        const string templatePanelId = "quit";
        var template = mainMenu.GetComponentInParent<PanelViewContainer>().m_panels.FirstOrDefault(p => p.m_id == templatePanelId);

        if (template.IsNull())
        {
            BloomLogger.Error($"Failed to create config panels: Unable to find template panel with id \"{templatePanelId}\".", LogPrefix);
            return;
        }

        // Create a config panel for each mod entry with a registered config
        foreach (var config in ModListService.RegisteredEntries.Where(e => e.HasConfigFields).Select(e => e.Config))
        {
            var panelObj = Object.Instantiate(template.gameObject, globalPanels.transform);
            config!.Panel = new ConfigPanel(panelObj.GetComponent<PanelView>(), config);
        }

        _panelsCreated = true;
    }
}