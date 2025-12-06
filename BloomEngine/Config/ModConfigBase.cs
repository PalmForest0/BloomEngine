using BloomEngine.Inputs;
using MelonLoader;

namespace BloomEngine.Config;

/// <summary>
/// Base config class that allows adding properties with <see cref="AddProperty{T}(string, T, Action{T}, Func{T, bool}, Func{T, T})"/>. 
/// </summary>
public class ModConfigBase
{
    //internal List<IConfigProperty> Properties { get; } = new();
    internal List<IInputField> InputFields { get; } = new List<IInputField>();

    public StringInputField AddStringInput(string name, string defaultValue, Action<string> onValueChanged = null, Action onInputChanged = null, Func<string, string> transformValue = null, Func<string, bool> validateValue = null)
    {
        var inputField = new StringInputField(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);
        InputFields.Add(inputField);
        return inputField;
    }

    public IntInputField AddIntInput(string name, int defaultValue, Action<int> onValueChanged = null, Action onInputChanged = null, Func<int, int> transformValue = null, Func<int, bool> validateValue = null)
    {
        var inputField = new IntInputField(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);
        InputFields.Add(inputField);
        return inputField;
    }

    public FloatInputField AddFloatInput(string name, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null, Action onInputChanged = null, Func<float, float> transformValue = null, Func<float, bool> validateValue = null)
    {
        var inputField = new FloatInputField(name, defaultValue, minValue, maxValue, onValueChanged, onInputChanged, transformValue, validateValue);
        InputFields.Add(inputField);
        return inputField;
    }

    public BoolInputField AddBoolInput(string name, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null)
    {
        var inputField = new BoolInputField(name, defaultValue, onValueChanged, onInputChanged, null, null);
        InputFields.Add(inputField);
        return inputField;
    }

    public EnumInputField AddEnumInput(string name, Enum defaultValue, Action<Enum> onValueChanged = null, Action onInputChanged = null, Func<Enum, bool> validateValue = null)
    {
        var inputField = new EnumInputField(name, defaultValue, onValueChanged, onInputChanged, null, validateValue);
        InputFields.Add(inputField);
        return inputField;
    }


    /// <summary>
    /// Adds a property to this config class, which can be passed to <see cref="Menu.ModEntry.AddConfig(ModConfigBase)"/> to register it.
    /// </summary>
    /// <typeparam name="T">The value type of this property.</typeparam>
    /// <param name="name">The text that will appear next to the property input inputField in the config panel.</param>
    /// <param name="defaultValue">Default value of this property.</param>
    /// <param name="onValueUpdated">A callback that provides the new value when it is updated.</param>
    /// <param name="validateFunc">A function that validates the value before the setter is called.</param>
    /// <param name="transformFunc">A function that allows any transformation to be made to the value before the setter is called.</param>
    /// <returns>A wrapper for this config property. Use <see cref="ConfigProperty{T}.Value"/> to get or set the value of this property.</returns>
    //public ConfigProperty<T> AddProperty<T>(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    //{
    //    if (!IConfigProperty.IsValidPropertyType<T>())
    //    {
    //        Melon<BloomEngineMod>.Logger.Error($"Attempted to add a config property with an unsupported type. Supported types currently include numeric types and strings.");
    //        return null;
    //    }

    //    var property = new ConfigProperty<T>(new ConfigPropertyData<T>(name, defaultValue, onValueUpdated, validateFunc, transformFunc));
    //    Properties.Add(property);
    //    return property;
    //}

    /// <summary>
    /// Retrieves a config property with the specified name and type.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the config property.</typeparam>
    /// <param name="name">The name of the config property to retrieve.</param>
    /// <returns>The config property of type <typeparamref name="T"/> with the specified name, or <c>null</c> if no
    /// matching property is found.</returns>
    //public ConfigProperty<T> GetProperty<T>(string name) => (ConfigProperty<T>) Properties.FirstOrDefault(prop => prop.Name == name);
}