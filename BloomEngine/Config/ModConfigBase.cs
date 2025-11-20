using BloomEngine.Utilities;
using MelonLoader;
using System.Collections.ObjectModel;

namespace BloomEngine.Config;

public class ModConfigBase
{
    private List<IConfigProperty> properties = new List<IConfigProperty>();
    public ReadOnlyCollection<IConfigProperty> Properties => properties.AsReadOnly();

    public ConfigProperty<T> AddProperty<T>(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    {
        if (!IsValidPropertyType<T>())
        {
            Melon<BloomEnginePlugin>.Logger.Error($"Attempted to add a config property with an unsupported type. Supported types currently include numeric types and strings.");
            return null;
        }

        var property = new ConfigProperty<T>(name, defaultValue, onValueUpdated, validateFunc, transformFunc);
        properties.Add(property);
        return property;
    }

    public IConfigProperty GetProperty(string name) => properties.Find(prop => prop.Name == name);

    /// <summary>
    /// Checks if the type T is a valid property type.
    /// Support for more types like boolean and enums can be added later.
    /// </summary>
    private static bool IsValidPropertyType<T>()
        => TypeHelper.IsNumericType(typeof(T)) || typeof(T) == typeof(string);
}