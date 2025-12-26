using BloomEngine.Interfaces;
using BloomEngine.Modules.Config.Inputs;
using BloomEngine.Modules.Menu;

namespace BloomEngine.Modules.Config;

/// <summary>
/// A static class which provides methods for creating input fields for a mod config menu.
/// The supported types are: <c>string, int, float, bool and enum</c>
/// </summary>
public static class ConfigMenu
{
    /// <summary>
    /// Creates a <see cref="string"/> input field in the form of a textbox in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>A <see cref="StringInputField"/> instance. You can read and modify the value of this config property using <see cref="IInputField{T}.Value"/>.</returns>
    public static StringInputField CreateStringInput(string name, string defaultValue, Action<string> onValueChanged = null, Action onInputChanged = null, Func<string, string> transformValue = null, Func<string, bool> validateValue = null)
        => new StringInputField(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);

    /// <summary>
    /// Creates an <see cref="int"/> input field in the form of a numeric textbox in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>An <see cref="IntInputField"/> instance. You can read and modify the value of this config property using <see cref="IInputField{T}.Value"/>.</returns>
    public static IntInputField CreateIntInput(string name, int defaultValue, Action<int> onValueChanged = null, Action onInputChanged = null, Func<int, int> transformValue = null, Func<int, bool> validateValue = null)
        => new IntInputField(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);


    /// <summary>
    /// Creates a <see cref="float"/> input field in the form of a slider in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="minValue">The minimum value of this input field.</param>
    /// <param name="maxValue">The maximum value of this input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>A <see cref="FloatInputField"/> instance. You can read and modify the value of this config property using <see cref="IInputField{T}.Value"/>.</returns>
    public static FloatInputField CreateFloatInput(string name, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null, Action onInputChanged = null, Func<float, float> transformValue = null, Func<float, bool> validateValue = null)
        => new FloatInputField(name, defaultValue, minValue, maxValue, onValueChanged, onInputChanged, transformValue, validateValue);


    /// <summary>
    /// Creates a <see cref="bool"/> input field in the form of a checkbox in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <returns>A <see cref="BoolInputField"/> instance. You can read and modify the value of this config property using <see cref="IInputField{T}.Value"/>.</returns>
    public static BoolInputField CreateBoolInput(string name, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null)
        => new BoolInputField(name, defaultValue, onValueChanged, onInputChanged, null, null);


    /// <summary>
    /// Creates an <see cref="Enum"/> input field in the form of a dropdown in your mod's config menu and returns it.
    /// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
    /// and call <see cref="ModEntry.AddConfig(Type)"/>, passing in your config class as the type.
    /// </summary>
    /// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
    /// <param name="defaultValue">This default value of this config input field.</param>
    /// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
    /// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
    /// <param name="validateValue">A function to validate the new value before it is updated.</param>
    /// <returns>An <see cref="EnumInputField"/> instance. You can read and modify the value of this config property using <see cref="IInputField{T}.Value"/>.</returns>
    public static EnumInputField CreateEnumInput(string name, Enum defaultValue, Action<Enum> onValueChanged = null, Action onInputChanged = null, Func<Enum, bool> validateValue = null)
        => new EnumInputField(name, defaultValue, onValueChanged, onInputChanged, null, validateValue);
}