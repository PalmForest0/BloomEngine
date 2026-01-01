using BloomEngine.Config.Inputs;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using MelonLoader;

namespace BloomEngine.Config.Services;

/// <summary>
/// A static class which provides methods for creating config inputs.
/// The supported types are: <see cref="string"/>, <see cref="int"/>, <see cref="float"/>, <see cref="bool"/> and <see cref="Enum"/>.
/// </summary>
public static class ConfigService
{
    internal static MelonLogger.Instance ConfigLogger { get; } = new MelonLogger.Instance($"{nameof(BloomEngine)}.{nameof(Config)}");
    
    /// <summary>
    /// The config panel UI that is currently open.
    /// </summary>
    private static ConfigPanel currentPanel;

    /// <summary>
    /// A value that indicates whether a mod config config is currently open.
    /// </summary>
    public static bool IsConfigPanelOpen => currentPanel is not null;

    /// <summary>
    /// Creates a <see cref="StringConfigInput"/> instance which represents a textbox. To add this input to your config,
    /// pass it to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModMenuEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">The display name of this config input, which will be displayed in the config menu.</param>
    /// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">The default <see cref="string"/> value of this config input.</param>
    /// <param name="options">An optional object through which additional logic can be added to this config input.</param>
    /// <returns>A <see cref="StringConfigInput"/> instance which can be passed to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>
    /// to add it to your mod's config.<br/> You can also store this config input and access its value using <see cref="TypedConfigInput{T}.Value"/></returns>
    public static StringConfigInput CreateString(string name, string description, string defaultValue, ConfigInputOptions<string> options = null)
        => new StringConfigInput(name, description, defaultValue, options);

    /// <summary>
    /// Creates an <see cref="IntConfigInput"/> instance which represents a numeric textbox. To add this input to your config,
    /// pass it to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModMenuEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">The display name of this config input, which will be displayed in the config menu.</param>
    /// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">The default <see cref="int"/> value of this config input.</param>
    /// <param name="options">An optional object through which additional logic can be added to this config input.</param>
    /// <returns>An <see cref="IntConfigInput"/> instance which can be passed to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>
    /// to add it to your mod's config.<br/> You can also store this config input and access its value using <see cref="TypedConfigInput{T}.Value"/></returns>
    public static IntConfigInput CreateInt(string name, string description, int defaultValue, ConfigInputOptions<int> options = null)
        => new IntConfigInput(name, description, defaultValue, options);

    /// <summary>
    /// Creates a <see cref="FloatConfigInput"/> instance which represents a slider. To add this input to your config,
    /// pass it to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModMenuEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">The display name of this config input, which will be displayed in the config menu.</param>
    /// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">The default <see cref="float"/> value of this config input.</param>
    /// <param name="minValue">The <strong>minimum</strong> value constraint of this <see cref="float"/> input slider.</param>
    /// <param name="maxValue">The <strong>maximum</strong> value constraint of this <see cref="float"/> input slider.</param>
    /// <param name="options">An optional object through which additional logic can be added to this config input.</param>
    /// <returns>A <see cref="FloatConfigInput"/> instance which can be passed to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>
    /// to add it to your mod's config.<br/> You can also store this config input and access its value using <see cref="TypedConfigInput{T}.Value"/></returns>
    public static FloatConfigInput CreateFloat(string name, string description, float defaultValue, float minValue, float maxValue, ConfigInputOptions<float> options = null)
        => new FloatConfigInput(name, description, defaultValue, minValue, maxValue, options);

    /// <summary>
    /// Creates a <see cref="BoolConfigInput"/> instance which represents a checkbox. To add this input to your config,
    /// pass it to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModMenuEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">The display name of this config input, which will be displayed in the config menu.</param>
    /// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">The default <see cref="bool"/> value of this config input.</param>
    /// <param name="options">An optional object through which additional logic can be added to this config input.</param>
    /// <returns>A <see cref="BoolConfigInput"/> instance which can be passed to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>
    /// to add it to your mod's config.<br/> You can also store this config input and access its value using <see cref="TypedConfigInput{T}.Value"/></returns>
    public static BoolConfigInput CreateBool(string name, string description, bool defaultValue, ConfigInputOptions<bool> options = null)
        => new BoolConfigInput(name, description, defaultValue, options);

    /// <summary>
    /// Creates an <see cref="EnumConfigInput"/> instance which represents a dropdown. To add this input to your config,
    /// pass it to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/><br/> or make it publicly accessible
    /// in a static class and use <see cref="ModMenuEntry.AddConfigClass(Type)"/> instead.
    /// </summary>
    /// <param name="name">The display name of this config input, which will be displayed in the config menu.</param>
    /// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">The default <see cref="Enum"/> value of this config input.</param>
    /// <param name="options">An optional object through which additional logic can be added to this config input.</param>
    /// <returns>An <see cref="EnumConfigInput"/> instance which can be passed to <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>
    /// to add it to your mod's config.<br/> You can also store this config input and access its value using <see cref="TypedConfigInput{T}.Value"/></returns>
    public static EnumConfigInput CreateEnum(string name, string description, Enum defaultValue, ConfigInputOptions<Enum> options = null)
        => new EnumConfigInput(name, description, defaultValue, options);

    /// <summary>
    /// Displays the config config for the specified mod if it is registered and no other configuration config is currently open.
    /// If the mod does not have a registered config config, a warning is logged.
    /// </summary>
    /// <param name="mod">The mod for which to display the configuration config. Must not be null.</param>
    public static void ShowConfigPanel(ModMenuEntry mod)
    {
        // Return if a panel is already open
        if (currentPanel is not null)
            return;

        // Log a warning if there is no config registered
        if (mod.Config is null || mod.Config.IsEmpty)
        {
            ConfigLogger.Warning($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.");
            return;
        }

        if(mod.Config.Panel is null)
        {
            ConfigLogger.Error($"Failed to open mod config panel for {mod.DisplayName}: Config UI panel has not been created.");
            return;
        }

        mod.Config.Panel.ShowPanel();
        currentPanel = mod.Config.Panel;
    }

    /// <summary>
    /// Hides the currently displayed configuration config, if there is one.
    /// </summary>
    public static void HideConfigPanel()
    {
        if (currentPanel is null)
            return;

        currentPanel.HidePanel();
        currentPanel = null;
    }
}