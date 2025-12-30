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
    

    /// <summary>
    /// Creates a <see cref="string"/> input field in the form of a textbox in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>A <see cref="StringConfigInput"/> instance. You can read and modify the value of this config property using <see cref="BaseConfigInput{T}.Value"/>.</returns>
    public static StringConfigInput CreateStringInput(string name, string defaultValue, Action<string> onValueChanged = null, Action onInputChanged = null, Func<string, string> transformValue = null, Func<string, bool> validateValue = null)
        => new StringConfigInput(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);

    /// <summary>
    /// Creates an <see cref="int"/> input field in the form of a numeric textbox in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>An <see cref="IntConfigInput"/> instance. You can read and modify the value of this config property using <see cref="BaseConfigInput{T}.Value"/>.</returns>
    public static IntConfigInput CreateIntInput(string name, int defaultValue, Action<int> onValueChanged = null, Action onInputChanged = null, Func<int, int> transformValue = null, Func<int, bool> validateValue = null)
        => new IntConfigInput(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);


    /// <summary>
    /// Creates a <see cref="float"/> input field in the form of a slider in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="minValue">The minimum value of this input field.</param>
    /// <param name="maxValue">The maximum value of this input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>A <see cref="FloatConfigInput"/> instance. You can read and modify the value of this config property using <see cref="BaseConfigInput{T}.Value"/>.</returns>
    public static FloatConfigInput CreateFloatInput(string name, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null, Action onInputChanged = null, Func<float, float> transformValue = null, Func<float, bool> validateValue = null)
        => new FloatConfigInput(name, defaultValue, minValue, maxValue, onValueChanged, onInputChanged, transformValue, validateValue);


    /// <summary>
    /// Creates a <see cref="bool"/> input field in the form of a checkbox in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <returns>A <see cref="BoolConfigInput"/> instance. You can read and modify the value of this config property using <see cref="BaseConfigInput{T}.Value"/>.</returns>
    public static BoolConfigInput CreateBoolInput(string name, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null)
        => new BoolConfigInput(name, defaultValue, onValueChanged, onInputChanged, null, null);


    /// <summary>
    /// Creates an <see cref="Enum"/> input field in the form of a dropdown in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>An <see cref="EnumConfigInput"/> instance. You can read and modify the value of this config property using <see cref="BaseConfigInput{T}.Value"/>.</returns>
    public static EnumConfigInput CreateEnumInput(string name, Enum defaultValue, Action<Enum> onValueChanged = null, Action onInputChanged = null, Func<Enum, bool> validateValue = null)
        => new EnumConfigInput(name, defaultValue, onValueChanged, onInputChanged, null, validateValue);
}